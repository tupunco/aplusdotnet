namespace AplusCore.Compiler.AST
{
    public class UserDefOperatorInvoke : Operator
    {
        #region Variables

        private Node name;
        private Node condition;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.UserDefOperatorInvoke; }
        }

        public Node Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        #endregion

        #region Constructors

        public UserDefOperatorInvoke(Node name)
            : base(null)
        {
            this.name = name;
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static UserDefOperatorInvoke UserDefOperatorInvoke(Node name)
        {
            return new UserDefOperatorInvoke(name);
        }

        public static UserDefOperatorInvoke UserDefOperatorInvoke(Node name, Node condition)
        {
            UserDefOperatorInvoke userOp = new UserDefOperatorInvoke(name);
            userOp.Condition = condition;
            return userOp;
        }
    }

    #endregion
}