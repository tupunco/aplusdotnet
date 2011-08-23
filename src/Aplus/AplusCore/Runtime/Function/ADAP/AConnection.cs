using System;
using System.Net;
using System.Net.Sockets;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    class AConnection : StringConnection
    {
        #region Constructors

        public AConnection(
            AipcService aipcService,
            ConnectionAttribute attribute,
            AipcAttributes aipcAttributes,
            Socket socket)
            : base(aipcService, attribute, aipcAttributes, socket)
        { }

        public AConnection(AipcService aipcService, ConnectionAttribute attribute)
            : base(aipcService, attribute, null, null)
        { }

        #endregion

        #region StringConnection Members

        protected override AType ConvertToAObject(byte[] messageByte)
        {
            return SysImp.Instance.Import(messageByte);
        }

        #endregion

        #region AipcConnection Members

        public override AType Read()
        {
            AType message;

            if (this.AipcAttributes.ReadPause)
            {
                return Utils.ANull();
            }

            if (AipcAttributes.BurstMode)
            {
                message = AArray.Create(ATypes.ABox);
                while (connectionSocket.Poll(1, SelectMode.SelectRead))
                {
                    message.Add(ABox.Create(DoRead()));
                }
            }
            else
            {
                message = DoRead();
            }

            this.MakeCallback("read", message);

            return message;
        }

        protected override byte[] ConvertToByte(AType message)
        {
            try
            {
                byte[] buffer = SysExp.Instance.Format(message);
                byte[] result = new byte[4 + buffer.Length];

                Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(buffer.Length)), result, 4);
                Array.Copy(buffer, 0, result, 4, buffer.Length);

                return result;
            }
            catch (Exception)
            {
                throw new ADAPException(ADAPExceptionType.Import);
            }
        }

        #endregion
    }
}
