using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Restructure : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            int cutValue = GetCutNumber(right, left);
            return Compute(right, cutValue);
        }

        #endregion

        #region Preparation

        private int GetCutNumber(AType right, AType left)
        {
            if (!(left.Type == ATypes.AFloat || left.Type == ATypes.AInteger || left.Type == ATypes.ANull))
            {
                throw new Error.Type(this.TypeErrorText);
            }
            
            AType scalar;
            int cutValue;

            // get the first scalar value with length check on
            if (!left.TryFirstScalar(out scalar, true))
            {
                throw new Error.Nonce(this.NonceErrorText);
            }

            // check if the scalar is a whole number and set the desired count of items
            if (!left.ConvertToRestrictedWholeNumber(out cutValue))
            {
                throw new Error.Type(this.TypeErrorText);
            }

            if (right.Rank > 8)
            {
                throw new Error.MaxRank(MaxRankErrorText);
            }

            if (right.Rank == 0 && cutValue != 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            return cutValue;
        }

        #endregion

        #region Computation

        private AType Compute(AType items, int cutValue)
        {
            AType result;

            if (items.IsArray)
            {
                int absoluteCutValue = Math.Abs(cutValue);
                int firstAxisLength;

                // compute the first axis with corrpespond formula
                if (cutValue > 0)
                {
                    if (items.Length % cutValue != 0)
                    {
                        throw new Error.Length(LengthErrorText);
                    }

                    firstAxisLength = items.Length / cutValue;

                } // y <= 0
                else
                {
                    if (items.Length < absoluteCutValue - 1)
                    {
                        throw new Error.Length(LengthErrorText);
                    }

                    firstAxisLength = cutValue + items.Length + 1;
                }

                result = AArray.Create(ATypes.AType);

                if (items.Length > 0)
                {
                    int counter = 0;

                    for (int i = 0; i < firstAxisLength; i++)
                    {
                        AType item = AArray.Create(ATypes.AArray);

                        // select the correspond element if y > 0: get the following element else iota abs_y + i # items.
                        for (int j = 0; j < absoluteCutValue; j++)
                        {
                            item.AddWithNoUpdate(items[cutValue > 0 ? counter++ : i + j].Clone());
                        }

                        item.Length = absoluteCutValue;
                        item.Shape = new List<int>() { absoluteCutValue };
                        item.Shape.AddRange(items[0].Shape);
                        item.Rank = items.Rank;
                        item.Type = 
                            item.Length > 0 ? item[0].Type : (items.MixedType() ? ATypes.ANull : items.Type);

                        result.AddWithNoUpdate(item);
                    }
                }

                result.Length = firstAxisLength;
                result.Shape = new List<int>() { firstAxisLength };
                result.Shape.Add(absoluteCutValue);
                if (items.Rank > 1)
                {
                    result.Shape.AddRange(items.Shape.GetRange(1, items.Shape.Count - 1));
                }

                result.Rank = 1 + items.Rank;
                result.Type = 
                    result.Length > 0 ? result[0].Type : (items.MixedType() ? ATypes.ANull : items.Type);
            }
            else
            {
                result = AArray.Create(items.Type, items);
            }

            return result;
        }

        #endregion
    }
}
