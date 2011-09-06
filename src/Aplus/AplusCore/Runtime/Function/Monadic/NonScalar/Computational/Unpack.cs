using System;
using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Computational
{
    class Unpack : AbstractMonadicFunction
    {
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
        private int DetermineLastDimension(AType argument)
        {
            int localMax = int.MinValue;

            if (argument.Rank > 0)
            {
                foreach (AType item in argument)
                {
                    localMax = Math.Max(DetermineLastDimension(item), localMax);
                }
            }
            else
            {
                localMax = Math.Max(localMax, argument.asString.Length);
            }

            return localMax;
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

            int lastDimension = DetermineLastDimension(argument);

            return CreateCharArray(argument, lastDimension);
        }

        /// <summary>
        /// Convert Symbol constant array to Character constant array.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType CreateCharArray(AType argument, int lastDimension)
        {
            if (argument.Rank > 0)
            {
                AType result = AArray.Create(ATypes.AChar);

                foreach (AType item in argument)
                {
                    result.AddWithNoUpdate(CreateCharArray(item, lastDimension));
                }

                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape);
                result.Shape.Add(lastDimension);
                result.Rank = argument.Rank + 1;

                return result;
            }
            else
            {
                return Convert(argument, lastDimension);
            }
        }

        /// <summary>
        /// Convert Symbol to Character constant vector.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private AType Convert(AType symbol, int lastDimension)
        {
            string item = symbol.asString;
            AType result = AArray.Create(ATypes.AChar);

            for (int i = 0; i < lastDimension; i++)
            {
                result.AddWithNoUpdate(AChar.Create(i >= item.Length ? ' ' : item[i]));
            }

            result.Length = lastDimension;
            result.Shape = new List<int>() { lastDimension };
            result.Rank = 1;

            return result;
        }

        #endregion
    }
}
