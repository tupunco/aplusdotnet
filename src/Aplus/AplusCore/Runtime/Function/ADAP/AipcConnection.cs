using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    public abstract class AipcConnection
    {
        #region Variables

        protected Socket connectionSocket;

        private ConnectionAttribute connectionAttributes;
        private AipcAttributes aipcAttributes;
        protected LinkedList<byte[]> writeBuffer = new LinkedList<byte[]>();
        protected bool partialSent;

        #endregion

        #region Properties

        public ConnectionAttribute ConnectionAttributes { get { return this.connectionAttributes; } }
        public AipcAttributes AipcAttributes { get { return this.aipcAttributes; } }

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

        public AipcConnection(ConnectionAttribute connectionAttribute, AipcAttributes aipcAttribute = null, Socket socket = null)
        {
            this.connectionAttributes = connectionAttribute;
            this.partialSent = false;

            if (aipcAttribute != null)
            {
                this.aipcAttributes = new AipcAttributes(this, aipcAttribute);
            }
            else
            {
                this.aipcAttributes = new AipcAttributes(this);
            }

            if (connectionAttributes.IsListener)
            {
                aipcAttributes.Listener = connectionAttributes.HandleNumber;
            }

            this.Socket = socket;
        }

        #endregion

        #region Methods

        private static IPEndPoint CreateIPEndpoint(string endPoint, int port)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(endPoint, out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }

            return new IPEndPoint(ip, port);
        }

        private void Connect(IAsyncResult result)
        {
            StateObject stateObject = (StateObject)result.AsyncState;

            try
            {
                this.connectionSocket.EndConnect(result);
                Console.WriteLine("Call connect callback here with {0}", this.connectionAttributes.HandleNumber);
            }
            catch (SocketException)
            {
                this.connectionSocket.BeginConnect(stateObject.endpoint, new AsyncCallback(Connect), stateObject);
            }
            catch (ObjectDisposedException)
            {

            }
        }

        private void AcceptSocket()
        {
            AsyncCallback callback = new AsyncCallback(CreateConnection);
            this.connectionSocket.BeginAccept(callback, new object());
        }

        private void CreateConnection(IAsyncResult result)
        {
            try
            {
                Socket socket = this.connectionSocket.EndAccept(result);
                Console.WriteLine("I'm creating a connection!{0}", connectionAttributes.HandleNumber);

                AipcService.Instance.InitFromListener(this.connectionAttributes, socket, this.aipcAttributes);
                AcceptSocket();
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        protected AType SyncFillError(SocketException exception)
        {
            string category;
            string detail;

            switch (exception.SocketErrorCode)
            {
                case SocketError.TimedOut:
                    category = "timeout";
                    detail = "Syncread loop timed out";
                    break;
                case SocketError.ConnectionReset:
                    category = "reset";
                    detail = "in reset";
                    break;
                case SocketError.Interrupted:
                    category = "interrupt";
                    detail = "interrupted";
                    break;
                case SocketError.NoBufferSpaceAvailable:
                    category = "buffer";
                    detail = "buffer is full";
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
                    Console.WriteLine("Call sent callback here {0}", this.ConnectionAttributes.HandleNumber);
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

        protected AType SyncFillOk(AType message)
        {
            return AArray.Create(
                ATypes.ABox,
                ABox.Create(ASymbol.Create("OK")),
                ABox.Create(message),
                ABox.Create(AipcAttributes.GetWriteQueue())
            );
        }

        internal void SetSocket()
        {
            if (this.connectionSocket != null)
            {
                this.connectionSocket.NoDelay = aipcAttributes.NoDelay;
                this.connectionSocket.ReceiveBufferSize = aipcAttributes.ReadBufsize;
                this.connectionSocket.SendBufferSize = aipcAttributes.WriteBufsize;
            }
        }

        #endregion

        #region Connection related

        public int Open()
        {
            if (this.connectionSocket != null && this.isOpen)
            {
                return 0;
            }

            if (!this.connectionAttributes.IsListener && connectionAttributes.Port == 0)
            {
                int port = AipcService.Instance.GetPortByServiceName(connectionAttributes.Name);

                if (port != 0)
                {
                    connectionAttributes.Port = port;
                }
                else
                {
                    AipcService.Instance.RetryList.Add(this);
                    return 0;
                }
            }

            this.connectionSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            SetSocket();

            if (this.connectionAttributes.IsListener)
            {
                this.connectionSocket.Bind(new IPEndPoint(IPAddress.Any, ConnectionAttributes.Port));

                if (this.connectionAttributes.Port == 0)
                {
                    this.connectionAttributes.Port = ((IPEndPoint)this.connectionSocket.LocalEndPoint).Port;
                }

                this.connectionSocket.Listen(Int32.MaxValue);
                this.AcceptSocket();
            }
            else
            {
                IPEndPoint ip;

                try
                {
                    ip = CreateIPEndpoint(this.connectionAttributes.Host.asString, this.connectionAttributes.Port);
                }
                catch (Exception)
                {
                    IPHostEntry hostAddress = Dns.GetHostEntry(this.connectionAttributes.Host.asString);
                    ip = new IPEndPoint(
                        hostAddress.AddressList.Where(p => p.AddressFamily == AddressFamily.InterNetwork).Last(),
                        this.connectionAttributes.Port
                    );
                }

                this.connectionSocket.BeginConnect(ip, new AsyncCallback(Connect), new StateObject(ip));
            }

            return 0;
        }

        public int Close()
        {
            if (this.connectionAttributes.ZeroPort)
            {
                this.connectionAttributes.Port = 0;
            }

            if (this.isOpen)
            {
                this.connectionSocket.Close();
            }

            return 1;
        }

        public int Reset()
        {
            if (this.isOpen)
            {
                this.connectionSocket.Disconnect(false);
            }

            Open();

            return 1;
        }

        public int Destroy()
        {
            if (this.connectionSocket != null)
            {
                this.connectionSocket.Dispose();
            }

            return 0;
        }

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
            catch (Error.Invalid)
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

        public abstract AType Read();

        public abstract AType SyncSend(AType message, AType timeout);

        public abstract AType SyncRead(AType timeout);

        protected abstract byte[] ConvertToByte(AType message);

        #endregion
    }
}
