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

        private delegate AType ItemConstructDelegate(IEnumerable<byte> data);

        private AType ConstructAInteger(IEnumerable<byte> data)
        {
            return AInteger.Create(BitConverter.ToInt32(data.ToArray(), 0));
        }

        private AType ConstructAChar(IEnumerable<byte> data)
        {
            return AChar.Create((char)data.ToArray<byte>()[0]);
        }

        private AType ConstructAFloat(IEnumerable<byte> data)
        {
            return AFloat.Create(BitConverter.ToDouble(data.ToArray(), 0));
        }

        public AType BuildArray(List<int> shape, IEnumerable<byte> data, ATypes type)
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
                    throw new Error.Type("");
            }

            if (shape.Count <= 1)
            {
                for (int i = 0; i < data.Count(); i += typeSize)
                {
                    result.Add(itemConstruct(data.Skip(i).Take(typeSize)));
                }
            }
            else
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    List<int> nextShape = shape.GetRange(1, shape.Count - 1);
                    int subDimensionLength = nextShape.Product() * typeSize;
                    List<byte> nextData = new List<byte>();

                    nextData.AddRange(data.Skip(i * subDimensionLength).Take(subDimensionLength));
                    result.Add(BuildArray(nextShape, nextData, type));
                }
            }

            return result;
        }

        #endregion
    }
}
