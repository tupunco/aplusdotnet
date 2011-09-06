using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Computational
{
    class Encode : AbstractDyadicFunction
    {
        #region Encode information

        class EncodeInformation
        {
            private int index;
            private double[] encodeKeys;
            private double[] encodeValues;
            private ATypes resultingType;

            public EncodeInformation(double[] encodeKeys, double[] encodeValues, ATypes resultingType)
            {
                this.index = 0;
                this.encodeKeys = encodeKeys;
                this.encodeValues = encodeValues;
                this.resultingType = resultingType;
            }

            internal ATypes ResultingType
            {
                get { return this.resultingType; }
            }

            internal int Index
            {
                get { return this.index; }
                set { this.index = value; }
            }

            internal double[] EncodeValues
            {
                get { return this.encodeValues; }
            }

            internal double[] EncodeKeys
            {
                get { return this.encodeKeys; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            EncodeInformation arguments = ExtractEncodeInformation(left, right);
            return Compute(left, right, arguments);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Type check, and extract data from left and right side.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private EncodeInformation ExtractEncodeInformation(AType left, AType right)
        {
            // TypeCheck!
            if (!Util.TypeCorrect(right.Type, left.Type, "FF", "II", "FI", "IF", "FN", "NF", "IN", "NI", "NN"))
            {
                throw new Error.Type(TypeErrorText);
            }

            // Left argument must be a scalar or vector.
            if (left.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            double[] encodeKeys;
            if (left.IsArray)
            {
                encodeKeys = left.Select(item => item.asFloat).ToArray();
            }
            else
            {
                encodeKeys = new double[] { left.asFloat };
            }

            ATypes resultingType;
            if (left.Type == ATypes.AFloat || right.Type == ATypes.AFloat ||
                left.Type == ATypes.ANull || right.Type == ATypes.ANull)
            {
                resultingType = ATypes.AFloat;
            }
            else
            {
                resultingType = ATypes.AInteger;
            }

            List<double> encodeValues = new List<double>();
            ExtractItems(encodeValues, right);

            EncodeInformation arguments = new EncodeInformation(encodeKeys, encodeValues.ToArray(), resultingType);
            return arguments;
        }

        /// <summary>
        /// Create double list from the right argument.
        /// </summary>
        /// <param name="right"></param>
        private static void ExtractItems(List<double> vector, AType right)
        {
            if (right.Rank > 0)
            {
                foreach (AType item in right)
                {
                    ExtractItems(vector, item);
                }
            }
            else
            {
                vector.Add(right.asFloat);
            }
        }

        #endregion

        #region Computation

        private static AType Compute(AType left, AType right, EncodeInformation encodeInfo)
        {
            AType result;

            // Left and right side are scalars.
            if (left.Rank == 0 && right.Rank == 0)
            {
                result = EncodeOneStep(0, encodeInfo);
            }
            else if (left.Rank == 0 && right.Rank > 0)
            {
                result = EncodeArray(right.Shape, 0, encodeInfo);
            }
            else
            {
                result = AArray.Create(encodeInfo.ResultingType);

                for (int i = encodeInfo.EncodeKeys.Length - 1; i >= 0; i--)
                {
                    encodeInfo.Index = 0;
                    AType item = right.Shape.Count > 0
                        ? EncodeArray(right.Shape, i, encodeInfo)
                        : EncodeOneStep(i, encodeInfo);
                    result.AddWithNoUpdate(item);
                }

                result.Length = encodeInfo.EncodeKeys.Length;
                result.Shape = new List<int>() { result.Length };
                result.Shape.AddRange(right.Shape);
                result.Rank = 1 + right.Rank;

                result = MonadicFunctionInstance.Reverse.Execute(result);
            }

            return result;
        }

        /// <summary>
        /// Encode right side with left side.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="counter"></param>
        /// <param name="encodeInfo"></param>
        /// <returns></returns>
        private static AType EncodeArray(List<int> shape, int counter, EncodeInformation encodeInfo)
        {
            List<int> cutShape;
            AType result = AArray.Create(encodeInfo.ResultingType);

            for (int i = 0; i < shape[0]; i++)
            {
                if (shape.Count > 1)
                {
                    cutShape = shape.GetRange(1, shape.Count - 1);

                    result.AddWithNoUpdate(EncodeArray(cutShape, counter, encodeInfo));
                }
                else
                {
                    result.AddWithNoUpdate(EncodeOneStep(counter, encodeInfo));
                }
            }

            result.Length = shape[0];
            result.Shape = new List<int>(shape);
            result.Rank = shape.Count;

            return result;
        }

        /// <summary>
        /// Encode x[index] with y[counter].
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="encodeInfo"></param>
        /// <returns></returns>
        private static AType EncodeOneStep(int counter, EncodeInformation encodeInfo)
        {
            double remainder;

            if (encodeInfo.EncodeKeys[counter] != 0)
            {
                remainder = encodeInfo.EncodeValues[encodeInfo.Index] % encodeInfo.EncodeKeys[counter];

                bool signDifference = 
                    (encodeInfo.EncodeKeys[counter] < 0 && encodeInfo.EncodeValues[encodeInfo.Index] > 0)
                    || (encodeInfo.EncodeKeys[counter] > 0 && encodeInfo.EncodeValues[encodeInfo.Index] < 0);

                if (signDifference && remainder != 0)
                {
                    remainder += encodeInfo.EncodeKeys[counter];
                }

                encodeInfo.EncodeValues[encodeInfo.Index] = 
                    (encodeInfo.EncodeValues[encodeInfo.Index] - remainder) / encodeInfo.EncodeKeys[counter];
            }
            else
            {
                remainder = encodeInfo.EncodeValues[encodeInfo.Index];
                encodeInfo.EncodeValues[encodeInfo.Index] = 0;
            }

            encodeInfo.Index++;

            AType result;

            if (encodeInfo.ResultingType == ATypes.AInteger)
            {
                result = AInteger.Create((int)remainder);
            }
            else
            {
                result = AFloat.Create(remainder);
            }

            return result;
        }

        #endregion
    }
}
