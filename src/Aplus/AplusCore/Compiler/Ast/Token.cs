using System;
using AplusCore.Compiler.Grammar;

namespace AplusCore.Compiler.AST
{
    public class Token : Node
    {
        #region Private Fields

        private string text;
        private Tokens tokenType;

        #endregion

        #region Constructor

        public Token(Tokens tokenType, string text = null)
        {
            this.tokenType = tokenType;
            this.text = text;
        }

        #endregion

        #region Properties

        public Tokens Type
        {
            get { return this.tokenType; }
            set { this.tokenType = value; }
        }
        public string Text { get { return this.text; } }

        #endregion

        #region Overridden

        public override string ToString()
        {
            return string.Format("<Token: {0}({1})>", this.tokenType, this.text);
        }

        public override int GetHashCode()
        {
            return this.tokenType.GetHashCode() ^ this.text.GetHashCode();
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

        #region GraphViz output (Only in DEBUG)
#if DEBUG
        static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder text)
        {
            string name = String.Format("Token{0}", counter++);
            text.AppendFormat(" {0} [label=\"{1}\"]", name, this.ToString());
            return this.ToString();
        }
#endif
        #endregion
    }

    #region Construction helper
    public partial class Node
    {
        public static Token Token(Tokens tokentype, string text = null)
        {
            return new Token(tokentype, text);
        }

    }
    #endregion
}
