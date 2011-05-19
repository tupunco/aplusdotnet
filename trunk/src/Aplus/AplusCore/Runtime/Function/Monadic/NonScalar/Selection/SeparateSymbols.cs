using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Selection
{
    class SeparateSymbols : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            if (argument.Type == ATypes.ANull)
            {
                return argument.Clone();
            }

            if (!argument.SimpleSymbolArray())
            {
                throw new Error.Type(TypeErrorText);
            }

            if (argument.Rank > 8)
            {
                throw new Error.MaxRank(MaxRankErrorText);
            }

            return Walk(argument);
        }

        #endregion

        #region Computation

        /// <summary>
        /// Execute separate symbol operation on each item.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Walk(AType argument)
        {
            if (argument.Rank > 0)
            {
                AType result = AArray.Create(ATypes.ASymbol);

                foreach (AType item in argument)
                {
                    result.AddWithNoUpdate(Walk(item));
                }

                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape);
                result.Shape.Add(2);
                result.Rank = 1 + argument.Rank;

                return result;
            }
            else
            {
                return SeparateSymbol(argument);
            }
        }

        /// <summary>
        /// Separate symbol constant by definiton.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType SeparateSymbol(AType argument)
        {
            AType result = AArray.Create(ATypes.ASymbol);

            string symbol = argument.asString;

            if (symbol.Contains('.'))
            {
                int index = symbol.LastIndexOf('.');

                result.AddWithNoUpdate(ASymbol.Create(symbol.Substring(0, index)));
                result.AddWithNoUpdate(ASymbol.Create(symbol.Substring(index + 1)));

            }
            else
            {
                result.AddWithNoUpdate(ASymbol.Create(""));
                result.AddWithNoUpdate(argument.Clone());
            }

            result.Length = 2;
            result.Shape = new List<int>() { 2 };
            result.Rank = 1;

            return result;
        }

        #endregion
    }
}
