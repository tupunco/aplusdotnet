using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Ravel : AbstractMonadicFunction
    {
        #region Variables

        private AType result;

        #endregion

        #region Entry point

        public override AType Execute(AType argument, Aplus environment = null)
        {
            if (argument.IsArray)
            {
                result = AArray.Create(argument.Type);
                ExecuteRecursion(argument);

                int length = 1;
                foreach (int item in argument.Shape)
                {
                    length *= item;
                }

                result.Length = length;
                result.Shape = new List<int>() { result.Length };
                result.Rank = 1;

                return result;
            }
            else
            {
                return AArray.Create(argument.Type, argument);
            }
        }

        #endregion

        #region Computation

        /// <summary>
        /// Recursively convert items to a vector
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="result">The array stores the result vector</param>
        private void ExecuteRecursion(AType argument)
        {
            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    ExecuteRecursion(item);
                }
            }
            else
            {
                this.result.AddWithNoUpdate(argument);
            }
        }

        #endregion
    }
}
