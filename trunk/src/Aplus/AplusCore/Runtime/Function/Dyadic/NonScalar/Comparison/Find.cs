using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Comparison
{
    class Find : AbstractDyadicFunction
    {
        #region Nested class for argument pass

        class FindArguments
        {
            private AType leftArgument;
            private List<int> cellShape;

            internal AType Items
            {
                get { return this.leftArgument; }
                set { this.leftArgument = value; }
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
            FindArguments arguments = PrepareVariables(left, right);
            return MultipleItemsWalking(right, arguments);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Prepare the left side and determine the cellshape.
        /// </summary>
        /// <param name="left"></param>
        private FindArguments PrepareVariables(AType left, AType right)
        {
            if (!Utils.IsSameGeneralType(left, right) && !Util.TypeCorrect(right.Type, left.Type, "N?", "?N"))
            {
                throw new Error.Type(TypeErrorText);
            }


            FindArguments arguments = new FindArguments()
            {
                Items = left,
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
        /// If the cell rank > 1, first step is check the shape of actual cell equal to determined cell shape.
        /// If different we throw Length error. The second step is classify the cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="arguments">Instead of class variables.</param>
        /// <returns></returns>
        private AType MultipleItemsWalking(AType cell, FindArguments arguments)
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

        /// <summary>
        /// The value of that element is the index of the first item of y to which the cell is identical.
        /// </summary>
        /// <param name="element">AType to search for.</param>
        /// <param name="arguments">Instead of class variables.</param>
        /// <returns></returns>
        private AType Classify(AType element, FindArguments arguments)
        {
            int resultIndex;

            if (arguments.Items.IsArray)
            {
                // check if the given left argument of the function contains the element
                for (resultIndex = 0; resultIndex < arguments.Items.Length; resultIndex++)
                {
                    if (ComparisonToleranceCompareTo(arguments.Items[resultIndex], element))
                    {
                        break;
                    }
                }
            }
            else if (ComparisonToleranceCompareTo(arguments.Items, element))
            {
                resultIndex = 0;
            }
            else
            {
                // item not found
                resultIndex = arguments.Items.Length;
            }

            return AInteger.Create(resultIndex);
        }

        private bool ComparisonToleranceCompareTo(AType actual, AType other)
        {
            if (actual.Rank > 0)
            {
                for (int i = 0; i < actual.Length; i++)
                {
                    if (!ComparisonToleranceCompareTo(actual[i], other[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                if (Util.TypeCorrect(actual.Type, other.Type, "II", "FF", "IF", "FI"))
                {
                    return Utils.ComparisonTolerance(actual.asFloat, other.asFloat);
                }
                else if (Util.TypeCorrect(actual.Type, other.Type, "CC"))
                {
                    return actual.asChar.CompareTo(other.asChar) == 0;
                }
                else if (Util.TypeCorrect(actual.Type, other.Type, "SS"))
                {
                    return actual.asString.CompareTo(other.asString) == 0;
                }
                else if (Util.TypeCorrect(actual.Type, other.Type, "UU") || actual.IsBox && other.IsBox)
                {
                    return actual.Equals(other);
                }

                return false;
            }
        }

        #endregion
    }
}
