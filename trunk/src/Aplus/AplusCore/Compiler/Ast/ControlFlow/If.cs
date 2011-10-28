using System;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents an if/if-else control statement in an A+ AST.
    /// </summary>
    public class If : Node
    {
        #region Variables

        private Node expression;
        private Node trueCase;
        private Node falseCase;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.If; }
        }

        /// <summary>
        /// Gets the expression of the <see cref="If"/> node.
        /// </summary>
        public Node Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the true case of the <see cref="If"/> node.
        /// </summary>
        public Node TrueCase
        {
            get { return this.trueCase; }
        }

        /// <summary>
        /// Gets the false case of the <see cref="If"/> node.
        /// </summary>
        public Node FalseCase
        {
            get { return this.falseCase; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="If">if/if-else</see> AST node.
        /// </summary>
        /// <param name="expression">The expression for the if/if-else node.</param>
        /// <param name="trueCase">The true case of the node.</param>
        /// <param name="falseCase">the false case of the node.</param>
        public If(Node expression, Node trueCase, Node falseCase)
        {
            this.expression = expression;
            this.trueCase = trueCase;
            this.falseCase = falseCase;
        }

        #endregion

        #region Methods

        public void AddFalseCase(Node falseCase)
        {
            this.falseCase = falseCase;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            // generate the following code:
            //  ($expression ? ($truecase) : ($falsecase))

            DLR.Expression result =
                DLR.Expression.Condition(
                    DLR.Expression.IsTrue(
                        this.expression.Generate(scope),
                        typeof(Helpers).GetMethod("BooleanTest")
                    ),
                    this.trueCase.Generate(scope),
                    this.falseCase.Generate(scope)
            );

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return string.Format("IF({0} {1}) ELSE({2})", this.expression, this.trueCase, this.falseCase);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is If))
            {
                return false;
            }
            If other = (If)obj;

            bool conditionOk = this.expression.Equals(other.expression);
            bool trueCaseOk = this.trueCase.Equals(other.trueCase);
            bool falseCaseOk = this.falseCase.Equals(other.falseCase);

            return conditionOk && trueCaseOk && falseCaseOk;

        }

        public override int GetHashCode()
        {
            return this.expression.GetHashCode() ^ this.trueCase.GetHashCode() ^ this.falseCase.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds an <see cref="If">if-else</see> node.
        /// </summary>
        /// <param name="expression">The expression for the node.</param>
        /// <param name="trueCase">The true case of the node.</param>
        /// <param name="falseCase">The false case of the node.</param>
        /// <returns>Returns a <see cref="If">if-else</see> node.</returns>
        public static If IfElse(Node expression, Node trueCase, Node falseCase)
        {
            return new If(expression, trueCase, falseCase);
        }

        /// <summary>
        /// Builds an <see cref="If">if-else</see> node, with a ANull as false case.
        /// </summary>
        /// <param name="expression">The expression for the node.</param>
        /// <param name="trueCase">The true case of the node.</param>
        /// <returns>Returns a <see cref="If">if</see> node, with a ANull as false case.</returns>
        public static If If(Node expression, Node trueCase)
        {
            return new If(expression, trueCase, Node.NullConstant());
        }
    }

    #endregion
}
