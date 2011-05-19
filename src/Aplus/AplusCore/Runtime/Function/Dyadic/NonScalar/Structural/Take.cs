using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Take : AbstractDyadicFunction
    {
        #region Variables

        private int desiredcount;
        private AType items;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            PrepareDesiredCount(left);
            PrepareInputItems(right);

            return Compute();
        }

        #endregion

        #region Computation

        private AType Compute()
        {
            AType result = AArray.Create(items.Type);

            if (this.desiredcount > 0)
            {
                // Check how many we can copy from the right argument
                int count = (this.desiredcount <= this.items.Length) ? this.desiredcount : this.items.Length;
                int remainder = this.desiredcount - count;

                for (int i = 0; i < count; i++)
                {
                    result.AddWithNoUpdate(this.items[i].Clone());
                }

                // Check if there is some leftover we need to fill
                if (remainder > 0)
                {
                    AType filler = FillElement();

                    for (; remainder > 0; remainder--)
                    {
                        result.AddWithNoUpdate(filler.Clone());
                    }
                }
            }
            else
            {
                // set the start point, which is the difference between the length of the items and
                //  the count we want to take from the end of the list
                // NOTE: + is used because in this case the 'desiredcount' variable is a negative number
                int start = this.items.Length + this.desiredcount;

                if (start < 0)
                {
                    // This case we need to add fill elements to the start of the array

                    AType filler = FillElement();
                    for(;start < 0; start++)
                    {
                        result.AddWithNoUpdate(filler.Clone());
                    }
                    // Now the 'start' is 0
                }

                for (; start < this.items.Length; start++)
                {
                    result.AddWithNoUpdate(this.items[start].Clone());
                }
            }

            result.Length = Math.Abs(this.desiredcount);
            result.Shape = new List<int>() { result.Length };

            if (this.items.Rank > 1)
            {
                result.Shape.AddRange(this.items.Shape.GetRange(1, this.items.Shape.Count - 1));
            }
            result.Rank = this.items.Rank;

            if (this.desiredcount == 0)
            {
                result.Type = this.items.MixedType() ? ATypes.ANull : this.items.Type;
            }
            else
            {
                result.Type = result[0].Type;
            }

            return result;
        }

        /// <summary>
        /// Get the filler element
        /// </summary>
        /// <remarks>
        /// if the result is a simple vector, simply return the element
        /// otherwise create an array from the filler element with the required shape
        /// </remarks>
        /// <returns></returns>
        private AType FillElement()
        {
            // Input is a vector, so return a simple element
            if (this.items.Rank == 1)
            {
                return Utils.FillElement(this.items.Type);
            }
            else // Input is not a vector, reshape the filler element
            {
                return Utils.FillElement(this.items.Type, this.items.Shape.GetRange(1, items.Shape.Count - 1).ToAArray());
            }
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Convert the input argument to an array.
        /// </summary>
        /// <param name="argument"></param>
        private void PrepareInputItems(AType argument)
        {
            if (argument.Type == ATypes.ANull)
            {
                this.items = AArray.Create(ATypes.ABox, ABox.Create(argument));
            }
            else if (argument.IsArray)
            {
                this.items = argument;
            }
            else
            {
                this.items = AArray.Create(argument.Type, argument);
            }
        }

        private void PrepareDesiredCount(AType element)
        {
            if (element.Type == ATypes.AFloat || element.Type == ATypes.AInteger)
            {
                AType scalar;

                // Get the first scalar value with length check on
                if (!element.TryFirstScalar(out scalar, true))
                {
                    throw new Error.Nonce(this.NonceErrorText);
                }

                // Check if the scalar is a whole number and set the desired count of items
                if (!scalar.ConvertToRestrictedWholeNumber(out this.desiredcount))
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
    }
}
