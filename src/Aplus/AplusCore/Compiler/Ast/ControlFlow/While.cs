using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a while control statment in an A+ AST.
    /// </summary>
    public class While : Node
    {
        #region Variables

        private Node expression;
        private Node codeBlock;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.While; }
        }

        /// <summary>
        /// Gets the expression of the <see cref="While"/> node.
        /// </summary>
        public Node Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the codeblock of the <see cref="While"/> node.
        /// </summary>
        public Node CodeBlock
        {
            get { return this.codeBlock; }
        }

        #endregion

        #region Constructor

        /// <summary>
        ///  Initializes a new instance of <see cref="While"/> AST Node.
        /// </summary>
        /// <param name="expression">The expression for the node.</param>
        /// <param name="codeBlock">The codeblock of the node.</param>
        public While(Node expression, Node codeBlock)
        {
            this.expression = expression;
            this.codeBlock = codeBlock;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            // This parameter will store the return value
            DLR.ParameterExpression returnValue = DLR.ParameterExpression.Parameter(typeof(AType), "RETURN_VALUE");
            DLR.LabelTarget breakLabel = DLR.Expression.Label(typeof(AType), "BREAK");

            DLR.Expression result = DLR.Expression.Block(
                new DLR.ParameterExpression[] { returnValue },
                // Assign the default return value: ANull
                DLR.Expression.Assign(returnValue, DLR.Expression.Constant(Utils.ANull())),
                DLR.Expression.Loop(
                    DLR.Expression.Block(
                        DLR.Expression.IfThen(
                // Test if the condition is equal to 0
                // Invert the inner test's result 
                            DLR.Expression.Not(
                // This part will test if the condition is true (this means it is not 0)
                                DLR.Expression.IsTrue(
                                    this.expression.Generate(scope),
                                    typeof(Helpers).GetMethod("BooleanTest")
                                )
                            ),
                // Break out from the loop with the last computed value
                            DLR.Expression.Break(breakLabel, returnValue)
                        ),
                // Compute & assign the value
                        DLR.Expression.Assign(returnValue, this.codeBlock.Generate(scope))
                    ),
                // The label where to jump in case of break
                    breakLabel
                )
            );


            return result;
        }


        #endregion

        #region overrides

        public override string ToString()
        {
            return string.Format("WHILE({0} {1})", this.expression, this.codeBlock);
        }

        public override bool Equals(object obj)
        {
            if (obj is While)
            {
                While other = (While)obj;
                return (this.expression == other.expression) && (this.codeBlock == other.codeBlock);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.expression.GetHashCode() ^ this.codeBlock.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds a <see cref="While"/> node.
        /// </summary>
        /// <param name="expression">The expression for the <see cref="While"/> node.</param>
        /// <param name="codeBlock">The codeblock for the node.</param>
        /// <returns>Returns a <see cref="While"/> AST node.</returns>
        public static While While(Node expression, Node codeBlock)
        {
            return new While(expression, codeBlock);
        }
    }

    #endregion
}
