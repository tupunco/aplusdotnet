using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Expand : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            byte[] expandVector = PrepareExpandVector(left);
            AType items = PrepareInputItems(right, expandVector);
            return Compute(items, expandVector);
        }

        #endregion

        #region Preparation

        private byte[] PrepareExpandVector(AType left)
        {
            // if the left side is User defined function, we throw Valence error.
            // this part belongs to Scan.
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

            //int element;
            AType scalar;

            byte[] expandVector;

            if (left.TryFirstScalar(out scalar, true))
            {
                expandVector = new byte[] { ExtractExpandArgument(scalar) };
            }
            else
            {
                expandVector = left.Select(item => ExtractExpandArgument(item)).ToArray();
            }

            return expandVector;
        }

        /// <summary>
        /// Extract the byte representation from the given number, if it is correct.
        /// </summary>
        /// <param name="number"></param>
        /// <returns>0 or 1.</returns>
        private byte ExtractExpandArgument(AType number)
        {
            int result;

            if (!number.ConvertToRestrictedWholeNumber(out result))
            {
                throw new Error.Type(TypeErrorText);
            }

            if (result != 0 && result != 1)
            {
                throw new Error.Domain(DomainErrorText);
            }

            return (byte)result;
        }

        private AType PrepareInputItems(AType input, byte[] expandVector)
        {
            AType result;

            if (input.IsArray)
            {
                if (input.Length != 1 &&
                    expandVector.Count<byte>(item => { return item == 1; }) != input.Length)
                {
                    throw new Error.Length(LengthErrorText);
                }

                result = input;
            }
            else
            {
                result = AArray.Create(input.Type, input);
            }

            return result;
        }

        #endregion

        #region Computation

        private AType Compute(AType items, byte[] expandVector)
        {
            AType result = AArray.Create(ATypes.AType);
            int index = 0;

            // get the filler element based on the right argument
            AType fillElementShape =
                items.Rank > 1 ? items.Shape.GetRange(1, items.Shape.Count - 1).ToAArray() : null;
            AType filler = Utils.FillElement(items.Type, fillElementShape);

            for (int i = 0; i < expandVector.Length; i++)
            {
                if (expandVector[i] == 1)
                {
                    result.AddWithNoUpdate(items[items.Length > 1 ? index++ : 0].Clone());
                }
                else
                {
                    result.AddWithNoUpdate(filler.Clone());
                }
            }

            result.Length = expandVector.Length;
            result.Shape = new List<int>() { expandVector.Length };

            if (items.Rank > 1)
            {
                result.Shape.AddRange(items.Shape.GetRange(1, items.Shape.Count - 1));
            }

            result.Rank = items.Rank;
            result.Type = result.Length > 0 ? result[0].Type : (items.MixedType() ? ATypes.ANull : items.Type);

            return result;
        }

        #endregion
    }
}
