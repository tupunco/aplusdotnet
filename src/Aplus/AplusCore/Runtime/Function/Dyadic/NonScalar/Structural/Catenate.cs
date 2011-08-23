using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Catenate : AbstractDyadicFunction
    {
        #region DLR Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            // First we check if one side is an ANull
            if (right.Type == ATypes.ANull)
            {
                return AArray.Create(left.Type, left.Clone());
            }
            else if (left.Type == ATypes.ANull)
            {
                return AArray.Create(right.Type, right.Clone());
            }

            // Type check
            if(!Utils.IsSameGeneralType(left, right))
            {
                throw new Error.Type(this.TypeErrorText);
            }

            AType result = CreateResult(right, left);

            // Convert to float if one of the arguments is an AFloat and the other is an AInteger
            if (Utils.DifferentNumberType(left, right))
            {
                result.ConvertToFloat();
            }

            return result;
        }

        #endregion

        #region Catenate

        /// <summary>
        /// Creates the resulting AArray
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns>AArray containing the concatenated elements.</returns>
        private AType CreateResult(AType right, AType left)
        {
            AType result = AArray.Create(left.Type);
            // Create a clone from the arguments to avoid side effects
            AType clonedLeft = left.Clone();
            AType clonedRight = right.Clone();


            if (!clonedLeft.IsArray && !clonedRight.IsArray)
            {
                // example input: 1 , 2

                result.AddWithNoUpdate(clonedLeft);
                result.Add(clonedRight);
            }
            else if (clonedLeft.IsArray && !clonedRight.IsArray)
            {
                // example input: 1 2 , 3

                result.AddRange(clonedLeft);
                AType item = PrepareItem(clonedRight, clonedLeft);

                result.Add(item);
            }
            else if (!clonedLeft.IsArray && clonedRight.IsArray)
            {
                // example input: 1 , 2 3

                result.AddWithNoUpdate(PrepareItem(clonedLeft, clonedRight));
                result.AddRange(clonedRight);
            }
            // now both left and right argument is an AArray

            else if (clonedLeft.Rank == clonedRight.Rank)
            {
                // example input: (iota 2 2) , (iota 2 2)

                CheckItemsShape(clonedLeft, clonedRight);

                if (clonedLeft.Length > 0)
                {
                    result.AddRange(clonedLeft);
                }
                if (clonedRight.Length > 0)
                {
                    result.AddRange(clonedRight);
                }
            }
            else if ((clonedLeft.Rank - clonedRight.Rank) == 1)
            {
                // example input: (iota 2 2 2) , (iota 2 2)

                CheckShape(clonedLeft.Shape.GetRange(1, clonedLeft.Shape.Count - 1), clonedRight.Shape);

                result.AddRange(clonedLeft);
                result.Add(clonedRight);
            }
            else if ((clonedLeft.Rank - clonedRight.Rank) == -1)
            {
                // example input: (iota 2 2) , (iota 2 2 2)

                CheckShape(clonedLeft.Shape, clonedRight.Shape.GetRange(1, clonedRight.Shape.Count - 1));

                result.AddWithNoUpdate(clonedLeft);
                result.AddRange(clonedRight);
            }
            else
            {
                // The rank difference was bigger than 1.
                throw new Error.Rank(this.RankErrorText);
            }

            return result;
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Prepares the item for adding it to the resulting AArray.
        /// Used only if one of the arguments is a scalar
        /// </summary>
        /// <param name="item"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        private AType PrepareItem(AType item, AType required)
        {
            // if the required item contains AArray's then reshape the current item
            if (required.Rank > 1)
            {
                return DyadicFunctionInstance.Reshape.Execute(
                    item,
                    required.Shape.GetRange(1, required.Shape.Count - 1).ToAArray()
                );
            }

            return item;
        }

        /// <summary>
        /// Simple shape check. If it fails throws a Length error.
        /// </summary>
        /// <exception cref="Error.Length">Length error if the shape is different.</exception>
        private void CheckShape(List<int> leftShape, List<int> rightShape)
        {
            if (!leftShape.SequenceEqual<int>(rightShape))
            {
                throw new Error.Length(this.LengthErrorText);
            }
        }


        private void CheckItemsShape(AType leftArray, AType rightArray)
        {
            AType item = null;

            if (leftArray.Length > 0)
            {
                item = leftArray[0];
            }
            else if (rightArray.Length > 0)
            {
                item = rightArray[0];
            }

            if (item != null)
            {
                for (int i = 0; i < rightArray.Length; i++)
                {
                    CheckShape(item.Shape, rightArray[i].Shape);
                }
            }
        }


        #endregion
    }
}
