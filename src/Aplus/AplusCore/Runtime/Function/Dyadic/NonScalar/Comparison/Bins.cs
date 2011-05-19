using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Comparison
{
    class Bins : AbstractDyadicFunction
    {
        #region Variables

        private AType interval;
        private List<int> cellShape;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            PrepareVariables(left,right);
            return MultipleItmesWalking(right);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Prepare the left side and determine the cellshape.
        /// </summary>
        /// <param name="left"></param>
        private void PrepareVariables(AType left, AType right)
        {
            if (left.IsBox || right.IsBox || left.Type != right.Type && !Util.TypeCorrect(right.Type, left.Type, "FI", "IF", "N?", "?N"))
            {
                throw new Error.Type(TypeErrorText);
            }

            this.interval = left;
            this.cellShape = (this.interval.Rank > 1 && this.interval.Length > 0) ? this.interval[0].Shape : new List<int>();

            if (right.Rank < this.cellShape.Count)
            {
                throw new Error.Rank(RankErrorText);
            }
        }

        #endregion

        #region Computation

        /// <summary>
        /// Classify element to the correspond class. 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private AType Classify(AType element)
        {
            int index = this.interval.Length;

            if (this.interval.IsArray)
            {
                AType intervalArray = this.interval;

                for (int i = 0; i < intervalArray.Length; i++)
                {
                    if (element.CompareTo(intervalArray[i]) <= 0)
                    {
                        index = i;
                        break;
                    }
                }
            }
            else
            {
                if (element.CompareTo(this.interval) <= 0)
                {
                    index = 0;
                }
            }

            return AInteger.Create(index);
        }

        /// <summary>
        /// If the cell rank > 1, first step is check the shape of actual cell equal to determined cell shape.
        /// If different we throw Length error. The second step is classify the cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private AType MultipleItmesWalking(AType cell)
        {
            if (this.cellShape.Count == cell.Shape.Count)
            {
                if (!this.cellShape.SequenceEqual(cell.Shape))
                {
                    throw new Error.Length(LengthErrorText);
                }
                return Classify(cell);
            }
            else
            {
                AType result = AArray.Create(ATypes.AInteger);

                foreach (AType item in cell)
                {
                    result.AddWithNoUpdate(MultipleItmesWalking(item));
                }
                result.UpdateInfo();

                return result;
            }
        }

        #endregion
    }
}
