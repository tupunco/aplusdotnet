using System;

using AplusCore.Compiler.Grammar;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Basic Ast Node point. All other Nodes subclasses this.
    /// Also every Node should extend with it's own construction helper method
    /// (this is possible, because the Node base type is a partial class
    /// </summary>
    public partial class Node
    {
        #region Construction helper

        /// <summary>
        /// This method will return a <see cref="DyadicFunction"/> or a <see cref="MonadicFunction"/> node 
        /// based on the number of nodes/expressions in the <see cref="ExpressionList"/> parameter
        /// </summary>
        /// <remarks>
        /// Returned nodes based on the ExpressionList parameter (number of nodes inside the list):
        ///  - 1: MonadicFunction
        ///  - 2: DyadicFunction
        ///  - other cases: Create parse valence error
        /// </remarks>
        /// <param name="token">The token for the builtin method</param>
        /// <param name="expressionList">List of parameters for the builtin method</param>
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
        /// Updates the input Operator's arguments based on the input Expression List.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="expressionList"></param>
        /// <returns></returns>
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
                    throw new ParseException("Valence", false);
            }

            return op;
        }

        #endregion

        #region Helper utils

        /// <summary>
        /// Test if the supplied node is a dyadic node with the specified token
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tokenType"></param>
        /// <returns>True if: node is a DyadicFunction and it's token's type is equal to the one we supplied</returns>
        internal static bool TestDyadicToken(Node node, Tokens tokenType)
        {
            return (node is DyadicFunction) && (((DyadicFunction)node).TokenType == tokenType);
        }

        /// <summary>
        /// Test if the supplied node is a monadic node with the specified token
        /// </summary>
        /// <param name="node"></param>
        /// <param name="tokenType"></param>
        /// <returns>True if: node is a MonadicFunction and it's token's type is equal to the one we supplied</returns>
        internal static bool TestMonadicToken(Node node, Tokens tokenType)
        {
            return (node is MonadicFunction) && (((MonadicFunction)node).TokenType == tokenType);
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
