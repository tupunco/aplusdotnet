using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Ravel : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, Aplus environment = null)
        {
            AType result = AArray.Create(argument.Type);

            if (argument.IsArray)
            {
                ExtractItems(argument, result);

                result.Length = argument.Shape.Product();
                result.Shape = new List<int>() { result.Length };
                result.Rank = 1;
            }
            else
            {
                result.Add(argument);
            }

            return result;
        }

        #endregion

        #region Computation

        /// <summary>
        /// ExtractItems from the <see cref="argument"/> and add it to the <see cref="result"/> vector.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="result">The array stores the result vector</param>
        private void ExtractItems(AType argument, AType result)
        {
            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    ExtractItems(item, result);
                }
            }
            else
            {
                result.AddWithNoUpdate(argument);
            }
        }

        #endregion
    }
}
