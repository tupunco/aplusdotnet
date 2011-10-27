using System;

using AplusCore.Compiler.Grammar;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public enum NodeTypes
    {
        Undefined,
        Assign,
        BuiltInFunction,
        BuiltInOperator,
        Case,
        Constant,
        ConstantList,
        Dependency,
        DyadicDo,
        DyadicFunction,
        EachOperator,
        ExpressionList,
        Identifier,
        If,
        Indexing,
        MonadicDo,
        MonadicFunction,
        RankOperator,
        Strand,
        SystemCommand,
        Token,
        UserDefFunction,
        UserDefOperator,
        UserDefOperatorInvoke,
        UserDefInvoke,
        While,
    }

    /// <summary>
    /// Represents a Node in an A+ AST.
    /// </summary>
    /// <remarks>
    /// Basic Ast Node point. All other Nodes subclasses this.
    /// Also every Node should extend with it's own construction helper method.
    /// This is possible, because the Node is a partial class.
    /// </remarks>
    public partial class Node
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public virtual NodeTypes NodeType
        {
            get { return NodeTypes.Undefined; }
        }

        #endregion

        #region Construction helper

        /// <summary>
        /// Builds a Node representing a <see cref="MonadicFunction"/> or a <see cref="DyadicFunction"/> based on the
        /// number of arguments in the <paramref name="expressionList"/> parameter.
        /// </summary>
        /// <remarks>
        /// Returned nodes based on the ExpressionList parameter (number of nodes inside the list):
        ///  - 1: MonadicFunction
        ///  - 2: DyadicFunction
        ///  - other cases: Create parse valence error
        /// </remarks>
        /// <param name="token">The <see cref="Token"/> representing the built-in function.</param>
        /// <param name="expressionList">List of arguments for the built-in function.</param>
        /// <exception cref="ParseException">Throws exception if the number of arguments is incorrect.</exception>
        /// <returns></returns>
        public static Node BuiltinInvoke(Token token, ExpressionList expressionList)
        {
            switch (expressionList.Length)
            {
                case 1:
                    return new MonadicFunction(token, expressionList[0]);
                case 2:
                    return new DyadicFunction(token, expressionList[0], expressionList[1]);

                default:
                    throw new ParseException("valence?", false);
            }
        }

        /// <summary>
        /// Updates the properties of the <see cref="Operator"/> based on the <paramref name="expressionList"/> parameter.
        /// </summary>
        /// <param name="op">The <see cref="Operator"/> to update.</param>
        /// <param name="expressionList">The list of arguments wrapped in an <see cref="ExpressionList"/>.</param>
        /// <exception cref="ParseException"></exception>
        /// <returns>Returns the updated <see cref="Operator"/>.</returns>
        public static Node BuiltinOpInvoke(Operator op, ExpressionList expressionList)
        {
            switch (expressionList.Length)
            {
                case 1:
                    op.RightArgument = expressionList[0];
                    break;
                case 2:
                    op.RightArgument = expressionList[1];
                    op.LeftArgument = expressionList[0];
                    break;
                default:
                    if (!(op is EachOperator))
                    {
                        throw new ParseException("Valence", false);
                    }

                    EachOperator eachOp = (EachOperator)op;
                    eachOp.IsGeneralApply = true;
                    eachOp.RightArgument = expressionList;
                    break;
            }

            return op;
        }

        #endregion

        #region Helper utils

        /// <summary>
        /// Test if the <see cref="Node"/> is a <see cref="DyadicFunction"/> node with the specified token type.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> to check.</param>
        /// <param name="tokenType">The <see cref="Tokens">token type</see> to check for.</param>
        /// <remarks>
        /// True if the <see cref="Node"/> is a 
        /// <see cref="DyadicFunction"/> with the given <see cref="Tokens">token type</see>.
        /// Otherwise false.
        /// </remarks>
        internal static bool TestDyadicToken(Node node, Tokens tokenType)
        {
            return (node is DyadicFunction) && (((DyadicFunction)node).Token.Type == tokenType);
        }

        /// <summary>
        /// Test if the <see cref="Node"/> is a <see cref="MonadicFunction"/> node with the specified token type.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> to check.</param>
        /// <param name="tokenType">The <see cref="Tokens">token type</see> to check for.</param>
        /// <remarks>
        /// True if the <see cref="Node"/> is a 
        /// <see cref="MonadicFunction"/> with the given <see cref="Tokens">token type</see>.
        /// Otherwise false.
        /// </remarks>
        internal static bool TestMonadicToken(Node node, Tokens tokenType)
        {
            return (node is MonadicFunction) && (((MonadicFunction)node).Token.Type == tokenType);
        }

        #endregion

        #region DLR Generator

        /// <summary>
        /// Generate the DLR expression tree for the AST node
        /// </summary>
        /// <param name="scope"></param>
        /// <returns>DLR expression tree</returns>
        public virtual DLR.Expression Generate(AplusScope scope)
        {
            throw new NotImplementedException("Should !NEVER! reach this point. If this shown, then not all AST Nodes overridden this method!");
        }

        #endregion

        #region overrides

        /// <summary>
        /// Node is not equal to anything!
        /// </summary>
        /// <param name="obj">object to check</param>
        /// <returns>false</returns>
        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ 0xDEAD;
        }

        #endregion

        #region Operator overloads

        public static bool operator ==(Node left, Node right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !(left == right);
        }

        #endregion
    }
}
