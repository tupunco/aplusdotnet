using System.Collections.Generic;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    public class UserDefOperatorInvoke : Operator
    {
        #region Variables

        private Identifier name;
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

        public UserDefOperatorInvoke(Identifier name)
            : base(null)
        {
            this.name = name;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            Aplus runtime = scope.GetRuntime();

            // arguments for the dynamic method call
            LinkedList<DLR.Expression> callArguments = new LinkedList<DLR.Expression>();

            // add the parameters in !reverse! order
            if (this.leftarg != null)
            {
                callArguments.AddFirst(this.leftarg.Generate(scope));
            }

            callArguments.AddFirst(this.function.Generate(scope));

            if (this.condition != null)
            {
                callArguments.AddFirst(this.condition.Generate(scope));
            }

            callArguments.AddFirst(this.rightarg.Generate(scope));

            // add A+ environment as first argument for user defined functions
            callArguments.AddFirst(scope.GetRuntimeExpression());
            callArguments.AddFirst(this.name.Generate(scope));

            return AST.UserDefInvoke.BuildInvoke(runtime, callArguments);
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return string.Format("UserDefOpInvoke(({0} {1} {2}) {3} {4})",
                this.function, this.name, this.condition, this.leftarg, this.rightarg);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UserDefOperatorInvoke))
            {
                return false;
            }

            UserDefOperatorInvoke opInvoke = (UserDefOperatorInvoke)obj;

            return (this.function == opInvoke.function) && (this.name == opInvoke.name)
                && (this.condition == opInvoke.condition)
                && (this.leftarg == opInvoke.leftarg) && (this.rightarg == opInvoke.rightarg);
        }

        public override int GetHashCode()
        {
            int hash = this.function.GetHashCode() ^ this.name.GetHashCode() ^ this.rightarg.GetHashCode();

            if (this.leftarg != null)
            {
                hash ^= this.leftarg.GetHashCode();
            }

            if (this.condition != null)
            {
                hash ^= this.condition.GetHashCode();
            }

            return hash;
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static UserDefOperatorInvoke UserDefOperatorInvoke(Identifier name)
        {
            return new UserDefOperatorInvoke(name);
        }

        public static UserDefOperatorInvoke UserDefOperatorInvoke(Identifier name, Node condition)
        {
            UserDefOperatorInvoke userOp = new UserDefOperatorInvoke(name);
            userOp.Condition = condition;
            return userOp;
        }
    }

    #endregion
}