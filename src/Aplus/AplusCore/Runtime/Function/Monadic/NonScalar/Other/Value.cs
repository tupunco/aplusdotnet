﻿using System;

using Microsoft.Scripting.Utils;

using AplusCore.Compiler;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class Value : AbstractMonadicFunction
    {
        #region DLR entry point

        public override AType Execute(AType argument, Aplus environment)
        {
            // Environment is required!
            Assert.NotNull(environment);

            CheckArgument<Value>(argument);

            // Get the context parts, (context, variablename) string pairs
            string[] contextParts = VariableHelper.CreateContextParts(environment.CurrentContext, argument.asString);

            // Build the method
            Func<AType> method = VariableHelper.BuildVariableAccessMethod(environment, contextParts).Compile();

            return method();
        }

        #endregion

        #region Assignment Helper

        public static AType Assign(AType target, AType value, Aplus environment)
        {
            // Environment is required!
            Assert.NotNull(environment);

            if ((!target.SimpleSymbolArray()) || (target.Rank != 0))
            {
                throw new Error.Domain("assign");
            }

            // Get the context parts, (context, variablename) string pairs
            string[] contextParts = VariableHelper.CreateContextParts(environment.CurrentContext, target.asString);

            // Build the method
            Func<AType> method = VariableHelper.BuildVariableAssignMethod(environment, contextParts, value).Compile();

            return method();
        }

        #endregion

        #region Argument check

        internal static void CheckArgument<T>(AType argument) where T : class
        {
            if (!argument.SimpleSymbolArray())
            {
                throw new Error.Type(typeof(T).Name);
            }

            if (argument.Rank != 0)
            {
                throw new Error.Rank(typeof(T).Name);
            }
        }

        #endregion
    }
}
