using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Rotate : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            AType result;
            int[] rotateVector = PrepareRotateVector(right, left);
            
            if (right.Rank == 0)
            {
                // if the right argument is scalar, we clone it
                result = right.Clone();
            }
            else if (right.Rank > 2)
            {
                result = TransformAndCompute(right, rotateVector);
            }
            else
            {
                result = Compute(right, rotateVector);
            }

            return result;
        }

        #endregion

        #region Preparation

        private int[] PrepareRotateVector(AType right, AType left)
        {
            if (!(left.Type == ATypes.AFloat || left.Type == ATypes.AInteger || left.Type == ATypes.ANull))
            {
                // Allowed types are: AFloat, AInteger and ANull
                // otherwise throw Type error
                throw new Error.Type(TypeErrorText);
            }

            AType scalar;
            List<int> rotateVector = new List<int>();

            if (left.TryFirstScalar(out scalar, true))
            {
                int result;

                if (!scalar.ConvertToRestrictedWholeNumber(out result))
                {
                    throw new Error.Type(TypeErrorText);
                }

                rotateVector.Add(result);
            }
            else
            {
                if (right.Rank > 0)
                {
                    if (left.Rank != right.Rank - 1)
                    {
                        throw new Error.Rank(RankErrorText);
                    }

                    if (!left.Shape.SequenceEqual(right.Shape.GetRange(1, right.Shape.Count - 1)))
                    {
                        throw new Error.Length(LengthErrorText);
                    }
                }

                // if the left is AArray and rank is bigger than 1, Ravel it
                AType leftvector = left.Rank > 1 ? MonadicFunctionInstance.Ravel.Execute(left) : left;

                int element;

                foreach (AType item in leftvector)
                {
                    if (!item.ConvertToRestrictedWholeNumber(out element))
                    {
                        throw new Error.Type(TypeErrorText);
                    }

                    rotateVector.Add(element);
                }
            }

            // if the right argument is scalar, we clone it!
            return rotateVector.ToArray();
        }

        /// <summary>
        /// Transform rightside to matrix, if the rank is bigger than 2.
        /// After the computation transform back the result to the original shape.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="RotateVector"></param>
        /// <returns></returns>
        private AType TransformAndCompute(AType right, int[] RotateVector)
        {
            AType row = AInteger.Create(right.Shape[0]);
            AType column = AInteger.Create(right.Shape.GetRange(1, right.Shape.Count - 1).Product());

            AType desiredShape = AArray.Create(ATypes.AInteger,
                row,
                column
            );

            AType reshapedItems = DyadicFunctionInstance.Reshape.Execute(right, desiredShape);
            AType result = Compute(reshapedItems, RotateVector);

            return DyadicFunctionInstance.Reshape.Execute(result, right.Shape.ToAArray());
        }

        #endregion

        #region Computation

        /// <summary>
        /// Rotate the input array the correspond direction with number.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static AType RotateArray(int number, AType item)
        {
            AType result;

            if (number == 0)
            {
                result = item;
            }
            else
            {
                result = AArray.Create(ATypes.AArray);

                if (item.Length > 0)
                {
                    if (item.Length <= Math.Abs(number))
                    {
                        number = number % item.Length;
                    }

                    if (number < 0)
                    {
                        number = item.Length + number;
                    }

                    for (int i = number; i <= item.Length - 1; i++)
                    {
                        result.AddWithNoUpdate(item[i]);
                    }

                    for (int i = 0; i < number; i++)
                    {
                        result.AddWithNoUpdate(item[i]);
                    }
                }

                result.Length = item.Length;
                result.Shape = new List<int>() { result.Length };
                result.Rank = 1;
                result.Type = result.Length > 0 ? result[0].Type : (item.MixedType() ? ATypes.ANull : item.Type);
            }

            return result;
        }

        private static AType Compute(AType items, int[] RotateVector)
        {
            AType result;

            if (items.Rank == 1)
            {
                result = RotateArray(RotateVector[0], items).Clone();
            }
            else // the matrix case
            {
                List<AType> indexes = new List<AType>() { Utils.ANull(), Utils.ANull() };
                List<AType> columns = new List<AType>();

                for (int i = 0; i < items.Shape[1]; i++)
                {
                    indexes[1] = AInteger.Create(i);
                    columns.Add(
                        RotateArray(RotateVector[RotateVector.Length > 1 ? i : 0], items[indexes])
                    );
                }

                result = ReStructure(columns, items);
            }

            return result;
        }

        /// <summary>
        /// Change column represenation to row representation.
        /// </summary>
        /// <param name="columnItems"></param>
        /// <param name="originalItems"></param>
        /// <returns></returns>
        private static AType ReStructure(List<AType> columnItems, AType originalItems)
        {
            AType result = AArray.Create(ATypes.AArray);
            AType row;

            for (int i = 0; i < originalItems.Shape[0]; i++)
            {
                row = AArray.Create(ATypes.AArray);

                for (int j = 0; j < originalItems.Shape[1]; j++)
                {
                    row.AddWithNoUpdate(columnItems[j][i].Clone());
                }

                row.Length = originalItems.Shape[1];
                row.Shape = new List<int>() { row.Length };
                row.Rank = 1;
                row.Type = row.Length > 0 ? row[0].Type : (originalItems.MixedType() ? ATypes.ANull : originalItems.Type);

                result.AddWithNoUpdate(row);
            }

            result.Length = originalItems.Length;
            result.Shape = new List<int>() { result.Length };
            result.Shape.AddRange(originalItems.Shape.GetRange(1, originalItems.Shape.Count - 1));
            result.Rank = originalItems.Rank;
            result.Type = result.Length > 0 ? result[0].Type : (originalItems.MixedType() ? ATypes.ANull : originalItems.Type);

            return result;
        }

        #endregion
    }
}
