using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    class SysExp
    {
        #region Constants

        // FIX #4: Move these constants to a separate class.

        static readonly byte CDRFlag = 0x82; // by default 80, +2 for little endian (more: impexp.c 774)
        static readonly byte[] CDRInt = { 0x49, 0x04 }; // I4
        static readonly byte[] CDRChar = { 0x43, 0x01 }; // C1
        static readonly byte[] CDRFloat = { 0x45, 0x08 }; // E8
        static readonly byte[] CDRBox = { 0x47, 0x00 }; // G0
        static readonly byte[] CDRSym = { 0x53, 0x01 }; // S1

        #endregion

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
            CDRMagic[0] = CDRFlag;

            result.AddRange(CDRMagic);
            result.AddRange(header);
            result.AddRange(FormatData(argument));

            return result.ToArray();
        }

        private List<byte> FormatHeader(AType argument)
        {
            List<byte> result = new List<byte>();

            byte[] flag;
            int length = 1;

            if (argument.Shape.Count != 0)
            {
                // FIX #1 (see ATypeConverter.cs)
                length = argument.Shape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor);
            }

            int networkOrderLength = IPAddress.HostToNetworkOrder(length);
            short networkOrderRank = IPAddress.HostToNetworkOrder((short)argument.Rank);
            IEnumerable<int> networkOrderShape = argument.Shape.Select(item => IPAddress.HostToNetworkOrder(item));

            switch (argument.Type)
            {
                case ATypes.ABox:
                    flag = CDRBox;
                    break;
                case ATypes.AChar:
                    flag = CDRChar;
                    break;
                case ATypes.AFloat:
                    flag = CDRFloat;
                    break;
                case ATypes.AInteger:
                    flag = CDRInt;
                    break;
                case ATypes.ASymbol:
                    flag = CDRBox; // symbols must be boxed
                    break;
                case ATypes.ANull:
                    flag = CDRBox;
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
                    List<byte> nullRepresentation = new List<byte>();

                    nullRepresentation.AddRange(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(1)));
                    nullRepresentation.AddRange(CDRBox);
                    nullRepresentation.AddRange(BitConverter.GetBytes(IPAddress.NetworkToHostOrder((short)0)));
                    nullRepresentation.AddRange(BitConverter.GetBytes(0));
                    nullRepresentation.AddRange(CDRInt);
                    nullRepresentation.AddRange(BitConverter.GetBytes(IPAddress.NetworkToHostOrder((short)1)));
                    nullRepresentation.AddRange(BitConverter.GetBytes(0));

                    result.AddRange(nullRepresentation);
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
                byte[] flag = CDRSym;
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
