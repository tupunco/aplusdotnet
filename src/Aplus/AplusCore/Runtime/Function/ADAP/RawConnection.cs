using System;
using System.Net.Sockets;
using System.Text;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    class RawConnection : AipcConnection
    {
        #region Constructors

        public RawConnection(ConnectionAttribute attribute, AipcAttributes aipcAttributes = null, Socket socket = null)
            : base(attribute, aipcAttributes, socket)
        { }

        #endregion

        #region AipcConnection Members

        protected override AType ConvertToAObject(byte[] message)
        {
            AType result = Utils.ANull();

            for (int i = 0; i < message.Length; i++)
            {
                try
                {
                    result.Add(AChar.Create((char)message[i]));
                }
                catch (InvalidCastException)
                {
                    throw new ADAPException(ADAPExceptionType.Import);
                }
            }

            return result;
        }
        
        public override AType Read()
        {
            AType message = Utils.ANull();

            if (!AipcAttributes.ReadPause)
            {
                int readLength = 0;
                byte[] buffer = new byte[1024];

                readLength = connectionSocket.Receive(buffer);

                if (readLength == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                
                byte[] messageByte = new byte[readLength];
                Array.Copy(buffer, messageByte, readLength);

                message = ConvertToAObject(messageByte);

                this.MakeCallback("read", message);
            }

            return message;
        }

        public override AType SyncSend(AType message, AType timeout)
        {
            throw new Error.Invalid("This protocol does not implements SyncSend");
        }

        public override AType SyncRead(AType timeout)
        {
            throw new Error.Invalid("This protocol does not implements SyncRead");
        }

        protected override byte[] ConvertToByte(AType message)
        {
            if (message.Type != ATypes.AChar)
            {
                throw new ADAPException(ADAPExceptionType.Export);
            }

            byte[] result;
            try
            {
                result = ASCIIEncoder.GetBytes(message.ToString());
            }
            catch (ArgumentNullException)
            {
                throw new ADAPException(ADAPExceptionType.Export);
            }
            catch (EncoderFallbackException)
            {
                throw new ADAPException(ADAPExceptionType.Export);
            }

            return result;
        }

        #endregion
    }
}
