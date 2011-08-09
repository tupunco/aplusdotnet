using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    class SimpleConnection : StringConnection
    {
        #region Constants

        static readonly int rankIndex = 12;
        static readonly int shapeIndex = 20;
        static readonly int typeIndex = 8;
        static readonly int dataIndex = 60;

        #endregion
        
        #region Constructors

        public SimpleConnection(ConnectionAttribute attribute, AipcAttributes aipcAttributes = null, Socket socket = null)
            : base(attribute, aipcAttributes, socket)
        {
        }

        #endregion

        #region StringConnection Members

        protected override AType ConvertToAObject(byte[] messageByte)
        {
            AType result = Utils.ANull();

            int index = rankIndex;
            int rank;
            List<int> shape = new List<int>();

            rank = BitConverter.ToInt32(messageByte, index);

            index = shapeIndex;

            for (int i = 0; i < rank; i++)
            {
                shape.Add(BitConverter.ToInt32(messageByte, index));
                index += 4;
            }

            List<byte> messageAsList = new List<byte>(messageByte);
            List<byte> data = messageAsList.GetRange(dataIndex, messageAsList.Count - dataIndex);

            index = typeIndex;

            switch (BitConverter.ToInt32(messageByte, index))
            {
                case 0:
                    result = ATypeConverter.Instance.BuildArray(shape, data, ATypes.AInteger);
                    break;
                case 1:
                    result = ATypeConverter.Instance.BuildArray(shape, data, ATypes.AFloat);
                    break;
                case 2:
                    result = ATypeConverter.Instance.BuildArray(shape, data, ATypes.AChar);
                    break;
                default:
                    throw new Error.Invalid("readImport");
            }

            return result;
        }

        #endregion

        #region AipcConnection Members

        protected override byte[] ConvertToByte(AType message)
        {
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            List<byte> byteMessage = new List<byte>();
            List<byte> byteHeader = new List<byte>();
            List<byte> byteBody = new List<byte>();

            if (message.Rank > 1)
            {
                byteHeader.AddRange(BitConverter.GetBytes(3));
            }
            else
            {
                byteHeader.AddRange(BitConverter.GetBytes(2));
            }

            switch (message.Type)
            {
                case ATypes.AInteger:
                    byteHeader.AddRange(BitConverter.GetBytes(0));
                    break;
                case ATypes.AFloat:
                    byteHeader.AddRange(BitConverter.GetBytes(1));
                    break;
                case ATypes.AChar:
                    byteHeader.AddRange(BitConverter.GetBytes(2));
                    break;
                default:
                    throw new Error.Type("Not convertable type");
            }

            byteHeader.AddRange(BitConverter.GetBytes(message.Rank));
            byteHeader.AddRange(BitConverter.GetBytes(message.Shape.Product()));
            
            foreach (int item in message.Shape)
            {
                byteHeader.AddRange(BitConverter.GetBytes(item));
            }

            int fill = 16307;
            byte[] fillByte = BitConverter.GetBytes(fill); // fillerbyte to fill empty places

            for (int i = 0; i < 9 - message.Rank; i++)
            {
                byteHeader.AddRange(fillByte);
            }

            byteHeader.AddRange(fillByte);

            byteBody.AddRange(GetItem(message));

            byteMessage.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(byteBody.Count + byteHeader.Count)));
            byteMessage.AddRange(byteHeader);
            byteMessage.AddRange(byteBody);

            if (message.Type == ATypes.AChar)
            {
                byteMessage.Add((byte)0);
            }

            return byteMessage.ToArray();
        }

        #endregion

        #region Utility

        private List<byte> GetItem(AType message)
        {
            List<byte> result = new List<byte>();

            if (message.IsArray)
            {
                foreach (AType item in message)
                {
                    result.AddRange(GetItem(item));
                }
            }
            else
            {
                switch (message.Type)
                {
                    case ATypes.AChar:
                        result.Add((byte)message.asChar);
                        break;
                    case ATypes.AInteger:
                        result.AddRange(BitConverter.GetBytes(message.asInteger));
                        break;
                    case ATypes.AFloat:
                        result.AddRange(BitConverter.GetBytes(message.asFloat));
                        break;
                }
            }

            return result;
        }

        #endregion
    }
}
