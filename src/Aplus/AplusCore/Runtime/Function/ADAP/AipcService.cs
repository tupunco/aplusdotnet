using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    internal class AipcService
    {
        #region Variables

        private Aplus environment;

        private int actualHandleNumber;
        private Dictionary<int, AipcConnection> roster;
        private HashSet<AipcConnection> retryList;
        private object mutex;
        private Thread networkThread;

        #endregion

        #region Properties

        public Aplus Environment { get { return this.environment; } }

        public HashSet<AipcConnection> RetryList { get { return retryList; } }

        #endregion

        #region Constructors

        internal AipcService(Aplus environment)
        {
            this.environment = environment;

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

        internal void AddToRetryList(AipcConnection conn)
        {
            lock (mutex)
            {
                retryList.Add(conn);
            }
        }

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
                    catch (SocketException)
                    {
                        continue;
                    }
                }

                foreach (Socket socket in readList)
                {
                    AipcConnection connection =
                        connectionList.Where<AipcConnection>(conn => conn.Socket == socket).FirstOrDefault<AipcConnection>();

                    if (connection == null || !connection.isOpen)
                    {
                        continue;
                    }

                    try
                    {
                        AType message = connection.Read();
                    }
                    catch (ADAPException)
                    {
                        this.Close(connection.ConnectionAttributes.HandleNumber);
                        connection.MakeCallback("reset", ASymbol.Create("readImport"));
                    }
                    catch (SocketException exception)
                    {
                        writeList.Remove(connection.Socket); // this should only happen at unknown state but...
                        CallbackBySocketException(connection, exception, true);
                    }
                    catch (ObjectDisposedException)
                    {
                        this.Close(connection.ConnectionAttributes.HandleNumber);
                    }
                    catch (NullReferenceException)
                    {
                        this.Close(connection.ConnectionAttributes.HandleNumber);
                    }
                }

                foreach (Socket socket in writeList)
                {
                    AipcConnection connection =
                        connectionList.Where<AipcConnection>(conn => conn.Socket == socket).FirstOrDefault<AipcConnection>();

                    if (connection == null || !connection.isOpen)
                    {
                        continue;
                    }

                    try
                    {
                        connection.Send();
                    }
                    catch (SocketException exception)
                    {
                        CallbackBySocketException(connection, exception, false);
                    }
                    catch (ObjectDisposedException)
                    {
                        this.Close(connection.ConnectionAttributes.HandleNumber);
                    }
                    catch (NullReferenceException)
                    {
                        this.Close(connection.ConnectionAttributes.HandleNumber);
                    }
                }

                HashSet<AipcConnection> actualRetryList;

                lock (mutex)
                {
                    actualRetryList = retryList;
                    retryList = new HashSet<AipcConnection>();
                }

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

        internal int GetPortByServiceName(AType name)
        {
            lock (mutex)
            {
                foreach (KeyValuePair<int, AipcConnection> item in roster)
                {
                    ConnectionAttribute attrbiutes = item.Value.ConnectionAttributes;
                    if (attrbiutes.IsListener && attrbiutes.Name.asString == name.asString)
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

            if (connection == null)
            {
                return Utils.ANull();
            }

            return connection.AipcAttributes.Attributes();
        }

        public AType GetAttribute(int handle, AType attribute)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return Utils.ANull();
            }

            return connection.AipcAttributes.GetAttribute(attribute);
        }

        public AType SetAttribute(int handle, AType attribute, AType value)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return AInteger.Create(-1);
            }

            return connection.AipcAttributes.SetAttribute(attribute, value);
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
            int nowSeconds = Convert.ToInt32(Math.Floor(timestamp.TotalSeconds));
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
                    && argument[1].asInteger > 0)
                {
                    result = argument;
                }
                else
                {
                    seconds = (argument.IsArray) ? argument[0].asInteger : argument.asInteger;
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

        public AType Listen(AType func, AType name, AType host, int port, AType protocol)
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

            connection = Create(attribute, null, null);

            if (connection == null)
            {
                return AInteger.Create(-1);
            }

            return AInteger.Create(connection.ConnectionAttributes.HandleNumber);
        }

        public AType Listen(AType func, AType name)
        {
            return Listen(
                func,
                name,
                ConnectionAttribute.DEFAULT_HOST,
                ConnectionAttribute.DEFAULT_PORT,
                ConnectionAttribute.DEFAULT_PROTOCOL
            );
        }

        public AType Listen(AType func, AType name, AType protocol)
        {
            return Listen(func, name, ConnectionAttribute.DEFAULT_HOST, ConnectionAttribute.DEFAULT_PORT, protocol);
        }

        public void InitFromListener(ConnectionAttribute connectionAttribute, AipcAttribute aipcAttributes, Socket socket)
        {
            ConnectionAttribute attribute = connectionAttribute.Clone();
            attribute.IsListener = false;
            AipcConnection connection = Create(attribute, aipcAttributes, socket);

            connection.AipcAttributes.Listener = connectionAttribute.HandleNumber;
            connection.MakeCallback("connected", AInteger.Create(connection.AipcAttributes.Listener));
        }

        private AipcConnection Create(ConnectionAttribute connectionAttribute, AipcAttribute aipcAttributes, Socket socket)
        {
            AipcConnection connection;

            switch (connectionAttribute.Protocol.asString)
            {
                case "A":
                    connection = new AConnection(this, connectionAttribute, aipcAttributes, socket);
                    break;
                case "string":
                    connection = new StringConnection(this, connectionAttribute, aipcAttributes, socket);
                    break;
                case "raw":
                    connection = new RawConnection(this, connectionAttribute, aipcAttributes, socket);
                    break;
                case "simple":
                    connection = new SimpleConnection(this, connectionAttribute, aipcAttributes, socket);
                    break;
                default:
                    connectionAttribute.HandleNumber = -1;
                    connection = null;
                    break;
            }

            if (connectionAttribute.HandleNumber != -1)
            {
                connectionAttribute.HandleNumber = NextHandleNumber();
                AddToRoster(connection);
            }

            return connection;
        }

        public AType Connect(
            AType func, AType name, AType host, int port, AType protocol)
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

            connection = Create(attribute, null, null);

            if (connection == null)
            {
                return AInteger.Create(-1);
            }

            return AInteger.Create(connection.ConnectionAttributes.HandleNumber);
        }

        public AType Connect(AType func, AType name)
        {
            return Connect(
                func,
                name,
                ConnectionAttribute.DEFAULT_HOST,
                ConnectionAttribute.DEFAULT_PORT,
                ConnectionAttribute.DEFAULT_PROTOCOL
            );
        }

        public AType Connect(AType func, AType name, AType protocol)
        {
            return Connect(
                func,
                name,
                ConnectionAttribute.DEFAULT_HOST,
                ConnectionAttribute.DEFAULT_PORT,
                protocol
            );
        }

        public AType Open(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return AInteger.Create(-1);
            }

            connection.Open();
            return AInteger.Create(0);
        }

        public AType Destroy(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return AInteger.Create(-1);
            }

            RemoveFromRoster(handle);
            connection.Destroy();
            return AInteger.Create(0);
        }

        public AType Close(int handle)
        {
            AipcConnection connection = Lookup(handle);

            if (connection == null)
            {
                return AInteger.Create(-1);
            }

            if (!connection.ConnectionAttributes.IsListener && connection.AipcAttributes.Listener == 0)
            {
                connection.Reset();
            }
            else
            {
                connection.Close();
            }

            return AInteger.Create(0);
        }

        public AType SyncSend(int handle, AType message, AType timeout)
        {
            AipcConnection connection = Lookup(handle);
            AType result;

            if (connection == null)
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
                result = Utils.ANull();
            }

            return result;
        }

        public AType SyncRead(int handle, AType timeout)
        {
            AipcConnection connection = Lookup(handle);
            AType result;

            if (connection == null)
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
                result = Utils.ANull();
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

        private void CallbackBySocketException(AipcConnection connection, SocketException exception, bool isRead)
        {
            int handle = connection.ConnectionAttributes.HandleNumber;

            switch (exception.SocketErrorCode)
            {
                case SocketError.TimedOut:
                    connection.MakeCallback("timeout", ASymbol.Create("Timeout"));
                    break;
                case SocketError.Interrupted:
                    connection.MakeCallback("interrupt", ASymbol.Create("Interrupted"));
                    break;
                case SocketError.NoBufferSpaceAvailable:
                    connection.MakeCallback(isRead ? "buffread" : "buffwrite",
                                            ASymbol.Create(String.Concat(isRead ? "Read " : "Write ", "buffer is full")));
                    this.Close(connection.ConnectionAttributes.HandleNumber);
                    break;
                case SocketError.NotInitialized:
                case SocketError.NotConnected:
                    this.Close(connection.ConnectionAttributes.HandleNumber);
                    break;
                case SocketError.ConnectionReset:
                default:
                    connection.MakeCallback("reset", ASymbol.Create("unknown State"));
                    this.Close(connection.ConnectionAttributes.HandleNumber);
                    break;
            }
        }

        #endregion
    }
}
