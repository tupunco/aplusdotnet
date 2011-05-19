using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Drop : AbstractDyadicFunction
    {
        #region Variables

        private int dropCounter;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            PrepareDropCounter(left);
            return Compute(right);
        }

        #endregion

        #region Preparation

        private void PrepareDropCounter(AType element)
        {
            if (element.Type == ATypes.AFloat || element.Type == ATypes.AInteger)
            {
                AType scalar;

                // Get the first scalar value with length check on
                if (!element.TryFirstScalar(out scalar, true))
                {
                    throw new Error.Nonce(this.NonceErrorText);
                }

                // Check if the scalar is a whole number and set the drop counter
                if (!scalar.ConvertToRestrictedWholeNumber(out this.dropCounter))
                {
                    throw new Error.Type(this.TypeErrorText);
                }
            }
            else if (element.Type == ATypes.ANull)
            {
                throw new Error.Nonce(this.NonceErrorText);
            }
            else
            {
                throw new Error.Type(this.TypeErrorText);
            }
        }

        #endregion

        #region Computation

        private AType Compute(AType right)
        {
            if (right.IsArray && dropCounter == 0)
            {
                return right.Clone();
            }

            AType result = AArray.Create(ATypes.AType);

            if (right.IsArray)
            {
                if (right.Length - Math.Abs(dropCounter) > 0)
                {
                    if (dropCounter > 0)
                    {
                        for (int i = dropCounter; i < right.Length; i++)
                        {
                            result.AddWithNoUpdate(right[i].Clone());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < right.Length + dropCounter; i++)
                        {
                            result.AddWithNoUpdate(right[i].Clone());
                        }
                    }
                    result.Length = right.Length - Math.Abs(dropCounter);
                    result.Shape = new List<int>() { result.Length };
                    if (right.Rank > 1)
                    {
                        result.Shape.AddRange(right.Shape.GetRange(1, right.Shape.Count - 1));
                    }
                    result.Rank = right.Rank;
                    result.Type = result[0].Type;
                }
                else
                {
                    result.Type = right.MixedType() ? ATypes.ANull : right.Type;
                }
            }
            else
            {
                if (dropCounter == 0)
                {
                    result.Add(right.Clone());
                    result.Length = 1;
                    result.Shape = new List<int>() { 1 };
                    result.Type = right.Type;

                }
                else
                {
                    result.Type = right.MixedType() ? ATypes.ANull : right.Type;
                }
            }
            return result;
        }

        #endregion
    }
}
