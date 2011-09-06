using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Comparison
{
    class Bins : AbstractDyadicFunction
    {
        #region Nested class for argument pass

        class CalculationArguments
        {
            private AType interval;
            private List<int> cellShape;

            internal AType Interval
            {
                get { return this.interval; }
                set { this.interval = value; }
            }

            internal List<int> CellShape
            {
                get { return this.cellShape; }
                set { this.cellShape = value; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            CalculationArguments arguments = PrepareVariables(left, right);
            return MultipleItemsWalking(right, arguments);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Prepare the left side and determine the cellshape.
        /// </summary>
        /// <param name="left"></param>
        private CalculationArguments PrepareVariables(AType left, AType right)
        {
            if (left.IsBox || right.IsBox ||
                (left.Type != right.Type && !Util.TypeCorrect(right.Type, left.Type, "FI", "IF", "N?", "?N")))
            {
                throw new Error.Type(TypeErrorText);
            }

            CalculationArguments arguments = new CalculationArguments()
            {
                Interval = left,
                CellShape = (left.Rank > 1 && left.Length > 0) ? left[0].Shape : new List<int>()
            };

            if (right.Rank < arguments.CellShape.Count)
            {
                throw new Error.Rank(RankErrorText);
            }

            return arguments;
        }

        #endregion

        #region Computation

        /// <summary>
        /// Classify element to the corresponding group.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="arguments">Instead of class variables.</param>
        /// <returns></returns>
        private AType Classify(AType element, CalculationArguments arguments)
        {
            int index;

            if (arguments.Interval.IsArray)
            {
                AType intervalArray = arguments.Interval;

                for (index = 0; index < intervalArray.Length; index++)
                {
                    if (element.CompareTo(intervalArray[index]) <= 0)
                    {
                        break;
                    }
                }
            }
            else
            {
                index = (element.CompareTo(arguments.Interval) <= 0) ? 0 : arguments.Interval.Length;
            }

            return AInteger.Create(index);
        }

        /// <summary>
        /// If the cell rank > 1, first step is check the shape of actual cell equal to determined cell shape.
        /// If different we throw Length error. The second step is classify the cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="arguments">Instead of class variables.</param>
        /// <returns></returns>
        private AType MultipleItemsWalking(AType cell, CalculationArguments arguments)
        {
            if (arguments.CellShape.Count == cell.Shape.Count)
            {
                if (!arguments.CellShape.SequenceEqual(cell.Shape))
                {
                    throw new Error.Length(LengthErrorText);
                }

                return Classify(cell, arguments);
            }
            else
            {
                AType result = AArray.Create(ATypes.AInteger);

                foreach (AType item in cell)
                {
                    result.AddWithNoUpdate(MultipleItemsWalking(item, arguments));
                }
                result.UpdateInfo();

                return result;
            }
        }

        #endregion
    }
}
