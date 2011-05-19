using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Rotate : AbstractDyadicFunction
    {
        #region Attributes

        private List<int> rotateVector;
        private AType items;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            if (PrepareRotateVector(right, left))
            {
                return right.Clone();
            }
            else
            {
                this.items = right;

                if (this.items.Rank > 2)
                {
                    return TransformAndCompute(right);
                }
                else
                {
                    return Compute();
                }
            }
        }

        #endregion

        #region Preparation

        private bool PrepareRotateVector(AType right, AType left)
        {
            rotateVector = new List<int>();

            if (!(left.Type == ATypes.AFloat || left.Type == ATypes.AInteger || left.Type == ATypes.ANull))
            {
                // Allowed types are: AFloat, AInteger and ANull
                // otherwise throw Type error
                throw new Error.Type(TypeErrorText);
            }

            AType scalar;

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

                // If the left is AArray and rank is bigger than 1, we will Ravel it.
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

            //If the right argument is scalar, we clone it!
            return right.Rank == 0;
        }

        /// <summary>
        /// Transform rightside to matrix, if the rank is bigger than 2.
        /// After the computation transform back the result to the original shape.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        private AType TransformAndCompute(AType right)
        {
            AType row = AInteger.Create(this.items.Shape[0]);

            int temp = 1;
            for (int i = 1; i < this.items.Shape.Count; i++)
            {
                temp *= this.items.Shape[i];
            }

            AType column = AInteger.Create(temp);

            AType shape = AArray.Create(ATypes.AInteger);
            shape.AddWithNoUpdate(row);
            shape.AddWithNoUpdate(column);
            shape.Length = 2;
            shape.Shape = new List<int>() { 2 };
            shape.Rank = 1;

            this.items = DyadicFunctionInstance.Reshape.Execute(this.items, shape);

            AType result = Compute();

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
        private AType RotateArray(int number, AType item)
        {
            if (number == 0)
            {
                return item;
            }
            else
            {
                AType result = AArray.Create(ATypes.AArray);

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

                return result;
            }
        }

        private AType Compute()
        {
            AType result = null;

            if (this.items.Rank == 1)
            {
                result = RotateArray(this.rotateVector[0], this.items).Clone();
            }
            else //The matrix case.
            {
                List<AType> indexes = new List<AType>();
                indexes.Add(Utils.ANull());
                indexes.Add(Utils.ANull());

                result = AArray.Create(ATypes.AArray);
                List<AType> columns = new List<AType>();

                for (int i = 0; i < this.items.Shape[1]; i++)
                {
                    indexes[1] = AInteger.Create(i);
                    columns.Add(RotateArray(rotateVector[this.rotateVector.Count > 1 ? i : 0], this.items[indexes]));
                }

                result = ReStructure(columns);
            }

            return result;
        }

        /// <summary>
        /// Change column represenation to row representation.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private AType ReStructure(List<AType> item)
        {
            AType result = AArray.Create(ATypes.AArray);
            AType row;

            for (int i = 0; i < this.items.Shape[0]; i++)
            {
                row = AArray.Create(ATypes.AArray);

                for (int j = 0; j < this.items.Shape[1]; j++)
                {
                    row.AddWithNoUpdate(item[j][i].Clone());
                }

                row.Length = this.items.Shape[1];
                row.Shape = new List<int>() { row.Length };
                row.Rank = 1;
                row.Type = row.Length > 0 ? row[0].Type : (this.items.MixedType() ? ATypes.ANull : this.items.Type);

                result.AddWithNoUpdate(row);
            }

            result.Length = this.items.Length;
            result.Shape = new List<int>() { result.Length };
            result.Shape.AddRange(this.items.Shape.GetRange(1, this.items.Shape.Count - 1));
            result.Rank = this.items.Rank;
            result.Type = result.Length > 0 ? result[0].Type : (this.items.MixedType() ? ATypes.ANull : this.items.Type);
            
            return result;
        }

        #endregion
    }
}
