using System;
using Microsoft.Scripting.Utils;

using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class ExecuteFunction : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment)
        {
            // Environment is required!
            Assert.NotNull(environment);

            if (argument.Type != ATypes.AChar)
            {
                throw new Error.Type(this.TypeErrorText);
            }

            if (argument.Rank > 1)
            {
                throw new Error.Rank(this.RankErrorText);
            }

            DLR.Expression<Func<AplusEnvironment, AType>> lambda = BuildExecuteMethod(argument.ToString(), environment);
            Func<AplusEnvironment, AType> method = lambda.Compile();

            AType result = method(environment);

            return result;
        }

        #endregion

        #region Method Builder

        internal static DLR.Expression<Func<AplusEnvironment, AType>> BuildExecuteMethod(
            string sourceCode, AplusEnvironment environment
        )
        {
            DLR.Expression codebody;
            AplusCore.Compiler.AST.Node tree = Compiler.Parse.String(sourceCode, environment.Runtime.LexerMode);

            AplusScope scope = new AplusScope(null, "__EVAL__", environment.Runtime,
                DLR.Expression.Parameter(typeof(Runtime.Aplus), "__EVAL_RUNTIME__"),
                DLR.Expression.Parameter(typeof(DYN.IDynamicMetaObjectProvider), "__EVAL_MODULE__"),
                DLR.Expression.Parameter(typeof(AplusEnvironment), "__EVAL_ENVIRONMENT__"),
                DLR.Expression.Label(typeof(AType), "__EVAL_EXIT__"),
                isEval: true
            );

            if (tree == null)
            {
                codebody = DLR.Expression.Constant(null);
            }
            else if (environment.FunctionScope != null)
            {
                AplusScope functionScope = new AplusScope(scope, "__EVAL_IN_FUNCTION__",
                    moduleParam: DLR.Expression.Parameter(typeof(DYN.ExpandoObject), "__EVAL_FUNCTION_SCOPE__"),
                    returnTarget: scope.ReturnTarget,
                    isMethod: true
                );

                codebody = DLR.Expression.Block(
                    new DLR.ParameterExpression[] {
                        scope.RuntimeExpression,         // runtime
                        scope.ModuleExpression,          // root context
                        functionScope.ModuleExpression   // Function local scope
                    },
                    DLR.Expression.Assign(
                        scope.RuntimeExpression, DLR.Expression.PropertyOrField(scope.AplusEnvironment, "Runtime")
                    ),
                    DLR.Expression.Assign(
                        scope.ModuleExpression, DLR.Expression.PropertyOrField(scope.AplusEnvironment, "Context")
                    ),
                    DLR.Expression.Assign(
                        functionScope.ModuleExpression, DLR.Expression.PropertyOrField(scope.AplusEnvironment, "FunctionScope")
                    ),
                    DLR.Expression.Label(
                        scope.ReturnTarget,
                        tree.Generate(functionScope)
                    )
                );
            }
            else
            {
                codebody = DLR.Expression.Block(
                    new DLR.ParameterExpression[] {
                        scope.RuntimeExpression,         // runtime
                        scope.ModuleExpression          // root context
                    },
                    DLR.Expression.Assign(
                       scope.RuntimeExpression, DLR.Expression.PropertyOrField(scope.AplusEnvironment, "Runtime")
                    ),
                    DLR.Expression.Assign(
                        scope.ModuleExpression, DLR.Expression.PropertyOrField(scope.AplusEnvironment, "Context")
                    ),
                    DLR.Expression.Label(
                        scope.ReturnTarget,
                        tree.Generate(scope)
                    )
               );
            }

            DLR.Expression<Func<AplusEnvironment, AType>> lambda = DLR.Expression.Lambda<Func<AplusEnvironment, AType>>(
                codebody,
                scope.GetAplusEnvironment()
            );

            return lambda;
        }

        #endregion

    }
}
