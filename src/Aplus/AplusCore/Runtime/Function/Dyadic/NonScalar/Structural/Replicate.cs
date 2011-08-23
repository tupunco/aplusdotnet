using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Replicate : AbstractDyadicFunction
    {
        #region Variables

        private List<int> replicateVector;
        private AType items;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            PrepareReplicateVector(right,left);
            PrepareInputItems(right);
            return Compute();
        }

        #endregion

        #region Preparation

        private void PrepareReplicateVector(AType right, AType left)
        {
            this.replicateVector = new List<int>();

            if (!(left.Type == ATypes.AFloat || left.Type == ATypes.AInteger || left.Type == ATypes.ANull))
            {
                throw new Error.Type(TypeErrorText);
            }

            if (left.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            int element;
            AType scalar;

            if (left.TryFirstScalar(out scalar, true))
            {
                if (!scalar.ConvertToRestrictedWholeNumber(out element))
                {
                    throw new Error.Type(TypeErrorText);
                }

                if (element < 0)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                this.replicateVector.Add(element);
            }
            else
            {
                if (left.Length > 0)
                {
                    foreach (AType item in left)
                    {
                        if (!item.ConvertToRestrictedWholeNumber(out element))
                        {
                            throw new Error.Type(TypeErrorText);
                        }

                        if (element < 0)
                        {
                            throw new Error.Domain(DomainErrorText);
                        }

                        this.replicateVector.Add(element);
                    }
                }
                else
                {
                    this.replicateVector.Add(0);
                }

                //Lenght check should be the first than parse the left side,
                //but the A+ follow that order.
                if (right.Length != 1 && left.Length != right.Length)
                {
                    throw new Error.Length(LengthErrorText);
                }

            }
        }

        private void PrepareInputItems(AType right)
        {
            // TODO: do we still need these converts?
            this.items = right.IsArray ? right : AArray.Create(right.Type, right);
        }

        #endregion

        #region Computation

        private AType Compute()
        {
            AType result = AArray.Create(ATypes.AArray);
            int length = 0;

            if (this.replicateVector.Count == 1)
            {
                    for (int i = 0; i < this.items.Length; i++)
                    {
                        for (int j = 0; j < this.replicateVector[0]; j++)
                        {
                            result.AddWithNoUpdate(items[i].Clone());
                        }
                    }

                    length = this.replicateVector[0] * this.items.Length;
            }
            else //replicateCounter.Count > 1
            {
                for (int i = 0; i < this.replicateVector.Count; i++)
                {
                    for (int j = 0; j < this.replicateVector[i]; j++)
                    {
                        result.AddWithNoUpdate(this.items[this.items.Length > 1 ? i : 0].Clone());
                    }

                    length += this.replicateVector[i];
                }
            }

            result.Length = length;
            result.Shape = new List<int>() { length };

            if (this.items.Rank > 1)
            {
                result.Shape.AddRange(this.items.Shape.GetRange(1, this.items.Shape.Count - 1));
            }

            result.Rank = this.items.Rank;
            result.Type = length > 0 ? result[0].Type :(this.items.MixedType() ? ATypes.ANull : this.items.Type);

            return result;
        }

        #endregion
    }
}
