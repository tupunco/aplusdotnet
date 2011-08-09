using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    public class SysExp
    {
        #region Variables

        private static SysExp instance = new SysExp();

        #endregion

        #region Properties

        public static SysExp Instance { get { return instance; } }

        #endregion

        #region Constructors

        private SysExp()
        {
        }

        #endregion

        #region Methods

        public byte[] Format(AType argument)
        {
            List<byte> result = new List<byte>();
            List<byte> header = FormatHeader(argument);

            int headerLength = header.Count;
            headerLength += 4;

            int networkOrderHeaderLength = IPAddress.HostToNetworkOrder(headerLength);

            byte[] CDRMagic = BitConverter.GetBytes(networkOrderHeaderLength);
            CDRMagic[0] = CDRConstants.CDRFlag;

            result.AddRange(CDRMagic);
            result.AddRange(header);
            result.AddRange(FormatData(argument));

            return result.ToArray();
        }

        private List<byte> FormatHeader(AType argument)
        {
            List<byte> result = new List<byte>();

            byte[] flag;
            int length = argument.Shape.Product();

            int networkOrderLength = IPAddress.HostToNetworkOrder(length);
            short networkOrderRank = IPAddress.HostToNetworkOrder((short)argument.Rank);
            IEnumerable<int> networkOrderShape = argument.Shape.Select(item => IPAddress.HostToNetworkOrder(item));

            switch (argument.Type)
            {
                case ATypes.ABox:
                    flag = CDRConstants.CDRBox;
                    break;
                case ATypes.AChar:
                    flag = CDRConstants.CDRChar;
                    break;
                case ATypes.AFloat:
                    flag = CDRConstants.CDRFloat;
                    break;
                case ATypes.AInteger:
                    flag = CDRConstants.CDRInt;
                    break;
                case ATypes.ASymbol:
                    flag = CDRConstants.CDRBox; // symbols must be boxed
                    break;
                case ATypes.ANull:
                    flag = CDRConstants.CDRBox;
                    break;
                default:
                    throw new Error.Type("sys.exp");
            }

            result.AddRange(BitConverter.GetBytes(networkOrderLength));
            result.AddRange(flag);
            result.AddRange(BitConverter.GetBytes(networkOrderRank));

            foreach (int item in networkOrderShape)
            {
                result.AddRange(BitConverter.GetBytes(item));
            }

            if (argument.Type == ATypes.ASymbol)
            {
                result.AddRange(FormatSymbolHeader(argument));
            }

            if (argument.IsBox)
            {
                if (argument.IsArray)
                {
                    foreach (AType item in argument)
                    {
                        result.AddRange(FormatHeader(item.NestedItem));
                    }
                }
                else
                {
                    result.AddRange(FormatHeader(argument.NestedItem));
                }
            }

            if (argument.Type == ATypes.ANull)
            {
                if (argument.IsArray && argument.Length != 0)
                {
                    foreach (AType item in argument)
                    {
                        result.AddRange(FormatHeader(item));
                    }
                }
                else
                {
                    result.AddRange(CDRConstants.CDRNull);
                }
            }

            return result;
        }

        private List<byte> FormatSymbolHeader(AType argument)
        {
            List<byte> result = new List<byte>();

            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    result.AddRange(FormatSymbolHeader(item));
                }
            }
            else
            {
                byte[] flag = CDRConstants.CDRSym;
                int length = argument.asString.Length;
                int networkOrderLength = IPAddress.HostToNetworkOrder(length);
                short networkOrderRank = IPAddress.HostToNetworkOrder((short)1);
                IEnumerable<int> networkOrderShape = argument.Shape.Select(item => IPAddress.HostToNetworkOrder(item));

                result.AddRange(BitConverter.GetBytes(networkOrderLength));
                result.AddRange(flag);
                result.AddRange(BitConverter.GetBytes(networkOrderRank));
                result.AddRange(BitConverter.GetBytes(networkOrderLength));
            }

            return result;
        }

        private List<byte> FormatData(AType argument)
        {
            List<byte> result = new List<byte>();

            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    result.AddRange(FormatData(item));
                }
            }
            else if (argument.IsBox)
            {
                return FormatData(argument.NestedItem);
            }
            else
            {
                switch (argument.Type)
                {
                    case ATypes.AChar:
                        foreach (byte b in BitConverter.GetBytes(argument.asChar))
                        {
                            if (b != 0)
                            {
                                result.Add(b);
                            }
                        }
                        break;
                    case ATypes.AFloat:
                        result.AddRange(BitConverter.GetBytes(argument.asFloat));
                        break;
                    case ATypes.AInteger:
                        result.AddRange(BitConverter.GetBytes(argument.asInteger));
                        break;
                    case ATypes.ASymbol:
                        foreach (char character in argument.asString)
                        {
                            result.Add(BitConverter.GetBytes(character)[0]);
                        }
                        break;
                    case ATypes.ANull:
                        break;
                    default:
                        throw new Error.Type("sys.exp");
                }
            }

            return result;
        }

        #endregion
    }
}
