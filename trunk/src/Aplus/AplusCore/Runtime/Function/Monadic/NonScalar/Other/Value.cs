using System;

using Microsoft.Scripting.Utils;

using AplusCore.Compiler;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class Value : AbstractMonadicFunction
    {
        #region DLR entry point

        public override AType Execute(AType argument, AplusEnvironment environment)
        {
            // Environment is required!
            Assert.NotNull(environment);

            if (!argument.SimpleSymbolArray())
            {
                throw new Error.Type(this.TypeErrorText);
            }

            if (argument.Rank != 0)
            {
                throw new Error.Rank(this.RankErrorText);
            }

            // Get the context parts, (context, variablename) string pairs
            string[] contextParts = VariableHelper.CreateContextParts(environment.Runtime.CurrentContext, argument.asString);

            // Build the method
            Func<AType> method = VariableHelper.BuildVariableAccessMethod(environment, contextParts).Compile();

            return method();
        }

        #endregion

        #region Assignment Helper

        public static AType Assign(AType target, AType value, AplusEnvironment environment)
        {
            // Environment is required!
            Assert.NotNull(environment);

            if ((!target.SimpleSymbolArray()) || (target.Rank != 0))
            {
                throw new Error.Domain("assign");
            }

            // Get the context parts, (context, variablename) string pairs
            string[] contextParts = VariableHelper.CreateContextParts(environment.Runtime.CurrentContext, target.asString);

            // Build the method
            Func<AType> method = VariableHelper.BuildVariableAssignMethod(environment, contextParts, value).Compile();

            return method();
        }

        #endregion
    }
}
