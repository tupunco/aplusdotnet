using System;
using System.Collections.Generic;

using AplusCore.Runtime;
using AplusCore.Runtime.Function.Operator.Dyadic;
using AplusCore.Runtime.Function.Operator.Monadic;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class EachOperator : Operator
    {
        #region Variables

        private bool isgeneralapply;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.EachOperator; }
        }

        public bool IsGeneralApply
        {
            get { return this.isgeneralapply; }
            set { this.isgeneralapply = value; }
        }

        #endregion

        #region Constructor

        public EachOperator(Node function)
            : base(function)
        {
            this.isgeneralapply = false;
        }

        #endregion

        #region DLR generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression func, result;

            if (this.function is Token)
            {
                Node wrappedFunction = new BuiltInFunction((Token)this.function);
                func = wrappedFunction.Generate(scope);
            }
            else
            {
                func = this.function.Generate(scope);
            }

            DLR.ParameterExpression environment = scope.GetRuntimeExpression();

            DLR.ParameterExpression functionParam = DLR.Expression.Variable(typeof(AType), "$$functionParam");
            DLR.ParameterExpression rightParam = DLR.Expression.Variable(typeof(AType), "$$rightParam");
            DLR.ParameterExpression valueParam = DLR.Expression.Variable(typeof(AType), "$$valueParam");

            // TODO: rewrite this
            if (this.IsGeneralApply)
            {
                ExpressionList argumnets = (ExpressionList)this.rightarg;
                LinkedList<DLR.Expression> callArguments = new LinkedList<DLR.Expression>();

                // 2. Add the parameters in !reverse! order
                foreach (Node item in argumnets.Items)
                {
                    callArguments.AddFirst(item.Generate(scope));
                }

                // 0. Add A+ environment as first argument for user defined functions
                callArguments.AddFirst(environment);

                // 1. Construct the method body
                callArguments.AddFirst(functionParam.Property("NestedItem"));

                result = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { functionParam, valueParam },
                    DLR.Expression.Assign(functionParam, func),
                    DLR.Expression.IfThenElse(
                            DLR.Expression.PropertyOrField(functionParam, "IsFunctionScalar"),
                            DLR.Expression.Assign(
                                valueParam,
                                AST.UserDefInvoke.BuildInvoke(scope.GetRuntime(), callArguments)
                            ),
                            DLR.Expression.Throw(
                                DLR.Expression.New(
                                    typeof(Error.Valence).GetConstructor(new Type[] { typeof(string) }),
                                    DLR.Expression.Constant("apply")
                                )
                            )
                        ),
                    valueParam
                    );
            }
            else if (isDyadic)
            {
                DLR.Expression right = this.rightarg.Generate(scope);
                DLR.Expression left = this.leftarg.Generate(scope);
                DLR.ParameterExpression leftParam = DLR.Expression.Variable(typeof(AType), "$$leftParam");

                result = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { functionParam, rightParam, leftParam, valueParam },
                    DLR.Expression.Assign(functionParam, func),
                    DLR.Expression.Assign(rightParam, right),
                    DLR.Expression.Assign(leftParam, left),
                    DLR.Expression.IfThenElse(
                        DLR.Expression.IsTrue(
                            DLR.Expression.PropertyOrField(functionParam, "IsFunctionScalar")
                        ),
                         DLR.Expression.Assign(
                            valueParam,
                             DLR.Expression.Call(
                                 DLR.Expression.Constant(DyadicOperatorInstance.Apply),
                                 DyadicOperatorInstance.Apply.GetType().GetMethod("Execute"),
                                 functionParam, rightParam, leftParam, environment
                             )
                         ),
                         DLR.Expression.Assign(
                            valueParam,
                             DLR.Expression.Call(
                                 DLR.Expression.Constant(DyadicOperatorInstance.Each),
                                 DyadicOperatorInstance.Each.GetType().GetMethod("Execute"),
                                 functionParam, rightParam, leftParam, environment
                             )
                        )
                    ),
                    valueParam
                 );
            }
            else
            {
                DLR.Expression right = this.rightarg.Generate(scope);
                result = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { functionParam, rightParam, valueParam },
                    DLR.Expression.Assign(functionParam, func),
                    DLR.Expression.Assign(rightParam, right),
                    DLR.Expression.IfThenElse(
                        DLR.Expression.IsTrue(
                            DLR.Expression.PropertyOrField(functionParam, "IsFunctionScalar")
                        ),
                         DLR.Expression.Assign(
                            valueParam,
                             DLR.Expression.Call(
                                 DLR.Expression.Constant(MonadicOperatorInstance.Apply),
                                 MonadicOperatorInstance.Apply.GetType().GetMethod("Execute"),
                                 functionParam, rightParam, environment
                             )
                         ),
                         DLR.Expression.Assign(
                            valueParam,
                             DLR.Expression.Call(
                                 DLR.Expression.Constant(MonadicOperatorInstance.Each),
                                 MonadicOperatorInstance.Each.GetType().GetMethod("Execute"),
                                 functionParam, rightParam, environment
                             )
                        )
                    ),
                    valueParam
                 );
            }

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            if (isDyadic)
            {
                return String.Format("Each({0} {1} {2}", this.function, this.leftarg, this.rightarg);
            }
            else
            {
                return String.Format("Each({0} {1}", this.function, this.rightarg);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is EachOperator)
            {
                EachOperator other = (EachOperator)obj;
                bool result = (this.function == other.function) && (this.rightarg == other.rightarg);
                if (isDyadic)
                {
                    result = result && (this.leftarg == other.leftarg);
                }
                return result;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int value = this.function.GetHashCode() ^ this.rightarg.GetHashCode();
            if (isDyadic)
            {
                value ^= this.leftarg.GetHashCode();
            }

            return value;
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static EachOperator EachOperator(Node function)
        {
            return new EachOperator(function);
        }

        public static EachOperator EachOperator(Node function, Node expresson)
        {
            EachOperator op = EachOperator(function);
            op.RightArgument = expresson;
            return op;
        }

        public static EachOperator EachOperator(Node function, Node leftExpression, Node rightExpresson)
        {
            EachOperator op = EachOperator(function);
            op.RightArgument = rightExpresson;
            op.LeftArgument = leftExpression;
            return op;
        }

        public static EachOperator EachOperator(Token opToken)
        {
            EachOperator op = new EachOperator(null);
            op.OperatorToken = opToken;
            return op;
        }
    }

    #endregion
}
