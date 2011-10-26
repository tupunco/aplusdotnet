using System;

using AplusCore.Compiler.Grammar;
using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a built-in monadic function in an A+ AST.
    /// </summary>
    public class MonadicFunction : Node
    {
        #region Variables

        private Token token;
        private Node expression;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the built-in <see cref="Token"/> for the monadic function.
        /// </summary>
        public new Token Token
        {
            get { return this.token; } 
        }

        /// <summary>
        /// Gets the argument for the monadic function.
        /// </summary>
        public Node Expression
        {
            get { return this.expression; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="MonadicFunction"/> AST node.
        /// </summary>
        /// <param name="token">The <see cref="Token"/> to use for the monadic function.</param>
        /// <param name="expression">The argument of the monadic function.</param>
        public MonadicFunction(Token token, Node expression)
        {
            this.token = token;
            this.expression = expression;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;

            if (scope.IsAssignment && TokenUtils.AllowedPrimitiveFunction(this.token.Type))
            {
                /*
                 * input: x -> right side, value
                 * Perform the function like this:
                 * 
                 * i := f{iota rho x}
                 * (,x)[i] := value
                 * 
                 * where 'f' is the monadic function
                 */
                DLR.Expression target = Node.TestMonadicToken(this.expression, Tokens.RAVEL)
                        ? ((MonadicFunction)this.expression).Expression.Generate(scope)
                        : this.expression.Generate(scope)
                    ;

                // i:=(iota rho x)
                DLR.Expression indices = AST.Assign.BuildIndicesList(scope, target);
                // (,x)[f{i}]
                result = AST.Assign.BuildIndexing(scope, target, GenerateMonadic(scope, indices));
            }
            else
            {
                result = GenerateMonadic(scope, this.expression.Generate(scope));
            }

            return result;
        }

        private DLR.Expression GenerateMonadic(AplusScope scope, DLR.Expression argument)
        {
            DLR.Expression result;
            // Handle the result monadic function a little differently:
            if (this.token.Type == Tokens.RESULT)
            {
                result = scope.ReturnTarget != null 
                    // If inside of a function create a return expression Tree
                    ? DLR.Expression.Return(scope.ReturnTarget, argument, typeof(AType))
                    // Otherwise just return the value
                    : argument
                ;
            }
            else
            {
                AbstractMonadicFunction method = MethodChooser.GetMonadicMethod(this.token);

                if (method == null)
                {
                    throw new ParseException(String.Format("Not supported Monadic function[{0}]", this.token));
                }

                result = DLR.Expression.Call(
                    DLR.Expression.Constant(method),
                    method.GetType().GetMethod("Execute"),
                    argument,
                    scope.GetRuntimeExpression()
                );
            }

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Monadic({0} {1})", this.token, this.expression);
        }

        public override bool Equals(object obj)
        {
            if (obj is MonadicFunction)
            {
                MonadicFunction other = (MonadicFunction)obj;
                return this.token.Equals(other.token) && this.expression.Equals(other.expression);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.token.GetHashCode() ^ this.expression.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds a <see cref="Node"/> representing a built-in monadic function.
        /// </summary>
        /// <param name="token">The <see cref="Token"/> to use for the monadic function.</param>
        /// <param name="expression">The argument of the monadic function.</param>
        /// <returns>Returns a <see cref="MonadicFunction"/> representing a built-in function.</returns>
        public static Node MonadicFunction(Token token, Node expression)
        {
            return new MonadicFunction(token, expression);
        }
    }

    #endregion
}
