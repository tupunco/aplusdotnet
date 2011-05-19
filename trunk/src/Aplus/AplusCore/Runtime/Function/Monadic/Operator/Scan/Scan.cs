using System.Collections.Generic;

using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Scan
{
    abstract class Scan : AbstractMonadicFunction
    {
        #region Variables

        protected AbstractDyadicFunction function;
        protected ATypes type;

        #endregion

        #region Abstract method

        /// <summary>
        /// This method sets the variables.
        /// </summary>
        /// <param name="type"></param>
        protected abstract void SetVariables(AType argument);

        #endregion

        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            SetVariables(argument);

            AType result;

            //First dimension equal with zero case (as Null).
            if (argument.Length == 0)
            {
                result = argument.Clone();
                result.Type = this.type;
            }
            else
            {
                //Accepted types are float and integer.
                if (argument.Type != ATypes.AFloat && argument.Type != ATypes.AInteger)
                {
                    throw new Error.Type(TypeErrorText);
                }

                result = argument.IsArray ? ScanAlgorithm(argument) : PreProcess(argument);
            }

            return result;
        }

        #endregion

        #region Computation

        /// <summary>
        /// </summary>
        /// <remarks>
        /// (function\argument)[i] == function/argument[iota i + 1] for every valid i.
        /// </remarks>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ScanAlgorithm(AType argument)
        {
            AType result = AArray.Create(this.type);

            AType temp = PreProcess(argument[0]);

            result.AddWithNoUpdate(temp);

            for (int i = 1; i < argument.Length; i++)
            {
                temp = this.function.Execute(argument[i], temp);
                result.AddWithNoUpdate(temp);
            }

            result.Length = argument.Length;
            result.Shape = new List<int>(argument.Shape);
            result.Rank = argument.Rank;

            return PostProcess(argument, result);
        }

        protected virtual AType PreProcess(AType argument)
        {
            return argument.Clone();
        }

        protected virtual AType PostProcess(AType argument, AType result)
        {
            return result;
        }

        /// <summary>
        /// Convert argument to integer.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected AType ConvertToInt(AType argument)
        {
            if (argument.IsArray)
            {
                AType result = AArray.Create(ATypes.AInteger);

                foreach (AType item in argument)
                {
                    result.AddWithNoUpdate(ConvertToInt(item));
                }

                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape);
                result.Rank = argument.Rank;

                return result;
            }
            else
            {
                int result;
                if(argument.ConvertToRestrictedWholeNumber(out result))
                {
                    return AInteger.Create(result);
                }
                else
                {
                    throw new Error.Type(TypeErrorText);
                }
            }
        }

        #endregion

    }
}
