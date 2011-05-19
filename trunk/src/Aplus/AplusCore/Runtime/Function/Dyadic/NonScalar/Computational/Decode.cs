using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Computational
{
    class Decode : AbstractDyadicFunction
    {
        #region Variables

        private List<double> y;
        private AType x;
        private ATypes type;
        private bool convert;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            PrepareVariables(left, right);
            return Compute();
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Check left and right side to statisfied conditions.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void PrepareVariables(AType left, AType right)
        {
            //TypeCheck!
            if (!Util.TypeCorrect(right.Type, left.Type, "FF", "II", "FI", "IF", "FN", "NF", "IN", "NI", "NN"))
            {
                throw new Error.Type(TypeErrorText);
            }

            this.type = (left.Type == ATypes.AFloat || right.Type == ATypes.AFloat || right.Type == ATypes.ANull) ? ATypes.AFloat : ATypes.AInteger;

            this.convert = this.type == ATypes.AFloat ? false : true;

            //Righ side must be array,else Rank error.
            if (!right.IsArray)
            {
                throw new Error.Rank(RankErrorText);
            }

            this.x = right;

            //Left side must be scalar or vector.
            if (left.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            this.y = new List<double>();

            if (left.IsArray)
            {
                AType leftArray = left;

                //One-element vector case, then we reshape it: (#x) rho y.
                if (leftArray.Length == 1)
                {
                    for (int i = 0; i < this.x.Length; i++)
                    {
                        this.y.Add(leftArray[0].asFloat);
                    }
                }
                else
                {
                    foreach (AType item in leftArray)
                    {
                        this.y.Add(item.asFloat);
                    }
                }
            }
            else
            {
                //Scalar case, reshape it: (#x) rho y.
                for (int i = 0; i < this.x.Length; i++)
                {
                    this.y.Add(left.asFloat);
                }
            }

            //Left and right side length have to equal!
            if (this.y.Count != this.x.Length)
            {
                throw new Error.Length(LengthErrorText);
            }
        }

        #endregion

        #region Computation

        private AType Compute()
        {
            AType result = DecodeArray(this.x);

            if (this.convert)
            {
                result = Convert(result);
            }

            return result;
        }

        /// <summary>
        /// Decode argument vector.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType DecodeVector(AType argument)
        {
            double result = argument.Length > 0 ? argument[0].asFloat : 0;

            for (int i = 1; i < this.y.Count; i++)
            {
                result = result * this.y[i] + argument[i].asFloat;
            }

            if (type == ATypes.AInteger && !IsInteger(result))
            {
                this.convert = false;
            }

            return AFloat.Create(result);
        }

        /// <summary>
        /// Check number can be representeted as Integer.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool IsInteger(double number)
        {
            return Int32.MinValue <= number && number <= Int32.MaxValue && number % 1 == 0;
        }

        /// <summary>
        /// If argument is matrix, each element i#r of the result is evaluation
        /// of the column x[;i]. If (rho rho x) > 2, each element of the result is the
        /// evalution corresponding vector along the first axis of x.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType DecodeArray(AType argument)
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

                    result.AddWithNoUpdate(DecodeArray(argument[indexes]));

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
                return DecodeVector(argument);
            }
        }

        /// <summary>
        /// The result type usually float, but if the items can be represent as integer
        /// then we convert items to float.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Convert(AType argument)
        {
            if (argument.Rank > 0)
            {
                AType argumentArray = argument;

                AType result = AArray.Create(ATypes.AArray);

                foreach (AType item in argumentArray)
                {
                    result.AddWithNoUpdate(Convert(item));
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
