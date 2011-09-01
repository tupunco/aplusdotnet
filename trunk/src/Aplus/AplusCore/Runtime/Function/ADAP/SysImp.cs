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
        #region Constants

        private static readonly int HeaderStartIndex = 4;
        private static readonly int SizeOfCount = 4;
        private static readonly int SizeOfType = 2;
        private static readonly int SizeOfRank = 2;
        private static readonly int SizeOfShape = 4;
        private static readonly int ANullDescriptorLength = 20;
        private static readonly int ASymbolDescriptorLength = 12;

        #endregion

        #region Nested class for argument pass

        /// <summary>
        /// Class to store information about the the data to be imported
        /// </summary>
        class ImportInfo
        {
            private byte[] data;
            private int headerIndex;
            private int dataIndex;

            internal ImportInfo(byte[] data)
            {
                this.data = data;
            }

            internal byte[] Data
            {
                get { return this.data; }
            }

            internal int HeaderIndex
            {
                get { return this.headerIndex; }
                set { this.headerIndex = value; }
            }

            internal int DataIndex
            {
                get { return this.dataIndex; }
                set { this.dataIndex = value; }
            }
        }

        #endregion

        #region Variables

        private static SysImp instance = new SysImp();

        #endregion

        #region Properties

        public static SysImp Instance { get { return instance; } }

        #endregion

        #region Constructors

        private SysImp() { }

        #endregion

        #region Methods

        /// <summary>
        /// Imports an AType from a byte array.
        /// </summary>
        /// <param name="argument">The byte array.</param>
        /// <returns>The AType created from the byte array.</returns>
        public AType Import(byte[] argument)
        {
            if (argument.Length < HeaderStartIndex || argument[0] != CDRConstants.CDRFlag)
            {
                throw new Error.Domain("sys.imp");
            }

            ImportInfo info = new ImportInfo(argument)
            {
                HeaderIndex = HeaderStartIndex
            };

            // the first 4 byte of the header contains the cdr flag, plus the length of the message.
            byte[] headerByteLength = { 0, argument[1], argument[2], argument[3] };
            int headerLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerByteLength, 0));

            info.DataIndex = headerLength;

            return ExtractItems(info);
        }


        /// <summary>
        /// Extract the AType elements from the header and the data.
        /// </summary>
        /// <param name="info">The class containing the informations for importing.</param>
        /// <returns>The AType created from the import informations.</returns>
        private AType ExtractItems(ImportInfo info)
        {
            AType result;

            List<int> shape = new List<int>();

            // The format of the CDR type descriptor
            int count = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(info.Data, info.HeaderIndex));
            info.HeaderIndex += SizeOfCount;
            short typeCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(info.Data, info.HeaderIndex));
            info.HeaderIndex += SizeOfType;
            short rank = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(info.Data, info.HeaderIndex));
            info.HeaderIndex += SizeOfRank;

            for (short i = 0; i < rank; i++)
            {
                shape.Add(IPAddress.NetworkToHostOrder(BitConverter.ToInt32(info.Data, info.HeaderIndex)));
                info.HeaderIndex += SizeOfShape;
            }

            if (typeCode == CDRConstants.CDRBoxShort)
            {
                if (count == 0)
                {
                    // if an item is ANull, the header contains 20 bytes (more in CDR spec)
                    result = Utils.ANull();
                    info.HeaderIndex += ANullDescriptorLength;
                }
                else
                {
                    short nestedType = 
                        IPAddress.NetworkToHostOrder(BitConverter.ToInt16(info.Data, info.HeaderIndex + 4));

                    // `sym is always boxed (see more in CDR spec)
                    if (nestedType == CDRConstants.CDRSymShort)
                    {
                        result = BuildSymbol(shape, info);
                    }
                    else
                    {
                        result = BuildBox(shape, info);
                    }
                }
            }
            else if (typeCode == CDRConstants.CDRSymShort)
            {
                result = BuildSymbol(shape, info);
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
                    length *= sizeof(Char) / 2; // sizeof(Char) == 2 in C#
                    type = ATypes.AChar;
                }
                else
                {
                    throw new Error.Domain("sys.imp");
                }

                byte[] data = info.Data;
                result = ATypeConverter.Instance.BuildArray(shape, ref data, type, info.DataIndex);

                info.DataIndex += length;
            }

            return result;
        }

        /// <summary>
        /// Builds a box/boxarray.
        /// </summary>
        /// <param name="shape">The shape of the AType.</param>
        /// <param name="info">The class containing the informations for importing.</param>
        /// <returns></returns>
        private AType BuildBox(List<int> shape, ImportInfo info)
        {
            AType result;

            if (shape.Count == 0)
            {
                result = ABox.Create(ExtractItems(info));
            }
            else
            {
                List<int> subshape = shape.GetRange(1, shape.Count - 1);
                result = AArray.Create(ATypes.ABox);
                for (int i = 0; i < shape[0]; i++)
                {
                    result.Add(BuildBox(subshape, info));
                }
            }

            return result;
        }

        /// <summary>
        /// Determines the lengths of the ASymbols to build, and then calls BuildSymbolArray to build it.
        /// </summary>
        /// <param name="shape">The shape of the AType.</param>
        /// <param name="info">The class containing the informations for importing.</param>
        /// <returns></returns>
        private AType BuildSymbol(List<int> shape, ImportInfo info)
        {
            int numberOfSymbols = shape.Product();
            int totalLength = 0;
            List<int> symbolLengths = new List<int>();

            for (int i = 0; i < numberOfSymbols; i++)
            {
                // because of the structure of the CDR type descriptor
                int actualLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(info.Data, info.HeaderIndex));

                info.HeaderIndex += ASymbolDescriptorLength;
                totalLength += actualLength;
                symbolLengths.Add(actualLength);
            }

            AType result = BuildSymbolArray(shape, symbolLengths, info);
            return result;
        }

        /// <summary>
        /// Builds ASymbol.
        /// </summary>
        /// <param name="shape">The shape of the AType.</param>
        /// <param name="symbolLengths">The lengths of the ASymbols contained by the AType.</param>
        /// <param name="info">The class containing the informations for importing.</param>
        /// <returns></returns>
        private AType BuildSymbolArray(List<int> shape, IEnumerable<int> symbolLengths, ImportInfo info)
        {
            AType result = Utils.ANull();

            if (shape.Count == 0)
            {
                StringBuilder toSymbol = new StringBuilder();
                int length = symbolLengths.First();

                for (int i = 0; i < length; i++)
                {
                    toSymbol.Append((char)info.Data[info.DataIndex]);
                    info.DataIndex++;
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

                    result.Add(BuildSymbolArray(nextShape, nextSymbolLengths, info));
                    // advance the bytestream further.
                }
            }

            return result;
        }

        #endregion
    }
}
