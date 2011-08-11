using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    class StringConnection : AipcConnection
    {
        #region Constructors

        public StringConnection(ConnectionAttribute attribute, AipcAttributes aipcAttributes, Socket socket = null)
            : base(attribute, aipcAttributes, socket)
        { }

        #endregion

        #region Methods

        protected int ReadMessageLength()
        {
            byte[] lengthBytes = new byte[4];
            int receivedLength = 0;

            receivedLength = connectionSocket.Receive(lengthBytes);

            if (receivedLength != 4)
            {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }

            int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBytes, 0));
            return length;
        }

        protected byte[] ReadMessageData(int length)
        {
            byte[] readByte = new byte[length];
            byte[] messageByte = new byte[length];
            int readLength = 0;

            while (length != readLength)
            {
                int actualReadLength = connectionSocket.Receive(readByte, 0, length - readLength, SocketFlags.None);

                Array.Copy(readByte, 0, messageByte, readLength, actualReadLength);
                readLength += actualReadLength;
            }

            return messageByte;
        }

        protected virtual AType ConvertToAObject(byte[] messageByte)
        {
            AType message = AArray.Create(ATypes.AChar);

            foreach (byte b in messageByte)
            {
                message.Add(AChar.Create((char)b));
            }

            return message;
        }

        protected AType DoRead()
        {
            AType message;
            int length = ReadMessageLength();
            byte[] messageByte = ReadMessageData(length);
            
            try
            {
                message = ConvertToAObject(messageByte);
            }
            catch (Error)
            {
                throw new Error.Invalid("readImport");
            }

            return message;
        }

        #endregion

        #region AipcConnection Members

        public override AType Read()
        {
            AType message = Utils.ANull();

            if (!this.isOpen)
            {
                // logically unreachable code
                throw new Error.Invalid("");
            }

            if (!this.AipcAttributes.ReadPause)
            {
                message = DoRead();
                Console.WriteLine("Call read callback here with {0},{1}", message, ConnectionAttributes.HandleNumber);
            }

            return message;
        }
        
        public override AType SyncSend(AType message, AType timeout)
        {
            AType result;
            bool prevState = this.AipcAttributes.WritePause;
            this.AipcAttributes.WritePause = true;

            this.writeBuffer.AddLast(this.ConvertToByte(message));
            int time = timeout.asInteger * 1000;
            int timeForAByte = time / this.WriteBufferContentSize;
            int messageSent = 0;

            while (writeBuffer.Count > 0)
            {
                byte[] item = writeBuffer.First.Value;

                connectionSocket.SendTimeout = item.Length * timeForAByte;
                int sentBytes = 0;
                try
                {
                    sentBytes = connectionSocket.Send(item);
                }
                catch (SocketException e)
                {
                    result = SyncFillError(e);
                }

                writeBuffer.RemoveFirst();

                if (sentBytes < item.Length)
                {
                    byte[] newMessage = new byte[item.Length - sentBytes];
                    writeBuffer.AddFirst(newMessage);
                    break;
                }

                messageSent++;
            }

            connectionSocket.SendTimeout = 0;
            this.AipcAttributes.WritePause = prevState;

            result = SyncFillOk(AInteger.Create(messageSent));
            return result;
        }

        public override AType SyncRead(AType timeout)
        {
            AType message = Utils.ANull();
            bool prevState = this.AipcAttributes.ReadPause;
            this.AipcAttributes.ReadPause = true;

            byte[] messageByte;

            try
            {
                this.connectionSocket.ReceiveTimeout = timeout.asInteger * 1000;
                int length = ReadMessageLength();
                messageByte = ReadMessageData(length);
            }
            catch (SocketException e)
            {
                return SyncFillError(e);
            }
            finally
            {
                this.connectionSocket.ReceiveTimeout = 0;
            }

            try
            {
                message = ConvertToAObject(messageByte);
            }
            catch (Error.Invalid)
            {
                Console.WriteLine("Call error callback here readimport");
            }

            this.AipcAttributes.ReadPause = prevState;

            return AArray.Create(ATypes.ABox,
                                 ABox.Create(ASymbol.Create("OK")),
                                 ABox.Create(message),
                                 ABox.Create(Utils.ANull())
                                 );
        }

        protected override byte[] ConvertToByte(AType message)
        {
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            List<byte> byteMessage = new List<byte>();
            byte[] byteBody = encoder.GetBytes(message.ToString());

            byteMessage.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(byteBody.Length)));
            byteMessage.AddRange(encoder.GetBytes(message.ToString()));

            return byteMessage.ToArray();
        }

        #endregion
    }
}
