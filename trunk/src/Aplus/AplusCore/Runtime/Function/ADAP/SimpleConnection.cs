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

        // The messageByte in ConvertToAObject doesn't contains the length of the message!!!
        static readonly int rankIndex = 8;
        static readonly int shapeIndex = 16;
        static readonly int typeIndex = 4;
        static readonly int dataIndex = 56;

        #endregion
        
        #region Constructors

        public SimpleConnection(ConnectionAttribute attribute, AipcAttributes aipcAttributes = null, Socket socket = null)
            : base(attribute, aipcAttributes, socket)
        {
        }

        #endregion

        #region StringConnection Members

        protected override AType ConvertToAObject(byte[] message)
        {
            // FIX ##? Check the length of the message

            AType result;
            List<int> shape = new List<int>();
            int rank = BitConverter.ToInt32(message, rankIndex);
            int index = shapeIndex;
            ATypes type;

            for (int i = 0; i < rank; i++)
            {
                shape.Add(BitConverter.ToInt32(message, index));
                index += 4;
            }

            // FIX ##?: ArrayCopy?
            List<byte> messageAsList = new List<byte>(message);
            List<byte> data = messageAsList.GetRange(dataIndex, messageAsList.Count - dataIndex);

            switch (BitConverter.ToInt32(message, typeIndex))
            {
                case 0:
                    type = ATypes.AInteger;
                    break;
                case 1:
                    type = ATypes.AFloat;
                    break;
                case 2:
                    type = ATypes.AChar;
                    break;
                default:
                    throw new Error.Invalid("readImport");
            }

            try
            {
                result = ATypeConverter.Instance.BuildArray(shape, data, type);
            }
            catch (Exception)
            {
                // FIX ##? Do we really need to catch all of the exceptions?
                throw new Error.Invalid("readImport");
            }
            return result;
        }

        #endregion

        #region AipcConnection Members

        protected override byte[] ConvertToByte(AType message)
        {
            List<byte> byteMessage = new List<byte>();
            List<byte> byteHeader = new List<byte>();
            List<byte> byteBody = new List<byte>();

            byteHeader.AddRange(BitConverter.GetBytes((message.Rank > 1) ? 2 : 3));

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
