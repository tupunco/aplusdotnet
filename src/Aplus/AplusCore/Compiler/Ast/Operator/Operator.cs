namespace AplusCore.Compiler.AST
{
    public abstract class Operator : Node
    {
        #region Variables

        protected Node function;
        protected Node leftarg;
        protected Node rightarg;

        protected Token opToken;

        #endregion

        #region Constructor

        public Operator(Node function)
        {
            this.function = function;
            this.leftarg = null;
            this.rightarg = null;
        }

        #endregion

        #region Properties

        public bool isDyadic { get { return this.leftarg is Node; } }
        public bool isBuiltin { get { return this.function is Token; } }

        public Node Function
        {
            get { return this.function; }
            set { this.function = value; }
        }

        public Token OperatorToken
        {
            get { return this.opToken; }
            set { this.opToken = value; }
        }

        public Node LeftArgument
        {
            get { return this.leftarg; }
            set { this.leftarg = value; }
        }

        public Node RightArgument
        {
            get { return this.rightarg; }
            set { this.rightarg = value; }
        }

        #endregion
    }
}
