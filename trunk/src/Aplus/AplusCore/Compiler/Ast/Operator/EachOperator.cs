using System;

using AplusCore.Runtime;
using AplusCore.Runtime.Function.Operator.Dyadic;
using AplusCore.Runtime.Function.Operator.Monadic;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class EachOperator : Operator
    {
        #region Constructor

        public EachOperator(Node function)
            : base(function)
        {
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

            DLR.Expression right = this.rightarg.Generate(scope);
            DLR.ParameterExpression environment = scope.GetAplusEnvironment();

            DLR.ParameterExpression functionParam = DLR.Expression.Variable(typeof(AType), "$$functionParam");
            DLR.ParameterExpression rightParam = DLR.Expression.Variable(typeof(AType), "$$rightParam");
            DLR.ParameterExpression valueParam = DLR.Expression.Variable(typeof(AType), "$$valueParam");

            if (isDyadic)
            {
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

        #region GraphViz output (Only in DEBUG)

#if DEBUG
        static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder text)
        {
            string name = String.Format("EachOP{0}", counter++);
            string funcDot = this.function.ToDot(name, text);
            text.AppendFormat(" {0} -> {1};\n", name, funcDot);

            if (isDyadic)
            {
                string leftArg = this.leftarg.ToDot(name, text);
                text.AppendFormat("  {0} -> {1};\n", name, leftArg);
            }

            if (rightarg != null)
            {
                string rightArg = this.rightarg.ToDot(name, text);
                text.AppendFormat(" {0} -> {1};\n", name, rightArg);
                text.AppendFormat(" {0} [label=\"{1} Each\"]", name, this.function);
            }

            return name;
        }
#endif

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
