using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    public class AipcService
    {
        #region Variables

        private static AipcService singletonInstance = new AipcService();

        private int actualHandleNumber;
        private Dictionary<int, AipcConnection> roster;
        private HashSet<AipcConnection> retryList;
        private object mutex;
        private Thread networkThread;

        #endregion

        #region Properties

        public static AipcService Instance { get { return singletonInstance; } }
        public HashSet<AipcConnection> RetryList { get { return retryList; } }

        #endregion

        #region Constructors

        private AipcService()
        {
            this.actualHandleNumber = 3210000;
            this.roster = new Dictionary<int, AipcConnection>();
            this.retryList = new HashSet<AipcConnection>();
            this.mutex = new object();
            this.networkThread = new Thread(new ThreadStart(NetworkLoop));
            this.networkThread.IsBackground = true;
            this.networkThread.Start();
        }

        #endregion

        #region Methods

        private void NetworkLoop()
        {
            while (true)
            {
                List<AipcConnection> connectionList = new List<AipcConnection>();
                List<Socket> readList = new List<Socket>();
                List<Socket> writeList = new List<Socket>();
                List<Socket> errorList = new List<Socket>();

                Dictionary<int, AipcConnection> rosterCopy;

                lock (mutex)
                {
                    rosterCopy = new Dictionary<int, AipcConnection>(roster);
                }

                //collect connections without readpause and writepause?!

                foreach (var item in rosterCopy)
                {
                    if (!item.Value.ConnectionAttributes.IsListener && item.Value.Socket != null)
                    {
                        connectionList.Add(item.Value);
                        readList.Add(item.Value.Socket);

                        if (item.Value.WriteBufferContentSize != 0)
                        {
                            writeList.Add(item.Value.Socket);
                        }
                    }
                }

                if (readList.Count != 0 || writeList.Count != 0)
                {
                    try
                    {
                        Socket.Select(readList, writeList, errorList, 100);
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.StackTrace);
                        continue;
                    }
                }

                foreach (Socket socket in readList)
                {

                    AipcConnection connection =
                        connectionList.Where<AipcConnection>(conn => conn.Socket == socket).FirstOrDefault<AipcConnection>();

                    if (connection == null)
                    {
                        continue;
                    }

                    try
                    {
                        AType message = connection.Read();
                    }
                    catch (Error.Invalid)
                    {
                        Console.WriteLine("Call readImport callback here {0}", connection.ConnectionAttributes.HandleNumber);
                    }
                    catch (SocketException exception)
                    {
                        writeList.Remove(connection.Socket); // this should only happen at unknown state but...
                        
                        // FIX ##? pass the connection/attribute
                        CallbackBySocketException(
                            exception, connection.ConnectionAttributes.HandleNumber, connection.AipcAttributes.Listener != 0);
                    }
                }

                foreach (Socket socket in writeList)
                {
                    AipcConnection connection =
                        connectionList.Where<AipcConnection>(conn => conn.Socket == socket).FirstOrDefault<AipcConnection>();

                    if (connection == null)
                    {
                        continue;
                    }

                    try
                    {
                        connection.Send();
                    }
                    catch (SocketException exception)
                    {
                        // FIX ##? pass the connection/attribute
                        CallbackBySocketException(
                            exception, connection.ConnectionAttributes.HandleNumber, connection.AipcAttributes.Listener != 0);
                    }
                }

                HashSet<AipcConnection> actualRetryList = retryList;
                retryList = new HashSet<AipcConnection>();

                foreach (AipcConnection item in actualRetryList)
                {
                    item.Open();
                }
            }
        }

        private AipcConnection Lookup(int handle)
        {
            AipcConnection result = null;

            lock (mutex)
            {
                roster.TryGetValue(handle, out result);
            }

            return result;
        }

        private int NextHandleNumber()
        {
            lock (mutex)
            {
                return ++this.actualHandleNumber;
            }
        }

        private void AddToRoster(AipcConnection connection)
        {
            if (connection != null)
            {
                lock (mutex)
                {
                    roster.Add(actualHandleNumber, connection);
                }
            }
        }

        private void RemoveFromRoster(int handle)
        {
            lock (mutex)
            {
                roster.Remove(handle);
            }
        }

        #endregion

        #region i context related

        public int GetPortByServiceName(AType name)
        {
            lock (mutex)
            {
                foreach (KeyValuePair<int, AipcConnection> item in roster)
                {
                    if (item.Value.ConnectionAttributes.IsListener &&
                        item.Value.ConnectionAttributes.Name.asString == name.asString)
                    {
                        return item.Value.ConnectionAttributes.Port;
                    }
                }

                return 0;
            }
        }

        public AType WriteQueueStatus(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return Utils.ANull();
            }

            return AArray.Create(ATypes.AInteger,
                                 AInteger.Create(connection.WriteBufferContentSize),
                                 AInteger.Create(connection.PartialSent ? 1 : 0)
                                 );

        }

        public AType ReadQueueStatus(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return Utils.ANull();
            }

            return AArray.Create(ATypes.AInteger,
                                 AInteger.Create(connection.Socket.Poll(10, SelectMode.SelectRead) ? 1 : 0),
                                 AInteger.Create(0)  // the acutal implementation reads the whole message!
                                 );
        }

        public AType Attributes(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection != null)
            {
                return connection.AipcAttributes.Attributes();
            }

            return Utils.ANull();
        }

        public AType GetAttribute(int handle, AType attribute)
        {
            AipcConnection connection = Lookup(handle);

            if (connection != null)
            {
                return connection.AipcAttributes.GetAttribute(attribute);
            }

            return Utils.ANull();
        }

        public AType SetAttribute(int handle, AType attribute, AType value)
        {
            AipcConnection connection = Lookup(handle);

            if (connection != null)
            {
                return connection.AipcAttributes.SetAttribute(attribute, value);
            }

            return AInteger.Create(-1);
        }

        public AType WhatIs(int handle)
        {
            AType result = AArray.Create(ATypes.ASymbol);
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                result.Add(ASymbol.Create(""));
                result.Add(ASymbol.Create(""));
            }
            else
            {
                result.Add(ASymbol.Create(connection.ConnectionAttributes.IsListener ? "listener" : "connector"));
                result.Add(connection.ConnectionAttributes.Protocol);
            }

            return result;
        }

        public AType Roster()
        {
            AType result = AArray.Create(ATypes.AInteger);

            lock (mutex)
            {
                foreach (var item in this.roster)
                {
                    result.Add(AInteger.Create(item.Key));
                }
            }
            return result;
        }

        public AType GetTimeout(AType argument)
        {
            //atotv
            TimeSpan timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)); // time since epoch
            AType result;
            int seconds;
            int microseconds;
            int nowSeconds = timestamp.Seconds;
            int nowMicroSeconds = 1000 * timestamp.Milliseconds;
            int argumentLength = argument.Length;

            if (argument.Type == ATypes.AFloat && argument.TryFirstScalar(out result, false))
            {
                seconds = (int)Math.Floor(argument.asFloat);
                microseconds = 1000000 * (int)(argument.asFloat - Math.Floor(argument.asFloat));

                result = AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(nowSeconds + seconds),
                    AInteger.Create(nowMicroSeconds + microseconds),
                    AInteger.Create(1)
                );
            }
            else if (argument.Type == ATypes.AInteger && argumentLength >= 1 && argumentLength <= 3)
            {
                if (argumentLength == 3 && argument[2].asInteger == 1
                    && argument[1].asInteger < 0)
                {
                    result = argument;
                }
                else
                {
                    seconds = argument[0].asInteger;
                    microseconds = (argumentLength >= 2) ? argument[1].asInteger * 1000 : 0;

                    result = AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(nowSeconds + seconds),
                        AInteger.Create(nowMicroSeconds + microseconds),
                        AInteger.Create(1)
                    );
                }
            }
            else
            {
                result = Utils.ANull();
            }

            return result;
        }

        #endregion

        #region i context connection handling

        public int Listen(AType func, AType name, AType host, int port, AType protocol, AipcAttributes aipcAttributes = null)
        {
            AipcConnection connection;
            ConnectionAttribute attribute = new ConnectionAttribute()
            {
                Function = func,
                Name = name,
                Protocol = protocol,
                Port = port,
                ZeroPort = (port == 0),
                Host = host,
                IsListener = true
            };

            switch (attribute.Protocol.asString)
            {
                case "A":
                    connection = new AConnection(attribute, aipcAttributes);
                    break;
                case "string":
                    connection = new StringConnection(attribute, aipcAttributes);
                    break;
                case "raw":
                    connection = new RawConnection(attribute, aipcAttributes);
                    break;
                case "simple":
                    connection = new SimpleConnection(attribute, aipcAttributes);
                    break;
                default:
                    attribute.HandleNumber = -1;
                    connection = null;
                    break;
            }

            if (attribute.HandleNumber != -1)
            {
                attribute.HandleNumber = NextHandleNumber();
                AddToRoster(connection);
            }

            return attribute.HandleNumber;
        }

        public int Listen(AType func, AType name)
        {
            return Listen(
                func,
                name,
                ConnectionAttribute.DEFAULT_HOST,
                ConnectionAttribute.DEFAULT_PORT,
                ConnectionAttribute.DEFAULT_PROTOCOL
            );
        }

        public int Listen(AType func, AType name, AType protocol)
        {
            return Listen(func, name, ConnectionAttribute.DEFAULT_HOST, ConnectionAttribute.DEFAULT_PORT, protocol);
        }

        public void InitFromListener(ConnectionAttribute attribute, Socket socket, AipcAttributes aipcAttributes)
        {
            int handleNumber = Connect(
                attribute.Function, attribute.Name, attribute.Host, attribute.Port, attribute.Protocol, socket, aipcAttributes);
            Console.WriteLine("Call connect callback here with {0}", handleNumber);
        }

        public int Connect(
            AType func, AType name, AType host, int port, AType protocol,
            Socket socket = null, AipcAttributes aipcAttributes = null)
        {
            AipcConnection connection;
            ConnectionAttribute attribute = new ConnectionAttribute
            {
                Function = func,
                Name = name,
                Protocol = protocol,
                Port = port,
                Host = host
            };

            switch (attribute.Protocol.asString)
            {
                case "A":
                    connection = new AConnection(attribute, aipcAttributes, socket);
                    break;
                case "string":
                    connection = new StringConnection(attribute, aipcAttributes, socket);
                    break;
                case "raw":
                    connection = new RawConnection(attribute, aipcAttributes, socket);
                    break;
                case "simple":
                    connection = new SimpleConnection(attribute, aipcAttributes, socket);
                    break;
                default:
                    attribute.HandleNumber = -1;
                    connection = null;
                    break;
            }

            if (attribute.HandleNumber != -1)
            {
                attribute.HandleNumber = NextHandleNumber();
                AddToRoster(connection);
            }

            return attribute.HandleNumber;
        }

        public int Connect(AType func, AType name)
        {
            return Connect(
                func,
                name,
                ConnectionAttribute.DEFAULT_HOST,
                ConnectionAttribute.DEFAULT_PORT,
                ConnectionAttribute.DEFAULT_PROTOCOL
            );
        }

        public int Connect(AType func, AType name, AType protocol)
        {
            return Connect(
                func,
                name,
                ConnectionAttribute.DEFAULT_HOST,
                ConnectionAttribute.DEFAULT_PORT,
                protocol
            );
        }

        public int Open(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return -1;
            }

            connection.Open();
            return 0;
        }

        public int Destroy(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return -1;
            }

            RemoveFromRoster(handle);
            connection.Destroy();
            return 0;
        }

        public int Close(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return -1;
            }

            if (!connection.ConnectionAttributes.IsListener && connection.AipcAttributes.Listener == 0)
            {
                connection.Reset();
            }
            else
            {
                connection.Close();
            }

            return 0;
        }

        public AType SyncSend(int handle, AType message, AType timeout)
        {
            AipcConnection connection = Lookup(handle);
            AType result;

            if (connection == null || !connection.isOpen)
            {
                return Utils.ANull();
            }

            try
            {
                result = connection.SyncSend(message, timeout);
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("This protocol has no SyncSend.");
                return Utils.ANull();
            }

            return result;
        }

        public AType SyncRead(int handle, AType timeout)
        {
            AipcConnection connection = Lookup(handle);
            AType result;

            if (connection == null || !connection.isOpen)
            {
                return Utils.ANull();
            }

            try
            {
                result = connection.SyncRead(timeout);
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("This protocol has no SyncRead");
                return Utils.ANull();
            }

            return result;
        }

        public AType Send(int handle, AType message)
        {
            AipcConnection connection = Lookup(handle);
            AType result;

            if (connection == null || !connection.isOpen)
            {
                return AInteger.Create(-1);
            }

            result = connection.Send(message);

            return result;
        }

        #endregion

        #region Utility

        private void CallbackBySocketException(SocketException exception, int handle, bool listenerInitialized)
        {
            switch (exception.SocketErrorCode)
            {
                case SocketError.ConnectionReset:
                    Console.WriteLine("call reset? callback here with reset?! {0}", handle);
                    break;
                case SocketError.TimedOut:
                    Console.WriteLine("call error callback here with timeout");
                    break;
                case SocketError.Interrupted:
                    Console.WriteLine("call error? callback here with interrupt {0}", handle);
                    break;
                case SocketError.NoBufferSpaceAvailable:
                    Console.WriteLine("call error callback here with buffread {0}", handle);
                    break;
                case SocketError.NotInitialized:
                case SocketError.NotConnected:
                    Console.WriteLine("call error callback here with nochan {0}", handle);
                    break;
                default:
                    Console.WriteLine("call error callback here with unknown state {0}", handle);


                    if (listenerInitialized)
                    {
                        AipcService.Instance.Destroy(handle);
                    }
                    else
                    {
                        AipcService.Instance.Close(handle);
                    }

                    break;
            }
        }

        #endregion
    }
}
