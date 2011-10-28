using System;

using AplusCore.Compiler.Grammar;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a token in an A+ AST.
    /// </summary>
    public class Token : Node
    {
        #region Variables

        private string text;
        private Tokens tokenType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.Token; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Tokens">type</see> of the token.
        /// </summary>
        public Tokens Type
        {
            get { return this.tokenType; }
            set { this.tokenType = value; }
        }

        /// <summary>
        /// Gets the string representation of the token.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Token"/> AST node.
        /// </summary>
        /// <param name="tokenType">The <see cref="Tokens">type</see> of the token.</param>
        /// <param name="text">The string representation of the token.</param>
        public Token(Tokens tokenType, string text = null)
        {
            this.tokenType = tokenType;
            this.text = text;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            return Node.BuiltInFunction(this).Generate(scope);
        }

        #endregion

        #region Overridden

        public override string ToString()
        {
            return string.Format("<Token: {0}({1})>", this.tokenType, this.text);
        }

        public override int GetHashCode()
        {
            return this.tokenType.GetHashCode() ^ (this.text != null ? this.text.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            if (obj is Token)
            {
                Token other = (Token)obj;
                // Token's type check is enough, we don't care about the string representation
                return (this.tokenType == other.tokenType)/* && (this.text == other.text)*/;
            }

            return false;
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds a <see cref="Token"/>.
        /// </summary>
        /// <param name="tokentype">The <see cref="Tokens">type</see> of the token.</param>
        /// <param name="text">The string representation of the token.</param>
        /// <returns>Returns a <see cref="Token"/>.</returns>
        public static Token Token(Tokens tokentype, string text = null)
        {
            return new Token(tokentype, text);
        }
    }

    #endregion
}
