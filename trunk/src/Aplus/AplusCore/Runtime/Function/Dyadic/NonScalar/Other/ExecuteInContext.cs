using System;

using Microsoft.Scripting.Utils;

using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class ExecuteInContext : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment)
        {
            // Environment is required!
            Assert.NotNull(environment);

            if (right.Type != ATypes.AChar)
            {
                throw new Error.Type(this.TypeErrorText);
            }

            if (right.Rank > 1)
            {
                throw new Error.Rank(this.RankErrorText);
            }

            string sourceCode = right.ToString();
            AType result;

            if(left.Type == ATypes.ASymbol)
            {
                AType symbol;
                if(left.TryFirstScalar(out symbol))
                {
                    result = ExecuteWithContextSwitch(environment, sourceCode, symbol.asString);
                }
                else
                {
                    result = ProtectedExecute(environment, sourceCode);
                }
            }
            else if (left.Type == ATypes.AInteger || left.Type == ATypes.ANull)
            {
                result = ProtectedExecute(environment, sourceCode);
            }
            else
            {
                throw new Error.Type(this.TypeErrorText);
            }

            return result;
        }

        #endregion

        #region Execute In Context

        /// <summary>
        /// Executes the <paramref name="sourceCode"/> in the <paramref name="newContext"/>.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="sourceCode">The code to execute</param>
        /// <param name="newContext">The context name to switch to before executeing the code</param>
        /// <returns>The executed source code's result</returns>
        private static AType ExecuteWithContextSwitch(AplusEnvironment environment, string sourceCode, string newContext)
        {
            AType result;

            string oldContext = environment.Runtime.CurrentContext;
            environment.Runtime.CurrentContext = newContext;

            DLR.Expression<Func<AplusEnvironment, AType>> lambda =
                Function.Monadic.NonScalar.Other.ExecuteFunction.BuildExecuteMethod(sourceCode, environment);

            Func<AplusEnvironment, AType> method = lambda.Compile();

            result = method(environment);

            environment.Runtime.CurrentContext = oldContext;
            return result;
        }

        #endregion

        #region Protected Execute

        /// <summary>
        /// Executes the <paramref name="sourceCode"/> in the current context and catches errors (like monadic-do)
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="sourcecode">The code to execute</param>
        /// <returns>
        ///  1) An AInteger if there is an error, this integer is the number of the error
        ///  2) The executed source code's result enclosed in an ABox
        /// </returns>
        private static AType ProtectedExecute(AplusEnvironment environment, string sourcecode)
        {
            AType result;

            try
            {
                DLR.Expression<Func<AplusEnvironment, AType>> lambda =
                    Function.Monadic.NonScalar.Other.ExecuteFunction.BuildExecuteMethod(sourcecode, environment);
                Func<AplusEnvironment, AType> method = lambda.Compile();

                // Enclose the result
                result = ABox.Create(method(environment));
            }
            catch (Error error)
            {
                result = AInteger.Create((int)error.ErrorType);
            }

            return result;
        }

        #endregion
    }
}
