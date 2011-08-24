using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using AplusCore.Compiler;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Function.ADAP
{
    public abstract class AipcConnection
    {
        #region Variables

        private static Func<AType, Aplus, AType, AType, AType, AType> CallbackFunction;
        protected static ASCIIEncoding ASCIIEncoder = new ASCIIEncoding();

        private AipcService aipcService;
        protected Socket connectionSocket;

        private ConnectionAttribute connectionAttributes;
        private AipcAttribute aipcAttributes;
        protected LinkedList<byte[]> writeBuffer = new LinkedList<byte[]>();
        protected bool partialSent;

        #endregion

        #region Properties

        internal AipcService AipcService { get { return this.aipcService; } }

        public ConnectionAttribute ConnectionAttributes { get { return this.connectionAttributes; } }
        public AipcAttribute AipcAttributes { get { return this.aipcAttributes; } }

        public bool isOpen { get { return (this.connectionSocket == null) ? false : this.connectionSocket.Connected; } }
        public Socket Socket
        {
            get { return this.connectionSocket; }
            set { this.connectionSocket = value; }
        }

        public int WriteBufferContentSize
        {
            get { return writeBuffer.Sum(n => n.Length); }
        }

        public bool PartialSent { get { return this.partialSent; } }

        #endregion

        #region Constructors

        static AipcConnection()
        {
            DLR.ParameterExpression functionParameter = DLR.Expression.Parameter(typeof(AType), "_FUNCTION_");

            DLR.ParameterExpression environmentParameter = DLR.Expression.Parameter(typeof(Aplus), "_ENVIRONMENT_");
            DLR.ParameterExpression handleParameter = DLR.Expression.Parameter(typeof(AType), "_HANDLE_NUMBER_");
            DLR.ParameterExpression eventTypeParameter = DLR.Expression.Parameter(typeof(AType), "_EVENT_TYPE_");
            DLR.ParameterExpression callDataParameter = DLR.Expression.Parameter(typeof(AType), "_CALL_DATA_");

            /**
             * Build the following lambda method:
             *  (function, env, handleNumber, eventType, callData) => function(env, callData, eventType, handleNumber);
             */
            DLR.Expression<Func<AType, Aplus, AType, AType, AType, AType>> method =
                DLR.Expression.Lambda<Func<AType, Aplus, AType, AType, AType, AType>>(
                    DLR.Expression.Convert(
                        DLR.Expression.Dynamic(
                            new Binder.InvokeBinder(new DYN.CallInfo(4)),
                            typeof(object),
                            functionParameter,
                            environmentParameter,
                            callDataParameter,
                            eventTypeParameter,
                            handleParameter
                        ),
                        typeof(AType)
                    ),
                    true,
                    functionParameter,
                    environmentParameter,
                    handleParameter,
                    eventTypeParameter,
                    callDataParameter
                );

            CallbackFunction = method.Compile();
        }

        internal AipcConnection(
            AipcService aipcService,
            ConnectionAttribute connectionAttribute,
            AipcAttribute aipcAttribute,
            Socket socket)
        {
            this.aipcService = aipcService;
            this.connectionAttributes = connectionAttribute;
            this.partialSent = false;

            if (aipcAttribute != null)
            {
                this.aipcAttributes = aipcAttribute.CreateNew(this);
            }
            else
            {
                this.aipcAttributes = AipcAttribute.Create(this);
            }

            this.Socket = socket;
        }

        internal AipcConnection(AipcService aipcService, ConnectionAttribute connectionAttribute)
            : this(aipcService, connectionAttribute, null, null)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Calls a callback function.
        /// </summary>
        /// <param name="eventTypeName"></param>
        /// <param name="callData"></param>
        internal void MakeCallback(string eventTypeName, AType callData)
        {
            AType handle = AInteger.Create(this.ConnectionAttributes.HandleNumber);
            AType eventType = ASymbol.Create(eventTypeName);

            CallbackFunction(
                this.ConnectionAttributes.Function,
                this.AipcService.Environment,
                handle,
                eventType,
                callData);
        }

        /// <summary>
        /// Creates an IPEndPoint from a string and a port.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="port"></param>
        /// <exception cref="FormatException">Throws if the ip-adress is invalid.</exception>
        /// <returns></returns>
        private static IPEndPoint CreateIPEndpoint(string endPoint, int port)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(endPoint, out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }

            return new IPEndPoint(ip, port);
        }

        /// <summary>
        /// Callback function for completion of the socket-connect.
        /// </summary>
        /// <param name="result"></param>
        /// <exception cref="SocketException">Throws if there is something wrong with the socket.</exception>
        /// <exception cref="ObjectDisposedException">Throws if the socket is disposed.</exception>
        /// <exception cref="NullReferenceException">Throws if the socket is destroyed.</exception>
        private void Connect(IAsyncResult result)
        {
            try
            {
                this.connectionSocket.EndConnect(result);
                this.MakeCallback("connected", AInteger.Create(aipcAttributes.Listener));
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Asynchronously accepts a socket.
        /// </summary>
        /// <exception cref="SocketException">Throws if there is something wrong with the socket.</exception>
        /// <exception cref="ObjectDisposedException">Throws if the socket is disposed.</exception>
        /// <exception cref="NullReferenceException">Throws if the socket is destroyed.</exception>
        private void AcceptSocket()
        {
            AsyncCallback callback = new AsyncCallback(CreateConnection);
            this.connectionSocket.BeginAccept(callback, new object());
        }

        /// <summary>
        /// Callback function for the connection creation.
        /// </summary>
        /// <param name="result"></param>
        private void CreateConnection(IAsyncResult result)
        {
            try
            {
                Socket socket = this.connectionSocket.EndAccept(result);
                this.AipcService.InitFromListener(this.connectionAttributes, this.aipcAttributes, socket);

                AcceptSocket();
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Creates a result for the SyncRead/SyncSend depending on the error type.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected AType SyncFillError(SocketError errorCode, bool isRead)
        {
            string category;
            string detail;

            switch (errorCode)
            {
                case SocketError.TimedOut:
                    category = "timeout";
                    detail = "Syncread loop timed out";
                    break;
                case SocketError.ConnectionReset:
                    category = "reset";
                    detail = "Reset occured. No message read.";
                    break;
                case SocketError.Interrupted:
                    category = "interrupt";
                    detail = "select() received an interrupt";
                    break;
                case SocketError.NoBufferSpaceAvailable:
                    category = isRead ? "buffread" : "buffwrite";
                    detail = String.Concat(category, " returned error");
                    break;
                case SocketError.NotInitialized:
                case SocketError.NotConnected:
                    category = "nochan";
                    detail = "channel is null";
                    break;
                default:
                    category = "other";
                    detail = "other error occured";
                    break;
            }

            AType result = AArray.Create(ATypes.ABox,
                                         ABox.Create(ASymbol.Create("error")),
                                         ABox.Create(ASymbol.Create(category)),
                                         ABox.Create(Helpers.BuildString(detail))
                                         );

            return result;
        }

        /// <summary>
        /// Sends message from the buffer.
        /// </summary>
        /// <exception cref="SocketException">If there is some problem with the socket.</exception>
        /// <exception cref="ArgumentNullException">If the socket is set to null before the send.</exception>
        /// <exception cref="ObjectDisposedException">If the socket is disposed.</exception>
        public void Send()
        {
            if (!aipcAttributes.WritePause && writeBuffer.Count > 0)
            {
                byte[] message = writeBuffer.First.Value;
                int sentLength = this.connectionSocket.Send(message);

                writeBuffer.RemoveFirst();

                if (sentLength == message.Length)
                {
                    partialSent = false;
                    this.MakeCallback("sent", this.aipcAttributes.GetWriteQueue());
                }
                else
                {
                    partialSent = true;
                    byte[] remainingByte = new byte[message.Length - sentLength];

                    Array.Copy(message, message.Length - sentLength, remainingByte, 0, sentLength);

                    writeBuffer.AddFirst(remainingByte);
                }
            }
        }

        /// <summary>
        /// Creates a result for the SyncRead/SyncWrite if it succeed
        /// </summary>
        protected AType SyncFillOk(AType message, bool isRead)
        {
            return AArray.Create(
                ATypes.ABox,
                ABox.Create(ASymbol.Create("OK")),
                ABox.Create(message),
                ABox.Create(isRead ? Utils.ANull() : AipcAttributes.GetWriteQueue())
            );
        }

        /// <summary>
        /// Sets the connection socket's attributes from the AipcAttributes.
        /// </summary>
        /// <exception cref="SocketException">If there is some problem with the socket.</exception>
        /// <exception cref="ObjectDisposedException">If the socket is disposed.</exception>
        internal void SetSocket()
        {
            if (this.connectionSocket != null)
            {
                if (aipcAttributes.ReadBufsize < 0)
                {
                    connectionSocket.ReceiveBufferSize = 8192; // FIX ##?: separate constant?
                }

                if (aipcAttributes.WriteBufsize < 0)
                {
                    connectionSocket.SendBufferSize = 8192; // FIX ##?: separate constant?
                }

                this.connectionSocket.NoDelay = aipcAttributes.NoDelay;
                this.connectionSocket.ReceiveBufferSize = aipcAttributes.ReadBufsize;
                this.connectionSocket.SendBufferSize = aipcAttributes.WriteBufsize;
            }
        }

        #endregion

        #region Connection related

        /// <summary>
        /// Opens a connection.
        /// </summary>
        /// <returns></returns>
        public int Open()
        {
            if (this.isOpen)
            {
                return 0;
            }

            // Find the correct port number:
            if (!this.connectionAttributes.IsListener && connectionAttributes.Port == 0)
            {
                if (connectionAttributes.Host.asString != "localhost" || connectionAttributes.Host.asString != "127.0.0.1")
                {
                    return 0;
                }

                int port = this.AipcService.GetPortByServiceName(connectionAttributes.Name);

                if (port != 0)
                {
                    connectionAttributes.Port = port;
                }
                else
                {
                    this.AipcService.AddToRetryList(this);
                    return 0;
                }
            }

            this.connectionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                SetSocket();
            }
            catch (SocketException)
            {
                this.AipcService.AddToRetryList(this);
                return 0;
            }
            catch (ObjectDisposedException)
            {
                this.AipcService.AddToRetryList(this);
                return 0;
            }

            if (this.connectionAttributes.IsListener)
            {
                OpenListener();
            }
            else
            {
                OpenClient();
            }

            return 0;
        }

        /// <summary>
        /// Creates a client socket.
        /// </summary>
        private void OpenClient()
        {
            IPEndPoint ip;

            try
            {
                ip = CreateIPEndpoint(this.ConnectionAttributes.Host.asString, this.ConnectionAttributes.Port);
            }
            catch (FormatException)
            {
                IPHostEntry hostAddress = Dns.GetHostEntry(this.ConnectionAttributes.Host.asString);
                ip = new IPEndPoint(
                    hostAddress.AddressList.Where(p => p.AddressFamily == AddressFamily.InterNetwork).Last(),
                    this.ConnectionAttributes.Port
                );
            }

            try
            {
                this.connectionSocket.BeginConnect(ip, new AsyncCallback(Connect), null);
            }
            catch (SocketException)
            {
                this.AipcService.AddToRetryList(this);
            }
            catch (ObjectDisposedException)
            {
                this.AipcService.AddToRetryList(this);
            }
            catch (NullReferenceException)
            {
                this.AipcService.AddToRetryList(this);
            }
        }

        /// <summary>
        /// Creates a server socket.
        /// </summary>
        private void OpenListener()
        {
            bool retryRequired = true;

            try
            {
                this.connectionSocket.Bind(new IPEndPoint(IPAddress.Any, ConnectionAttributes.Port));
                this.connectionSocket.Listen(100);
                this.AcceptSocket();

                retryRequired = false;
            }
            catch (SocketException)
            {
                // Intentionally left blank.
            }
            catch (ObjectDisposedException)
            {
                // Intentionally left blank.
            }
            catch (NullReferenceException)
            {
                // Intentionally left blank.
            }

            if (retryRequired && this.aipcAttributes.Retry)
            {
                this.AipcService.AddToRetryList(this);
            }

        }

        /// <summary>
        /// Destroys the connection socket.
        /// </summary>
        /// <returns></returns>
        public int Close()
        {
            if (this.connectionAttributes.ZeroPort)
            {
                this.connectionAttributes.Port = 0;
            }

            if (this.isOpen)
            {
                this.connectionSocket.Close();
                this.connectionSocket = null;
            }

            return 1;
        }

        /// <summary>
        /// Destroys the connection socket, and adds the connection to the retry-list, should called from AipcService
        /// </summary>
        /// <returns></returns>
        public int Reset()
        {
            if (this.isOpen)
            {
                this.connectionSocket.Disconnect(false);
                this.connectionSocket = null;
            }

            this.AipcService.AddToRetryList(this);

            return 1;
        }


        /// <summary>
        /// Destroys the connection socket, should called from AipcService
        /// </summary>
        /// <returns></returns>
        public int Destroy()
        {
            if (this.connectionSocket != null)
            {
                this.connectionSocket.Dispose();
                this.connectionSocket = null;
            }

            return 0;
        }


        /// <summary>
        /// Sends an AType.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>0 if the message is not fully sent, 1 if the message is sent, -1 if there is any error.</returns>
        public AType Send(AType message)
        {
            if (!isOpen)
            {
                return AInteger.Create(-1);
            }

            byte[] byteMessage;

            try
            {
                byteMessage = ConvertToByte(message);
            }
            catch (ADAPException)
            {
                return AInteger.Create(-1);
            }

            if (this.aipcAttributes.WritePause)
            {
                this.writeBuffer.AddLast(byteMessage);
                return AInteger.Create(0);
            }
            else
            {
                int sentLength = 0;

                if (writeBuffer.Count == 0 && !aipcAttributes.WritePause)
                {
                    sentLength = connectionSocket.Send(byteMessage);
                }

                if (sentLength != byteMessage.Length)
                {
                    byte[] messageRemaining = new byte[byteMessage.Length - sentLength];
                    Array.Copy(byteMessage, sentLength, messageRemaining, 0, byteMessage.Length - sentLength);
                    this.writeBuffer.AddFirst(messageRemaining);
                    return AInteger.Create(0);
                }

                return AInteger.Create(1);
            }
        }

        /// <summary>
        /// Reads an AType.
        /// </summary>
        /// <exception cref="SocketException">Throwed if there is a problem with the socket connection.</exception>
        /// <exception cref="ADAPException">Throwed if the received AType is invalid.</exception>
        /// <exception cref="ObjectDisposedException">Throwed if the socket is disposed.</exception>
        /// <exception cref="NullReferenceException">Throwed if the socket is set to null.</exception>
        /// <returns></returns>
        public abstract AType Read();

        public abstract AType SyncSend(AType message, AType timeout);

        public abstract AType SyncRead(AType timeout);

        /// <summary>
        /// Converts an AType to byte array.
        /// </summary>
        /// <param name="message">The message to be converted.</param>
        /// <exception cref="ADAPException">Throwed if the received AType is invalid.</exception>
        /// <returns></returns>
        protected abstract byte[] ConvertToByte(AType message);

        /// <summary>
        /// Converts a byte array to AType.
        /// </summary>
        /// <param name="message">The message to be converted.</param>
        /// <exception cref="ADAPException">Throwed if the received AType is invalid.</exception>
        /// <returns></returns>
        protected abstract AType ConvertToAObject(byte[] message);

        #endregion
    }
}
