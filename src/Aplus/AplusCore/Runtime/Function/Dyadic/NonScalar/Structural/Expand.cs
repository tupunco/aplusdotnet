using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Expand : AbstractDyadicFunction
    {
        #region Variables

        private List<byte> expandVector;
        private AType items;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            PrepareExpandVector(left);
            PrepareInputItems(right);
            return Compute();
        }

        #endregion

        #region Preparation

        private void PrepareExpandVector(AType left)
        {
            this.expandVector = new List<byte>();

            //If the left side is User defined function, we throw Valence error.
            //This part excatly belongs to Scan.
            if (left.Type == ATypes.AFunc)
            {
                throw new Error.Valence(ValenceErrorText);
            }

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

                if (element != 0 && element != 1)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                this.expandVector.Add((byte)element);
            }
            else
            {
                foreach (AType item in left)
                {
                    if (!item.ConvertToRestrictedWholeNumber(out element))
                    {
                        throw new Error.Type(TypeErrorText);
                    }

                    if (element != 0 && element != 1)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }

                    this.expandVector.Add((byte)element);
                }
            }
        }

        private void PrepareInputItems(AType right)
        {
            if (right.IsArray)
            {
                this.items = right;

                if (this.items.Length != 1 && this.expandVector.Count<byte>(item => { return item == 1; }) != this.items.Length)
                {
                    throw new Error.Length(LengthErrorText);
                }
            }
            else
            {
                this.items = AArray.Create(right.Type, right);
            }
        }

        #endregion

        #region Computation

        private AType Compute()
        {
            AType result = AArray.Create(ATypes.AType);
            int index = 0;

            // Get the filler element based on the right argument
            AType fillElementShape = 
                this.items.Rank > 1 ? this.items.Shape.GetRange(1, this.items.Shape.Count - 1).ToAArray() : null;
            AType filler = Utils.FillElement(this.items.Type, fillElementShape);

            for (int i = 0; i < this.expandVector.Count; i++)
            {
                if (this.expandVector[i] == 1)
                {
                    result.AddWithNoUpdate(items[this.items.Length > 1 ? index++ : 0].Clone());
                }
                else
                {
                    result.AddWithNoUpdate(filler.Clone());
                }
            }

            result.Length = this.expandVector.Count;
            result.Shape = new List<int>() { this.expandVector.Count };

            if (items.Rank > 1)
            {
                result.Shape.AddRange(items.Shape.GetRange(1, items.Shape.Count - 1));
            }

            result.Rank = items.Rank;
            result.Type = result.Length > 0 ? result[0].Type : (this.items.MixedType() ? ATypes.ANull : this.items.Type);

            return result;
        }

        #endregion
    }
}
