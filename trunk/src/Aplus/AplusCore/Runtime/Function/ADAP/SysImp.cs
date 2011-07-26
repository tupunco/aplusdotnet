using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    public class SysImp
    {
        #region Variables

        // FIX #6: Do we need these two at all?
        private int headerIndex;
        private int dataIndex;

        private static SysImp instance = new SysImp();

        #endregion

        #region Constructors

        private SysImp() { }

        #endregion

        #region Properties

        public static SysImp Instance { get { return instance; } }

        #endregion

        #region Methods

        public AType Import(byte[] argument)
        {
            headerIndex = 4;
            dataIndex = 0;
            int messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(argument, 0));

            byte[] headerByteLength = { 0, argument[1], argument[2], argument[3] };
            int headerLength = BitConverter.ToInt32(headerByteLength, 0);
            headerLength = IPAddress.NetworkToHostOrder(headerLength);

            dataIndex = headerLength;

            return GetItems(argument);
        }

        private AType GetItems(byte[] argument)
        {
            AType result = Utils.ANull();

            int count = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(argument, headerIndex));
            headerIndex += 4;

            short typeCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(argument, headerIndex));
            headerIndex += 2;
            short rank = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(argument, headerIndex));
            headerIndex += 2;

            List<int> shape = new List<int>();

            for (short i = 0; i < rank; i++)
            {
                shape.Add(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(argument, headerIndex)));
                headerIndex += 4;
            }

            if (typeCode == CDRConstants.CDRBoxShort)
            {
                if (count == 0)
                {
                    // if an item is ANull, the header contains 20 bytes (more in CDR spec)
                    headerIndex += 20; 
                }
                else
                {
                    short nestedType = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(argument, headerIndex + 4));

                    if (nestedType == CDRConstants.CDRSymShort)
                    {
                        result = BuildSymbol(shape, argument);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            result.Add(ABox.Create(GetItems(argument)));
                        }
                    }
                }
            }
            else if (typeCode == CDRConstants.CDRSymShort)
            {
                result = BuildSymbol(shape, argument);
            }
            else
            {
                int length = shape.Product();
                ATypes type;

                if (CDRConstants.IntegerTypes.Contains(typeCode))
                {
                    length *= sizeof(Int32);
                    type = ATypes.AInteger;
                }
                else if (CDRConstants.FloatTypes.Contains(typeCode))
                {
                    length *= sizeof(Double);
                    type = ATypes.AFloat;
                }
                else if (typeCode == CDRConstants.CDRCharShort)
                {
                    length *= sizeof(Char);
                    type = ATypes.AChar;
                }
                else
                {
                    throw new NotSupportedException("Should never reach this!");
                }

                result = ATypeConverter.Instance.BuildArray(shape, argument.Skip(dataIndex).Take(length), type);
                dataIndex += length;
            }

            return result;
        }

        private AType BuildSymbol(List<int> shape, byte[] argument)
        {
            int numberOfSymbols = shape.Product();
            int totalLength = 0;
            List<int> symbolLengths = new List<int>();

            for (int i = 0; i < numberOfSymbols; i++)
            {
                // because of the structure of the CDR type descriptor
                int actualLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(argument, headerIndex + i * 12));

                totalLength += actualLength;
                symbolLengths.Add(actualLength);
            }

            AType result = BuildSymbolArray(shape, symbolLengths, argument.Skip(dataIndex).Take(totalLength));
            dataIndex += totalLength;
            return result;
        }

        private AType BuildSymbolArray(List<int> shape, IEnumerable<int> symbolLengths, IEnumerable<byte> data)
        {
            AType result = Utils.ANull();

            if (shape.Count == 0)
            {
                StringBuilder toSymbol = new StringBuilder();

                foreach (byte b in data)
                {
                    toSymbol.Append((char)b);
                }

                result = ASymbol.Create(toSymbol.ToString());
            }
            else
            {
                List<int> nextShape = (shape.Count > 1) ? shape.GetRange(1, shape.Count - 1) : new List<int>();
                int subDimensionLength = nextShape.Product();

                for (int i = 0; i < shape[0]; i++)
                {
                    IEnumerable<int> nextSymbolLengths = symbolLengths.Skip(i * subDimensionLength).Take(subDimensionLength);
                    int dataLength = nextSymbolLengths.Sum();

                    result.Add(BuildSymbolArray(nextShape, nextSymbolLengths, data.Take(dataLength)));
                    // advance the bytestream further.
                    data = data.Skip(dataLength);
                }
            }

            return result;
        }

        #endregion
    }
}
