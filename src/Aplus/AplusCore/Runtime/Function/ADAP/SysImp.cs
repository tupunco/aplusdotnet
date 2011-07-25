using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    class SysImp
    {
        #region Constants

        // FIX #4: Move these constants to a separate class.

        private const byte CDRFlag = 0x82; // by default 80, +2 for little endian (more: impexp.c 774)

        private static readonly List<short> integerTypes = new List<short>(){
                                                             0x4201, // B1
                                                             0x4204, // B4
                                                             0x4208, // B8
                                                             0x4902, // I2
                                                             0x4904, // I4
                                                            };

        private static readonly List<short> floatTypes = new List<short>(){ 
                                                                   0x4504, // E4
                                                                   0x4508, // E8
                                                                  };

        private const short CDRCharShort = (0x4301); // C1
        private const short CDRBoxShort = 0x4700; // G0
        private const short CDRSymShort = 0x5301; // S1

        #endregion

        #region Variables

        // FIX #5: This class is a singleton, should't be these two initialised in the Import method? 
        // FIX #6: Do we need these two at all?
        private int headerIndex = 4;
        private int dataIndex = 0;

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
            int messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(argument, 0));

            byte[] headerByteLength = { 0, argument[5], argument[6], argument[7] };
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

            short type = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(argument, headerIndex));
            headerIndex += 2;
            short rank = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(argument, headerIndex));
            headerIndex += 2;

            List<int> shape = new List<int>();
            int length = 0;

            for (short i = 0; i < rank; i++)
            {
                shape.Add(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(argument, headerIndex)));
                headerIndex += 4;
            }

            if (type == CDRBoxShort)
            {
                AType items = Utils.ANull();

                if (count == 0)
                {
                    items = Utils.ANull();
                    headerIndex += 20; // if an item is ANull, the header contains 20 bytes (more in CDR spec)
                }
                else
                {
                    short nestedType = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(argument, headerIndex + 4));

                    if (nestedType == CDRSymShort)
                    {
                        items = HelperFunction(shape, argument);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            items.Add(ABox.Create(GetItems(argument)));
                        }
                    }
                }

                result = items;
            }
            else if (integerTypes.Contains(type))
            {
                // FIX #1 (see ATypeConverter.cs)
                // FIX #2
                if (shape.Count == 0)
                {
                    length = sizeof(System.Int32);
                }
                else
                {
                    length = shape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor) * sizeof(System.Int32);
                }

                result = ATypeConverter.Instance.BuildIntegerArray(shape, argument.Skip(dataIndex).Take(length));
                dataIndex += length;
            }
            else if (floatTypes.Contains(type))
            {
                // FIX #1 (see ATypeConverter.cs)
                // FIX #2
                if (shape.Count == 0)
                {
                    length = sizeof(System.Double);
                }
                else
                {
                    length = shape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor) * sizeof(System.Double);
                }

                result = ATypeConverter.Instance.BuildFloatArray(shape, argument.Skip(dataIndex).Take(length));
                dataIndex += length;
            }
            else if (type == CDRCharShort)
            {
                // FIX #1 (see ATypeConverter.cs)
                // FIX #2
                if (shape.Count == 0)
                {
                    length = sizeof(System.Int32);
                }
                else
                {
                    length = shape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor) * sizeof(System.Char);
                }

                result = ATypeConverter.Instance.BuildCharArray(shape, argument.Skip(dataIndex).Take(length));
                dataIndex += length;
            }
            else if (type == CDRSymShort)
            {
                result = HelperFunction(shape, argument);
            }

            return result;

        }

        // FIX #7: Helper for what?
        private AType HelperFunction(List<int> shape, byte[] argument)
        {
            int numberOfSymbols = 1;

            if (shape.Count > 1)
            {
                // FIX #1 (see ATypeConverter.cs)
                numberOfSymbols = shape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor);
            }

            int totalLength = 0;
            List<int> symbolLengths = new List<int>();

            for (int i = 0; i < numberOfSymbols; i++)
            {
                int actualLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(argument, headerIndex + i * 12)); // because of the structure of the CDR type descriptor
                totalLength += actualLength;
                symbolLengths.Add(actualLength);
            }


            AType result = BuildSymArray(shape, symbolLengths, argument.Skip(dataIndex).Take(totalLength));
            dataIndex += totalLength;
            return result;
        }

        private AType BuildSymArray(List<int> shape, IEnumerable<int> symbolLengths, IEnumerable<byte> data)
        {
            AType result = Utils.ANull();

            if (shape.Count == 0)
            {
                // FIX #6: use StringBuilder
                string toSymbol = "";

                foreach (byte b in data)
                {
                    toSymbol += (char)b;
                }

                result = ASymbol.Create(toSymbol);
            }
            else
            {
                if (shape.Count == 1)
                {
                    IEnumerable<byte> dataClone = data;

                    for (int i = 0; i < shape[0]; i++)
                    {
                        result.Add(BuildSymArray(new List<int>(), new List<int>(), dataClone.Take(symbolLengths.ElementAt(i))));
                        dataClone = dataClone.Take(symbolLengths.ElementAt(i));
                    }

                }
                else
                {
                    IEnumerable<byte> dataClone = data;
                    for (int i = 0; i < shape[0]; i++)
                    {
                        List<int> nextShape = shape.GetRange(1, shape.Count - 1);
                        int subDimensionLength = nextShape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor);
                        List<byte> nextData = new List<byte>();
                        IEnumerable<int> nextSymbolLengths = symbolLengths.Skip(i * subDimensionLength).Take(subDimensionLength);
                        int dataLength = nextSymbolLengths.Sum(n => n);
                        result.Add(BuildSymArray(nextShape, nextSymbolLengths, dataClone.Take(dataLength)));
                        dataClone = dataClone.Skip(dataLength);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
