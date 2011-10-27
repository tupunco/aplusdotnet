using System;
using System.Diagnostics;
using System.Reflection;

using AplusCore.Compiler.Grammar;
using AplusCore.Runtime;
using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents an standalone built-in function in an A+ AST.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This node represents a single built-in function if it
    /// is not used for an operator or dyadic/monadic function.
    /// </para>
    /// <para>
    /// This node will build a DLR lambda expression for the given built-in function.
    /// </para>
    /// </remarks>
    public class BuiltInFunction : Node
    {
        #region Variables

        private Token function;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.BuiltInFunction; }
        }

        /// <summary>
        /// Gets the dyadic/monadic token witch represents the wrapped function.
        /// </summary>
        public Token Function
        {
            get { return this.function; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="BuiltInFunction"/> AST node.
        /// </summary>
        /// <param name="function">The <see cref="AST.Token"/> to wrap.</param>
        public BuiltInFunction(Token function)
        {
            this.function = function;
        }

        #endregion

        #region DLR generator

        private static DLR.Expression BuildDyadicCase(
            Token functionToken,
            DLR.LabelTarget returnTarget,
            DLR.Expression environment,
            DLR.Expression rightParam,
            DLR.Expression leftParam)
        {
            DLR.Expression result;
            MethodInfo method = typeof(AbstractDyadicFunction).GetMethod("Execute");

            if (functionToken.Type == Tokens.TYPE)
            {
                result = DLR.Expression.IfThenElse(
                    // $left.IsNumber || ($left.Type == ATypes.ANull)
                    DLR.Expression.OrElse(
                        DLR.Expression.IsTrue(
                            DLR.Expression.PropertyOrField(leftParam, "IsNumber")
                        ),
                        DLR.Expression.Equal(
                            DLR.Expression.PropertyOrField(leftParam, "Type"),
                            DLR.Expression.Constant(ATypes.ANull)
                        )
                    ),
                    // Or($right, $left)
                    DLR.Expression.Goto(
                        returnTarget,
                        DLR.Expression.Call(
                            DLR.Expression.Constant(DyadicFunctionInstance.Or),
                            method,
                            rightParam, leftParam, environment
                        )
                    ),
                    // Cast($right, $left)
                    DLR.Expression.Goto(
                        returnTarget,
                        DLR.Expression.Call(
                            DLR.Expression.Constant(DyadicFunctionInstance.Cast),
                            method,
                            rightParam, leftParam, environment
                        )
                    )
                );
            }
            else
            {
                MethodChooser.ConvertToDyadicToken(functionToken);
                AbstractDyadicFunction dyadic = MethodChooser.GetDyadicMethod(functionToken);

                if (dyadic != null)
                {
                    result =
                        DLR.Expression.Goto(
                            returnTarget,
                            DLR.Expression.Call(
                                DLR.Expression.Constant(dyadic),
                                method,
                                rightParam,
                                leftParam,
                                environment
                            )
                        );
                }
                else
                {
                    result =
                        DLR.Expression.Throw(
                            DLR.Expression.New(
                                typeof(Error.Valence).GetConstructor(new Type[] { typeof(string) }),
                                DLR.Expression.Constant(functionToken.Text)
                            )
                        );
                }
            }

            return result;
        }

        private static DLR.Expression BuildMonadicCase(
            Token functionToken,
            DLR.LabelTarget methodReturnTarget,
            DLR.ParameterExpression methodEnvArg,
            DLR.ParameterExpression methodRightArg)
        {
            DLR.Expression result;
            AbstractMonadicFunction monadic = MethodChooser.GetMonadicMethod(functionToken);

            if (monadic != null)
            {
                result = DLR.Expression.Goto(
                    methodReturnTarget,
                    DLR.Expression.Call(
                        DLR.Expression.Constant(monadic),
                        monadic.GetType().GetMethod("Execute"),
                        methodRightArg,
                        methodEnvArg
                    )
                );
            }
            else
            {
                result = DLR.Expression.Throw(
                    DLR.Expression.New(
                        typeof(Error.Valence).GetConstructor(new Type[] { typeof(string) }),
                        DLR.Expression.Constant(functionToken.Text)
                    )
                );
            }

            return result;
        }

        public override DLR.Expression Generate(AplusScope scope)
        {
            string methodName = String.Format("__built-in({0})__", this.function.Text);

            DLR.ParameterExpression methodEnvArg = DLR.Expression.Parameter(typeof(Aplus), "_ENV_ARG_");
            DLR.ParameterExpression methodLeftArg = DLR.Expression.Parameter(typeof(AType), "_LEFT_ARG_");
            DLR.ParameterExpression methodRightArg = DLR.Expression.Parameter(typeof(AType), "_RIGHT_ARG_");
            DLR.LabelTarget methodReturnTarget = DLR.Expression.Label(typeof(AType), "_RESULT_");

            DLR.Expression codeBlock = DLR.Expression.Block(
                DLR.Expression.IfThenElse(
                    DLR.Expression.Equal(methodLeftArg, DLR.Expression.Constant(null)),
                    BuildMonadicCase(this.function, methodReturnTarget, methodEnvArg, methodRightArg),
                    BuildDyadicCase(this.function, methodReturnTarget, methodEnvArg, methodRightArg, methodLeftArg)

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

            DLR.Expression result = DLR.Expression.Call(
                typeof(AFunc).GetMethod("CreateBuiltIn"),
                DLR.Expression.Constant(this.function.Text),
                lambda,
                DLR.Expression.Constant(this.function.Text),
                DLR.Expression.Constant(false)
            );

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return string.Format("<Builtin: {0}>", this.Function);
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Build a AST node which represents a standalone built-in function.
        /// </summary>
        /// <param name="token">The <see cref="AST.Token"/> to build the function from.</param>
        /// <returns><see cref="AST.BuiltInFunction"/> AST node.</returns>
        public static BuiltInFunction BuiltInFunction(Token token)
        {
            return new BuiltInFunction(token);
        }
    }

    #endregion
}
