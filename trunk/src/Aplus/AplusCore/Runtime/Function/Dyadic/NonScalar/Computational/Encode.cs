using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Computational
{
    class Encode : AbstractDyadicFunction
    {
        #region Variables

        private List<double> y;
        private List<double> x;
        private int index;
        private ATypes type;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            PrepareVariables(left, right);
            return Compute(left, right);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Type check, and extract data from left and right side.
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

            //Left argument must be a scalar or vector.
            if (left.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            this.y = new List<double>();

            if (left.IsArray)
            {
                foreach (AType item in left)
                {
                    this.y.Add(item.asFloat);
                }
            }
            else
            {
                this.y.Add(left.asFloat);
            }

            this.x = new List<double>();

            if (left.Type == ATypes.AFloat || right.Type == ATypes.AFloat ||
                left.Type == ATypes.ANull  || right.Type == ATypes.ANull)
            {
                this.type = ATypes.AFloat;
            }
            else
            {
                this.type = ATypes.AInteger;
            }

            RavelRightSideToArray(right);
        }

        /// <summary>
        /// Create double list from the right argument.
        /// </summary>
        /// <param name="right"></param>
        private void RavelRightSideToArray(AType right)
        {
            if (right.Rank > 0)
            {
                foreach (AType item in right)
                {
                    RavelRightSideToArray(item);
                }
            }
            else
            {
                this.x.Add(right.asFloat);
            }
        }

        #endregion

        #region Computation

        private AType Compute(AType left, AType right)
        {
            this.index = 0;

            //Left and right side are scalars.
            if (left.Rank == 0 && right.Rank == 0)
            {
                return EncodeOneStep(0);
            }
            else if (left.Rank == 0 && right.Rank > 0)
            {
                return EncodeArray(right.Shape, 0);
            }
            else
            {
                AType result = AArray.Create(type);

                for (int i = this.y.Count - 1; i >= 0; i--)
                {
                    this.index = 0;

                    result.AddWithNoUpdate(right.Shape.Count > 0 ? EncodeArray(right.Shape, i) : EncodeOneStep(i));
                }

                result.Length = this.y.Count;
                result.Shape = new List<int>() { result.Length };
                result.Shape.AddRange(right.Shape);
                result.Rank = 1 + right.Rank;

                return MonadicFunctionInstance.Reverse.Execute(result);
            }
        }

        /// <summary>
        /// Encode right side with left side.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="counter"></param>
        /// <returns></returns>
        private AType EncodeArray(List<int> shape, int counter)
        {
            List<int> cutShape = null;
            AType result = AArray.Create(type);

            for (int i = 0; i < shape[0]; i++)
            {
                if (shape.Count > 1)
                {
                    cutShape = new List<int>();
                    cutShape.AddRange(shape.GetRange(1, shape.Count - 1));

                    result.AddWithNoUpdate(EncodeArray(cutShape, counter));
                }
                else
                {
                    result.AddWithNoUpdate(EncodeOneStep(counter));
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
        /// <returns></returns>
        private AType EncodeOneStep(int counter)
        {
            double remainder = this.x[index] % this.y[counter];

            if (((this.y[counter] < 0 && this.x[index] > 0) ||
                (this.y[counter] > 0 && this.x[index] < 0)) &&
                remainder != 0)
            {
                remainder += this.y[counter];
            }

            this.x[this.index] = (this.x[this.index] - remainder) / this.y[counter];
            this.index++;

            AType result;

            if(this.type == ATypes.AInteger)
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
