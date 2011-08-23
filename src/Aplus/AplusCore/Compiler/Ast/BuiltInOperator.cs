using System;

using AplusCore.Runtime;
using AplusCore.Runtime.Function.Operator.Dyadic;
using AplusCore.Runtime.Function.Operator.Monadic;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class BuiltInOperator : Node
    {
        #region Variables

        private Operator op;

        #endregion

        #region Properties

        internal Operator Operator { get { return this.op; } }

        #endregion

        #region Constructor

        public BuiltInOperator(Operator op)
        {
            this.op = op;
        }

        #endregion

        #region DLR generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;

            if (this.op.Function is Token || this.op.Function is Identifier || this.op.Function is Operator ||
                this.op.Function is BuiltInOperator)
            {
                string methodName = String.Format("__built-in({0})__", this.op.ToString());

                DLR.ParameterExpression functionVariable = DLR.Expression.Variable(typeof(AType), "__function__");
                DLR.ParameterExpression methodEnvArg = DLR.Expression.Parameter(typeof(Aplus), "_ENV_ARG_");
                DLR.ParameterExpression methodLeftArg = DLR.Expression.Parameter(typeof(AType), "_LEFT_ARG_");
                DLR.ParameterExpression methodRightArg = DLR.Expression.Parameter(typeof(AType), "_RIGHT_ARG_");
                DLR.LabelTarget methodReturnTarget = DLR.Expression.Label(typeof(AType), "_RESULT_");

                DLR.Expression opFunction = GenerateOpFunction(scope, op);

                DLR.Expression codeBlock = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { functionVariable },
                    DLR.Expression.Assign(functionVariable, opFunction),
                    DLR.Expression.IfThenElse(
                        DLR.Expression.Equal(methodLeftArg, DLR.Expression.Constant(null)),
                        BuildMonadicCase(scope, this.op,
                            functionVariable, methodReturnTarget, methodEnvArg, methodRightArg
                        ),
                        BuildDyadicCase(scope, this.op,
                            functionVariable, methodReturnTarget, methodEnvArg, methodRightArg, methodLeftArg
                        )
                    ),
                    DLR.Expression.Label(methodReturnTarget, DLR.Expression.Constant(default(AType), typeof(AType)))
                );

                DLR.Expression lambda = DLR.Expression.Lambda<Func<Aplus, AType, AType, AType>>(
                    codeBlock,
                    methodName,
                    new DLR.ParameterExpression[] {
                        methodEnvArg,
                        methodRightArg,
                        methodLeftArg
                    }
                );

                result = DLR.Expression.Call(
                    typeof(AFunc).GetMethod("CreateBuiltIn"),
                    DLR.Expression.Constant(this.op.ToString()),
                    lambda,
                    DLR.Expression.Constant(lambda.ToString()),
                    DLR.Expression.Constant(true)
                );
            }
            else
            {
                // This will allow: (3;4) each
                // which will return the (3;4)
                result = this.op.Function.Generate(scope);
            }

            return result;
        }

        private static DLR.Expression GenerateOpFunction(AplusScope scope, Operator op)
        {
            DLR.Expression function;
            if (op.isBuiltin)
            {
                function = AST.Node.BuiltInFunction(op.Function).Generate(scope);
            }
            else
            {
                function = op.Function.Generate(scope);
            }

            return function;
        }

        private static DLR.Expression BuildDyadicCase(
            AplusScope scope,
            Operator op,
            DLR.ParameterExpression functionVariable,
            DLR.LabelTarget methodReturnTarget,
            DLR.ParameterExpression methodEnvArg,
            DLR.ParameterExpression methodRightArg,
            DLR.ParameterExpression methodLeftArg)
        {
            DLR.Expression result;
            if (op is EachOperator)
            {
                result = 
                    DLR.Expression.IfThenElse(
                        DLR.Expression.Property(functionVariable, "IsFunctionScalar"),
                        DLR.Expression.Goto(
                            methodReturnTarget,
                            DLR.Expression.Call(
                                DLR.Expression.Constant(DyadicOperatorInstance.Apply),
                                DyadicOperatorInstance.Apply.GetType().GetMethod("Execute"),
                                functionVariable,
                                methodRightArg,
                                methodLeftArg,
                                methodEnvArg
                            )
                        ),
                        DLR.Expression.Goto(
                            methodReturnTarget,
                            DLR.Expression.Call(
                                DLR.Expression.Constant(DyadicOperatorInstance.Each),
                                DyadicOperatorInstance.Each.GetType().GetMethod("Execute"),
                                functionVariable,
                                methodRightArg,
                                methodLeftArg,
                                methodEnvArg
                            )
                        )
                    );
            }
            else
            {
                result = DLR.Expression.Goto(
                    methodReturnTarget,
                    DLR.Expression.Call(
                        DLR.Expression.Constant(DyadicOperatorInstance.Rank),
                        DyadicOperatorInstance.Rank.GetType().GetMethod("Execute"),
                        functionVariable,
                        ((RankOperator)op).Condition.Generate(scope),
                        methodRightArg,
                        methodLeftArg,
                        methodEnvArg
                    )
                );
            }

            return result;
        }


        private static DLR.Expression BuildMonadicCase(
            AplusScope scope,
            Operator op,
            DLR.ParameterExpression functionVariable,
            DLR.LabelTarget methodReturnTarget,
            DLR.ParameterExpression methodEnvArg,
            DLR.ParameterExpression methodRightArg)
        {
            DLR.Expression result;
            if (op is EachOperator)
            {

                result =
                    DLR.Expression.IfThenElse(
                        DLR.Expression.Property(functionVariable, "IsFunctionScalar"),
                        DLR.Expression.Goto(
                            methodReturnTarget,
                            DLR.Expression.Call(
                                DLR.Expression.Constant(MonadicOperatorInstance.Apply),
                                MonadicOperatorInstance.Apply.GetType().GetMethod("Execute"),
                                functionVariable,
                                methodRightArg,
                                methodEnvArg
                            )
                        ),
                        DLR.Expression.Goto(
                            methodReturnTarget,
                            DLR.Expression.Call(
                                DLR.Expression.Constant(MonadicOperatorInstance.Each),
                                MonadicOperatorInstance.Each.GetType().GetMethod("Execute"),
                                functionVariable,
                                methodRightArg,
                                methodEnvArg
                            )
                        )
                    );
            }
            else
            {
                result = DLR.Expression.Goto(
                    methodReturnTarget,
                    DLR.Expression.Call(
                        DLR.Expression.Constant(MonadicOperatorInstance.Rank),
                        MonadicOperatorInstance.Rank.GetType().GetMethod("Execute"),
                        functionVariable,
                        ((RankOperator)op).Condition.Generate(scope),
                        methodRightArg,
                        methodEnvArg
                    )
                );
            }

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("<BuiltIn: {0}>", this.op.ToString());
        }

        #endregion
    }

    #region Construction Helper

    public partial class Node
    {
        public static BuiltInOperator BuiltInOperator(Node op)
        {
            return new BuiltInOperator((Operator)op);
        }
    }

    #endregion
}
