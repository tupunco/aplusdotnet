using System;
using AplusCore.Compiler.Grammar;
using DLR = System.Linq.Expressions;
using AplusCore.Runtime;
using AplusCore.Types;
using DYN = System.Dynamic;
using AplusCore.Runtime.Function.Monadic;

namespace AplusCore.Compiler.AST
{
    public class MonadicFunction : Node
    {
        #region Variables

        private Token token;
        private Node expression;

        #endregion

        #region Properties

        public Tokens TokenType { get { return this.token.Type; } }
        public Node Expression { get { return this.expression; } }

        #endregion

        #region Constructor

        public MonadicFunction(Token token, Node expression)
        {
            this.token = token;
            this.expression = expression;
        }

        #endregion

        #region Overrides

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
                    scope.GetAplusEnvironment()
                );
            }

            return result;
        }

        #endregion

        #region GraphViz output (Only in DEBUG)
#if DEBUG
        static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder textBuilder)
        {
            string name = String.Format("Monadic{0}", counter++);
            string exprName = this.expression.ToDot(name, textBuilder);

            textBuilder.AppendFormat("  {0} [label=\"{1} ({2})\"];\n", name, this.token.Text, this.token.Type.ToString());
            textBuilder.AppendFormat("  {0} -> {1};\n", name, exprName);

            return name;
        }
#endif
        #endregion

    }

    #region Construction helper
    public partial class Node
    {

        public static Node MonadicFunction(Token token, Node expression)
        {
            return new MonadicFunction(token, expression);
        }

    }
    #endregion
}
