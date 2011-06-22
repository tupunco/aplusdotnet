using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Take : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            int desiredCount = PrepareDesiredCount(left);
            AType items = PrepareInputItems(right);

            return Compute(items, desiredCount);
        }

        #endregion

        #region Computation

        private AType Compute(AType items, int desiredCount)
        {
            AType result = AArray.Create(items.Type);

            if (desiredCount > 0)
            {
                // Check how many we can copy from the right argument
                int count = (desiredCount <= items.Length) ? desiredCount : items.Length;
                int remainder = desiredCount - count;

                for (int i = 0; i < count; i++)
                {
                    result.AddWithNoUpdate(items[i].Clone());
                }

                // Check if there is some leftover we need to fill
                if (remainder > 0)
                {
                    AType filler = FillElement(items);

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
                // NOTE: + is used because in this case the 'desiredCount' variable is a negative number
                int start = items.Length + desiredCount;

                if (start < 0)
                {
                    // This case we need to add fill elements to the start of the array

                    AType filler = FillElement(items);
                    for(;start < 0; start++)
                    {
                        result.AddWithNoUpdate(filler.Clone());
                    }
                    // Now the 'start' is 0
                }

                for (; start < items.Length; start++)
                {
                    result.AddWithNoUpdate(items[start].Clone());
                }
            }

            result.Length = Math.Abs(desiredCount);
            result.Shape = new List<int>() { result.Length };

            if (items.Rank > 1)
            {
                result.Shape.AddRange(items.Shape.GetRange(1, items.Shape.Count - 1));
            }
            result.Rank = items.Rank;

            if (desiredCount == 0)
            {
                result.Type = items.MixedType() ? ATypes.ANull : items.Type;
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
        private AType FillElement(AType items)
        {
            // Input is a vector, so return a simple element
            if (items.Rank == 1)
            {
                return Utils.FillElement(items.Type);
            }
            else // Input is not a vector, reshape the filler element
            {
                return Utils.FillElement(items.Type, items.Shape.GetRange(1, items.Shape.Count - 1).ToAArray());
            }
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Convert the input argument to an array.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType PrepareInputItems(AType argument)
        {
            AType result;

            if (argument.Type == ATypes.ANull)
            {
                result = AArray.Create(ATypes.ABox, ABox.Create(argument));
            }
            else if (argument.IsArray)
            {
                result = argument;
            }
            else
            {
                result = AArray.Create(argument.Type, argument);
            }

            return result;
        }

        private int PrepareDesiredCount(AType element)
        {
            if (element.Type == ATypes.AFloat || element.Type == ATypes.AInteger)
            {
                AType scalar;

                // Get the first scalar value with length check on
                if (!element.TryFirstScalar(out scalar, true))
                {
                    throw new Error.Nonce(this.NonceErrorText);
                }

                int desiredCount;
                // Check if the scalar is a whole number and set the desired count of items
                if (!scalar.ConvertToRestrictedWholeNumber(out desiredCount))
                {
                    throw new Error.Type(this.TypeErrorText);
                }

                return desiredCount;
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
