using System;

using AplusCore.Compiler.Grammar;
using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class DyadicFunction : Node
    {
        #region Variables

        private Token token;
        private Node leftExpression;
        private Node rightExpression;

        #endregion

        #region Properties

        public new Token Token { get { return this.token; } }
        public Tokens TokenType { get { return this.token.Type; } }
        public Node Left { get { return this.leftExpression; } }
        public Node Right { get { return this.rightExpression; } }

        #endregion

        #region Constructor

        public DyadicFunction(Token token, Node leftExpression, Node rightExpression)
        {
            this.token = token;
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;

            MethodChooser.ConvertToDyadicToken(this.token);
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;

            if (scope.IsAssignment && TokenUtils.AllowedPrimitiveFunction(this.token.Type))
            {
                /*
                 * input: y -> left side, x -> right side, value
                 * Perform the function like this:
                 * 
                 * i := f{y; iota rho x}
                 * (,x)[i] := value
                 * 
                 * where 'f' is the dyadic function
                 */
                DLR.Expression left = this.leftExpression.Generate(scope);
                DLR.Expression right = Node.TestMonadicToken(this.rightExpression, Tokens.RAVEL)
                    ? ((MonadicFunction)this.rightExpression).Expression.Generate(scope)
                    : this.rightExpression.Generate(scope)
                ;

                // i:=(iota rho x)
                DLR.Expression indices = AST.Assign.BuildIndicesList(scope, right);
                // (,x)[f{a;i}]
                result = AST.Assign.BuildIndexing(scope, right, GenerateDyadic(scope, indices, left));
            }
            else if (scope.IsAssignment && this.token.Type == Tokens.CHOOSE)
            {
                DLR.Expression left = this.leftExpression.Generate(scope);
                DLR.Expression right = this.rightExpression.Generate(scope);

                result = DLR.Expression.Call(
                    DLR.Expression.Constant(DyadicFunctionInstance.Choose),
                    DyadicFunctionInstance.Choose.GetType().GetMethod("Assign"),
                    right, left, scope.GetRuntimeExpression()
                );
            }
            else
            {
                DLR.Expression left = this.leftExpression.Generate(scope);
                DLR.Expression right = this.rightExpression.Generate(scope);
                result = GenerateDyadic(scope, right, left);
            }

            return result;
        }

        private DLR.Expression GenerateDyadic(AplusScope scope, DLR.Expression right, DLR.Expression left)
        {
            DLR.Expression result;

            DLR.ParameterExpression environment = scope.GetRuntimeExpression();

            if (this.token.Type == Tokens.OR)
            {
                DLR.ParameterExpression leftParam = DLR.Expression.Variable(typeof(AType), "$$leftParam");
                DLR.ParameterExpression rightParam = DLR.Expression.Variable(typeof(AType), "$$rightParam");
                DLR.ParameterExpression valueParam = DLR.Expression.Variable(typeof(AType), "$$valueParam");

                result = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { leftParam, rightParam, valueParam },
                    DLR.Expression.Assign(rightParam, right),
                    DLR.Expression.Assign(leftParam, left),
                    DLR.Expression.IfThenElse(
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
                        DLR.Expression.Assign(
                            valueParam,
                            DLR.Expression.Call(
                                DLR.Expression.Constant(DyadicFunctionInstance.Or),
                                DyadicFunctionInstance.Or.GetType().GetMethod("Execute"),
                                rightParam, leftParam, environment
                            )
                        ),
                        // Cast($right, $left)
                        DLR.Expression.Assign(
                            valueParam,
                            DLR.Expression.Call(
                                DLR.Expression.Constant(DyadicFunctionInstance.Cast),
                                DyadicFunctionInstance.Cast.GetType().GetMethod("Execute"),
                                rightParam, leftParam, environment
                            )
                        )
                    ),
                    valueParam
                 );
            }
            else
            {
                AbstractDyadicFunction method = MethodChooser.GetDyadicMethod(this.token);

                if (method == null)
                {
                    throw new ParseException(String.Format("Not supported Dyadic function[{0}]", this.token));
                }

                result = DLR.Expression.Call(
                    DLR.Expression.Constant(method),
                    method.GetType().GetMethod("Execute"),
                    right, left, environment
                );
            }

            return result;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return String.Format("Dyadic({0} {1} {2})", this.token, this.leftExpression, this.rightExpression);
        }

        public override bool Equals(object obj)
        {
            if (obj is DyadicFunction)
            {
                DyadicFunction other = (DyadicFunction)obj;
                var tokenOk = this.token.Equals(other.token);
                var leftOk = this.leftExpression.Equals(other.leftExpression);
                var rightOK = this.rightExpression.Equals(other.rightExpression);

                return tokenOk && leftOk && rightOK;

            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.token.GetHashCode() ^ this.leftExpression.GetHashCode() ^ this.rightExpression.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static Node DyadicFunction(Token token, Node leftExpression, Node rightExpression)
        {
            return new DyadicFunction(token, leftExpression, rightExpression);
        }
    }

    #endregion
}
