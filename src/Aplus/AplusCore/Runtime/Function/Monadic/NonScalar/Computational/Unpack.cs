using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Computational
{
    class Unpack : AbstractMonadicFunction
    {
        #region Variables

        private int lastDimension;

        #endregion

        #region Entry point

        public override AType Execute(AType argument, Aplus environment = null)
        {
            return Compute(argument);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Find longest symbol in the argument.
        /// </summary>
        /// <param name="argument"></param>
        private void DetermineLastDimension(AType argument)
        {
            if (argument.Rank > 0)
            {
                foreach (AType item in argument)
                {
                    DetermineLastDimension(item);
                }
            }
            else
            {
                this.lastDimension = Math.Max(this.lastDimension, argument.asString.Length);
            }
        }

        #endregion

        #region Computation

        /// <summary>
        /// Checks and call convert algorithm.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Compute(AType argument)
        {
            if (!argument.SimpleSymbolArray())
            {
                throw new Error.Type(TypeErrorText);
            }

            if (argument.Rank >= 9)
            {
                throw new Error.MaxRank(MaxRankErrorText);
            }

            this.lastDimension = int.MinValue;

            DetermineLastDimension(argument);

            return CreateCharArray(argument);
        }

        /// <summary>
        /// Convert Symbol constant array to Character constant array.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType CreateCharArray(AType argument)
        {
            if (argument.Rank > 0)
            {
                AType result = AArray.Create(ATypes.AChar);

                foreach (AType item in argument)
                {
                    result.AddWithNoUpdate(CreateCharArray(item));
                }

                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape);
                result.Shape.Add(this.lastDimension);
                result.Rank = argument.Rank + 1;

                return result;
            }
            else
            {
                return Convert(argument);
            }
        }

        /// <summary>
        /// Convert Symbol to Character constant vector.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private AType Convert(AType symbol)
        {
            string item = symbol.asString;
            AType result = AArray.Create(ATypes.AChar);

            for (int i = 0; i < this.lastDimension; i++)
            {
                result.AddWithNoUpdate(AChar.Create(i >= item.Length ? ' ' : item[i]));
            }

            result.Length = this.lastDimension;
            result.Shape = new List<int>() { this.lastDimension };
            result.Rank = 1;

            return result;
        }

        #endregion
    }
}
