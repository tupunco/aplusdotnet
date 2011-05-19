using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Restructure : AbstractDyadicFunction
    {
        #region Variables

        private int y;
        private AType items;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            PrepareVariables(right, left);
            return Compute();
        }

        #endregion

        #region Preparation

        private void PrepareVariables(AType right, AType left)
        {
            if (left.Type == ATypes.AFloat || left.Type == ATypes.AInteger || left.Type == ATypes.ANull)
            {
                AType scalar;

                // Get the first scalar value with length check on
                if (!left.TryFirstScalar(out scalar, true))
                {
                    throw new Error.Nonce(this.NonceErrorText);
                }

                // Check if the scalar is a whole number and set the desired count of items
                if (!left.ConvertToRestrictedWholeNumber(out this.y))
                {
                    throw new Error.Type(this.TypeErrorText);
                }
            }
            else
            {
                throw new Error.Type(this.TypeErrorText);
            }

            if (right.Rank > 8)
            {
                throw new Error.MaxRank(MaxRankErrorText);
            }

            if(right.Rank == 0 && y != 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            this.items = right;
        }

        #endregion

        #region Computation

        private AType Compute()
        {
            AType result;

            if (this.items.IsArray)
            {
                result = AArray.Create(ATypes.AType);
                AType item;
                int firstAxisLength, abs_y, counter = 0;

                //Compute the first axis with corrpespond formula.
                if (this.y > 0)
                {
                    abs_y = this.y;

                    if (this.items.Length % this.y != 0)
                    {
                        throw new Error.Length(LengthErrorText);
                    }

                    firstAxisLength = this.items.Length / this.y;

                } // y <= 0
                else
                {
                    abs_y = Math.Abs(this.y);

                    if (this.items.Length < abs_y - 1)
                    {
                        throw new Error.Length(LengthErrorText);
                    }

                    firstAxisLength = this.y + 1 + this.items.Length;
                }

                if (this.items.Length > 0)
                {
                    for (int i = 0; i < firstAxisLength; i++)
                    {
                        item = AArray.Create(ATypes.AArray);

                        //Select the correspond element if y > 0: get the following element else iota abs_y + i # items.
                        for (int j = 0; j < abs_y; j++)
                        {
                            item.AddWithNoUpdate(this.items[this.y > 0 ? counter++ : i + j].Clone());
                        }

                        item.Length = abs_y;
                        item.Shape = new List<int>() { abs_y };
                        item.Shape.AddRange(this.items[0].Shape);
                        item.Rank = this.items.Rank;
                        item.Type = item.Length > 0 ? item[0].Type : (this.items.MixedType() ? ATypes.ANull : items.Type);

                        result.AddWithNoUpdate(item);
                    }
                }

                //Set type, length, shape and rank.

                result.Length = firstAxisLength;
                result.Shape = new List<int>() { firstAxisLength };
                result.Shape.Add(abs_y);
                if (this.items.Rank > 1)
                {
                    result.Shape.AddRange(this.items.Shape.GetRange(1, this.items.Shape.Count - 1));
                }
                result.Rank = 1 + this.items.Rank;
                result.Type = result.Length > 0 ? result[0].Type : (this.items.MixedType() ? ATypes.ANull : items.Type);
            }
            else
            {
                result = AArray.Create(this.items.Type, this.items);
            }

            return result;
        }

        #endregion
    }
}
