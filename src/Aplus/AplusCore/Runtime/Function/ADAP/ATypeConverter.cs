using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    class ATypeConverter
    {
        #region Variables

        private static ATypeConverter instance = new ATypeConverter();

        #endregion

        #region Properties

        public static ATypeConverter Instance { get { return instance; } }

        #endregion

        #region Constructors

        private ATypeConverter()
        {
        }

        #endregion

        #region Methods

        private delegate AType ItemConstructDelegate(ref byte[] data, int index);

        private AType ConstructAInteger(ref byte[] data, int index)
        {
            return AInteger.Create(BitConverter.ToInt32(data, index));
        }

        private AType ConstructAChar(ref byte[] data, int index)
        {
            return AChar.Create((char)data[index]);
        }

        private AType ConstructAFloat(ref byte[] data, int index)
        {
            return AFloat.Create(BitConverter.ToDouble(data, index));
        }

        public AType BuildArray(List<int> shape, ref byte[] data, ATypes type, int index)
        {
            AType result = Utils.ANull();
            int typeSize;
            ItemConstructDelegate itemConstruct;

            switch (type)
            {
                case ATypes.AInteger:
                    typeSize = sizeof(Int32);
                    itemConstruct = ConstructAInteger;
                    break;
                case ATypes.AChar:
                    typeSize = sizeof(Char) / 2; // FIXMEEE!!!!! sizeof(Char) == 2 in C#!!!!!
                    itemConstruct = ConstructAChar;
                    break;
                case ATypes.AFloat:
                    typeSize = sizeof(Double);
                    itemConstruct = ConstructAFloat;
                    break;
                default:
                    throw new ADAPException(ADAPExceptionType.Import);
            }

            if (data.Length < (typeSize * shape.Product() + index))
            {
                throw new ADAPException(ADAPExceptionType.Import);
            }

            if (shape.Count == 0)
            {
                result = itemConstruct(ref data, index);
            }
            else if (shape.Count == 1)
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    result.Add(itemConstruct(ref data, index));
                    index += typeSize;
                }
            }
            else
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    List<int> nextShape = shape.GetRange(1, shape.Count - 1);
                    int subDimensionLength = nextShape.Product() * typeSize;

                    result.Add(BuildArray(nextShape, ref data, type, index));
                    index += subDimensionLength;
                }
            }

            return result;
        }

        #endregion
    }
}
