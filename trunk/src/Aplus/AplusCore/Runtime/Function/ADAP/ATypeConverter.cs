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

        // FIX #3: (Proposal) Can we somehow merge these methods?

        public AType BuildIntegerArray(List<int> shape, IEnumerable<byte> data)
        {
            AType result = AArray.Create(ATypes.AInteger);

            if (shape.Count <= 1)
            {
                for (int i = 0; i < data.Count<byte>(); i += 4)
                {
                    result.Add(AInteger.Create((BitConverter.ToInt32(data.ToArray(), i))));
                }
            }
            else
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    List<int> nextShape = shape.GetRange(1, shape.Count - 1);
                    // FIX #1: Create a 'Product' extension method.
                    // FIX #2: Int32 is enough no need for the fully qualified System.Int32 name
                    int subDimensionLength = nextShape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor) * sizeof(System.Int32);
                    List<byte> nextData = new List<byte>();
                    nextData.AddRange(data.Skip(i * subDimensionLength).Take(subDimensionLength));
                    result.Add(BuildIntegerArray(nextShape, nextData));
                }
            }

            return result;
        }

        public AType BuildFloatArray(List<int> shape, IEnumerable<byte> data)
        {
            AType result = AArray.Create(ATypes.AFloat);

            if (shape.Count <= 1)
            {
                for (int i = 0; i < data.Count<byte>(); i += 8)
                {
                    AType element = AFloat.Create(BitConverter.ToDouble(data.ToArray(), i));
                    result.Add(element);
                }
            }
            else
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    List<int> nextShape = shape.GetRange(1, shape.Count - 1);
                    // FIX #1: Create a 'Product' extension method.
                    // FIX #2: Int32 is enough no need for the fully qualified System.Int32 name
                    int subDimensionLength = nextShape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor) * sizeof(System.Double);
                    List<byte> nextData = new List<byte>();
                    nextData.AddRange(data.Skip(i * subDimensionLength).Take(subDimensionLength));
                    result.Add(BuildFloatArray(nextShape, nextData));
                }
            }

            return result;
        }

        public AType BuildCharArray(List<int> shape, IEnumerable<byte> data)
        {
            AType result = AArray.Create(ATypes.AChar);

            if (shape.Count <= 1)
            {
                for (int i = 0; i < data.Count<byte>(); i++)
                {
                    AType element = AChar.Create((char)data.ElementAt(i));
                    result.Add(element);
                }
            }
            else
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    List<int> nextShape = shape.GetRange(1, shape.Count - 1);
                    // FIX #1: Create a 'Product' extension method.
                    // FIX #2: Int32 is enough no need for the fully qualified System.Int32 name
                    int subDimensionLength = nextShape.Aggregate((actualProduct, nextFactor) => actualProduct * nextFactor) * sizeof(System.Char);
                    List<byte> nextData = new List<byte>();
                    nextData.AddRange(data.Skip(i * subDimensionLength).Take(subDimensionLength));
                    result.Add(BuildCharArray(nextShape, nextData));
                }
            }

            return result;
        }

        #endregion
    }
}
