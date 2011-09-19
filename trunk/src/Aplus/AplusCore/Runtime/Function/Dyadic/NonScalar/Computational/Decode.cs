using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Computational
{
    class Decode : AbstractDyadicFunction
    {
        #region Decode information

        class DecodeInformation
        {
            private double[] decodeValues;
            private ATypes type;
            private bool integerConversion;

            internal DecodeInformation(ATypes resultingType, double[] decodeValues)
            {
                this.type = resultingType;
                this.RequiresConvert = (resultingType != ATypes.AFloat);// ? false : true;
                this.decodeValues = decodeValues;
            }

            internal bool RequiresConvert
            {
                get { return this.integerConversion; }
                set { this.integerConversion = value; }
            }

            internal ATypes Type
            {
                get { return type; }
            }

            internal double[] DecodeValues
            {
                get { return decodeValues; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            DecodeInformation info = ExtractDecodeInformation(left, right);
            AType result = DecodeArray(right, info);

            if (info.RequiresConvert)
            {
                result = ConvertToInteger(result);
            }

            return result;
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Extracts <see cref="DecodeInformation"/> from the arguments.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private DecodeInformation ExtractDecodeInformation(AType left, AType right)
        {
            // Error if the arguments are not numbers or Null
            if (!((left.IsNumber || left.Type == ATypes.ANull) && (right.IsNumber || right.Type == ATypes.ANull))) 
            {
                throw new Error.Type(TypeErrorText);
            }

            // righ side must be array
            if (!right.IsArray)
            {
                throw new Error.Rank(RankErrorText);
            }

            // left side must be scalar or vector
            if (left.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            ATypes resultType = 
                (left.Type == ATypes.AFloat || right.Type == ATypes.AFloat || right.Type == ATypes.ANull)
                ? ATypes.AFloat
                : ATypes.AInteger;

            double[] decodeValues;

            if (left.IsArray)
            {
                if (left.Length == 1)
                {
                    // one-element vector case, then we reshape it: (#x) rho y.
                    decodeValues = Enumerable.Repeat(left[0].asFloat, right.Length).ToArray();
                }
                else
                {
                    // left and right side length have to equal!
                    if (left.Length != right.Length)
                    {
                        throw new Error.Length(LengthErrorText);
                    }

                    decodeValues = left.Select(item => item.asFloat).ToArray();
                }
            }
            else
            {
                // scalar case, reshape it: (#x) rho y.
                decodeValues = Enumerable.Repeat(left.asFloat, right.Length).ToArray();
            }

            return new DecodeInformation(resultType, decodeValues);
        }

        #endregion

        #region Computation

        /// <summary>
        /// Decode argument vector.
        /// </summary>
        /// <returns></returns>
        private static AType DecodeVector(AType argument, DecodeInformation decodeInfo)
        {
            double result = argument.Length > 0 ? argument[0].asFloat : 0;

            for (int i = 1; i < decodeInfo.DecodeValues.Length; i++)
            {
                result = result * decodeInfo.DecodeValues[i] + argument[i].asFloat;
            }

            if (decodeInfo.Type == ATypes.AInteger && !IsInteger(result))
            {
                decodeInfo.RequiresConvert = false;
            }

            return AFloat.Create(result);
        }

        /// <summary>
        /// Check number can be representeted as Integer.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool IsInteger(double number)
        {
            return Int32.MinValue <= number && number <= Int32.MaxValue && number % 1 == 0;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// If argument is matrix, each element i#r of the result is evaluation
        /// of the column x[;i]. If (rho rho x) > 2, each element of the result is the
        /// evalution corresponding vector along the first axis of x.
        /// </remarks>
        /// <param name="argument"></param>
        /// <param name="decodeInfo"></param>
        /// <returns></returns>
        private static AType DecodeArray(AType argument, DecodeInformation decodeInfo)
        {
            List<AType> indexes = new List<AType>() { Utils.ANull() };
            AType index;

            if (argument.Rank > 1)
            {
                AType result = AArray.Create(ATypes.AFloat);

                for (int i = 0; i < argument.Shape[1]; i++)
                {
                    index = AInteger.Create(i);
                    indexes.Add(index);

                    result.AddWithNoUpdate(DecodeArray(argument[indexes], decodeInfo));

                    indexes.Remove(index);
                }

                result.Length = argument.Shape[1];
                result.Shape = new List<int>();
                result.Shape.AddRange(argument.Shape.GetRange(1, argument.Rank - 1));
                result.Rank = argument.Rank - 1;

                return result;
            }
            else
            {
                return DecodeVector(argument, decodeInfo);
            }
        }

        /// <summary>
        /// Convert items to integers.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static AType ConvertToInteger(AType argument)
        {
            if (argument.Rank > 0)
            {
                AType result = AArray.Create(ATypes.AArray);

                foreach (AType item in argument)
                {
                    result.AddWithNoUpdate(ConvertToInteger(item));
                }

                result.UpdateInfo();
                result.Type = ATypes.AInteger;

                return result;
            }
            else
            {
                return AInteger.Create(argument.asInteger);
            }
        }

        #endregion
    }
}
