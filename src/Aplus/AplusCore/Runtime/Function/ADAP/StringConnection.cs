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

        public StringConnection(
            AipcService aipcService, 
            ConnectionAttribute attribute,
            AipcAttribute aipcAttributes,
            Socket socket)
            : base(aipcService, attribute, aipcAttributes, socket)
        { }

        public StringConnection(AipcService aipcService, ConnectionAttribute attribute)
            : base(aipcService, attribute, null, null)
        { }

        #endregion

        #region Methods

        protected int ReadMessageLength()
        {
            byte[] lengthBytes = new byte[4];
            int receivedLength = 0;

            receivedLength = connectionSocket.Receive(lengthBytes);

            if (receivedLength == 0)
            {
                throw new SocketException((int)SocketError.ConnectionAborted);
            }

            if (receivedLength != 4)
            {
                throw new ADAPException(ADAPExceptionType.Import);
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

        protected override AType ConvertToAObject(byte[] messageByte)
        {
            AType message = AArray.Create(ATypes.AChar);

            foreach (byte b in messageByte)
            {
                try
                {
                    message.Add(AChar.Create((char)b));
                }
                catch (InvalidCastException)
                {
                    throw new ADAPException(ADAPExceptionType.Import);
                }
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
            catch (Error.Domain)
            {
                throw new ADAPException(ADAPExceptionType.Import);
            }

            return message;
        }

        #endregion

        #region AipcConnection Members

        public override AType Read()
        {
            AType message;

            if (this.AipcAttributes.ReadPause)
            {
                message = Utils.ANull();
            }
            else
            {
                message = DoRead();
                this.MakeCallback("read", message);
            }

            return message;
        }

        public override AType SyncSend(AType message, AType timeout)
        {
            AType result = null;
            int time = timeout.asInteger * 1000;
            int timeForAByte = time / this.WriteBufferContentSize;
            int messageSent = 0;
            bool errorOccured = false;
            bool previousWritePauseState = this.AipcAttributes.WritePause;

            this.AipcAttributes.WritePause = true;
            this.writeBuffer.AddLast(this.ConvertToByte(message));

            while (writeBuffer.Count > 0)
            {
                byte[] item = writeBuffer.First.Value;
                int sentBytes = 0;

                connectionSocket.SendTimeout = item.Length * timeForAByte;

                try
                {
                    sentBytes = connectionSocket.Send(item);
                }
                catch (SocketException e)
                {
                    result = SyncFillError(e.SocketErrorCode, false);
                    errorOccured = true;
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
            this.AipcAttributes.WritePause = previousWritePauseState;
            
            if (!errorOccured)
            {
                result = SyncFillOk(AInteger.Create(messageSent), false);
            }

            return result;
        }

        public override AType SyncRead(AType timeout)
        {
            
            byte[] messageByte;
            bool previousReadPauseState = this.AipcAttributes.ReadPause;
            this.AipcAttributes.ReadPause = true;

            try
            {
                this.connectionSocket.ReceiveTimeout = timeout.asInteger * 1000;
                int length = ReadMessageLength();
                messageByte = ReadMessageData(length);
            }
            catch (SocketException e)
            {
                return SyncFillError(e.SocketErrorCode, true);
            }
            finally
            {
                this.connectionSocket.ReceiveTimeout = 0;
            }

            AType message;
            try
            {
                message = ConvertToAObject(messageByte);
            }
            catch (ADAPException)
            {
                this.Reset();
                this.MakeCallback("reset", ASymbol.Create("readImport"));
                return SyncFillError(SocketError.ConnectionReset, true);
            }

            this.AipcAttributes.ReadPause = previousReadPauseState;

            return SyncFillOk(message, true);
        }

        protected override byte[] ConvertToByte(AType message)
        {
            if (message.Type != ATypes.AChar)
            {
                throw new ADAPException(ADAPExceptionType.Export);
            }

            byte[] messageBody = ASCIIEncoder.GetBytes(message.ToString());
            byte[] messageHeader = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(messageBody.Length));
            List<byte> result = new List<byte>();
            
            result.AddRange(messageHeader);
            result.AddRange(messageBody);

            return result.ToArray();
        }

        #endregion
    }
}
