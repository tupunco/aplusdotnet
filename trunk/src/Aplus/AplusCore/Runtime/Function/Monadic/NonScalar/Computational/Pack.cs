using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Computational
{
    class Pack : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            if (argument.Type != ATypes.AChar)
            {
                throw new Error.Type(TypeErrorText);
            }

            return Compute(argument);
        }

        #endregion

        #region Computation

        /// <summary>
        /// Convert Character constant array to symbol array.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Compute(AType argument)
        {
            //If argument is character constant or character constant vector then we convert it symbol,
            //and cut blanks from end.
            if (argument.Rank <= 1)
            {
                return ASymbol.Create(argument.ToString().TrimEnd());
            }
            else
            {
                AType result = AArray.Create(ATypes.ASymbol);

                foreach (AType item in argument)
                {
                    result.AddWithNoUpdate(Compute(item));
                }

                result.Length = argument.Length;
                result.Shape = new List<int>();
                result.Shape.AddRange(argument.Shape.GetRange(0, argument.Shape.Count - 1));
                result.Rank = argument.Rank - 1;

                return result;
            }
        }

        #endregion
    }
}
