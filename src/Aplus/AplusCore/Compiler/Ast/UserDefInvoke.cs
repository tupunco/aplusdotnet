using System;
using System.Collections.Generic;
using System.Diagnostics;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a invocation of a user defined function in an A+ AST.
    /// </summary>
    public class UserDefInvoke : Node
    {
        #region Variables

        private Identifier method;
        private ExpressionList arguments;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.UserDefInvoke; }
        }

        /// <summary>
        /// Gets the identifier of the user defined function.
        /// </summary>
        public Identifier Method
        {
            get { return this.method; }
        }

        /// <summary>
        /// Gets the arguments of the user defined function.
        /// </summary>
        public ExpressionList Arguments
        {
            get { return this.arguments; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="UserDefInvoke"/> AST node.
        /// </summary>
        /// <param name="method">The identifier of the user defined function to invoke.</param>
        /// <param name="arguments">The arguments for the user defined function.</param>
        public UserDefInvoke(Identifier method, ExpressionList arguments)
        {
            this.method = method;
            this.arguments = arguments;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            Aplus runtime = scope.GetRuntime();

            // arguments for the dynamic method call
            LinkedList<DLR.Expression> callArguments = new LinkedList<DLR.Expression>();

            if (scope.IsAssignment)
            {
                // Generate the paramters differently for assignment

                // f{...;x} := value  <=> i := f{...; iota rho x}; (,x)[i] := value

                List<Node> items = new List<Node>(this.arguments.Items);
                // 2. Add the parameters in !reverse! order except the last one
                for (int i = 0; i < items.Count - 1; i++)
                {
                    callArguments.AddFirst(items[i].Generate(scope));
                }

                Node lastItem = items[items.Count - 1];
                bool isPick = Node.TestDyadicToken(lastItem, Grammar.Tokens.PICK);
                bool isRavel = Node.TestMonadicToken(lastItem, Grammar.Tokens.RAVEL);

                if (!(lastItem is Identifier || isPick || isRavel))
                {
                    throw new ParseException("user-def invoke assign", false);
                }

                DLR.Expression indexList = AST.Assign.BuildIndicesList(scope, lastItem.Generate(scope));

                callArguments.AddFirst(indexList);
            }
            else
            {
                // 2. Add the parameters in !reverse! order
                foreach (Node item in this.arguments.Items)
                {
                    callArguments.AddFirst(item.Generate(scope));
                }
            }

            // 0. Add A+ environment as first argument for user defined functions
            callArguments.AddFirst(scope.GetRuntimeExpression());

            // 1. Construct the method body
            callArguments.AddFirst(this.method.Generate(scope));

            DLR.Expression result;
            if (scope.IsAssignment)
            {
                // (,x)[f{...;iota rho x}]
                result = AST.Assign.BuildIndexing(
                    scope,
                    this.arguments.Items.Last.Value.Generate(scope),
                    BuildInvoke(runtime, callArguments)
                );
            }
            else
            {
                // 3. Dynamic invoke of method
                // Order of arguments:
                //  (method, Enviroment, argN, ... arg1, arg0)
                result = BuildInvoke(runtime, callArguments);
            }

            return result;
        }

        internal static DLR.Expression BuildInvoke(Aplus runtime, ICollection<DLR.Expression> callArguments)
        {
            DLR.Expression result = DLR.Expression.Convert(
                DLR.Expression.Dynamic(
                    runtime.InvokeBinder(new DYN.CallInfo(callArguments.Count - 1)),
                    typeof(object),
                    callArguments
                ),
                typeof(AType)
            );
            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("FunctionInvoke({0} {1})", this.method.ToString(), this.arguments.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj is UserDefInvoke)
            {
                UserDefInvoke other = (UserDefInvoke)obj;
                return (this.method == other.method) && (this.arguments == other.arguments);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.method.GetHashCode() ^ this.arguments.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds a <see cref="UserDefInvoke">user defined function invoke</see> node.
        /// </summary>
        /// <param name="method">The identifier of the user defined function to invoke.</param>
        /// <param name="arguments">The arguments for the user defined function.</param>
        /// <returns>Returns a <see cref="UserDefInvoke"> node.</returns>
        public static UserDefInvoke UserDefInvoke(Node method, Node arguments)
        {
            Debug.Assert(method is Identifier);
            Debug.Assert(arguments is ExpressionList);
            return new UserDefInvoke((Identifier)method, (ExpressionList)arguments);
        }
    }

    #endregion

}
