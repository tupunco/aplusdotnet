using System;
using System.Net.Sockets;

using AplusCore.Runtime;
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

                for (int i = 0; i < readLength; i++)
                {
                    message.Add(AChar.Create((char)buffer[i]));
                }

                Console.WriteLine("Call read callback here with {0},{1}", message, ConnectionAttributes.HandleNumber);
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
            try
            {
                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                byte[] result = encoder.GetBytes(message.ToString());
                return result;
            }
            catch (Exception)
            {
                throw new Error.Invalid("readImport");
            }
        }

        #endregion
    }
}
