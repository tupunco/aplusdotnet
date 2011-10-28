using System;
using System.Collections.Generic;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a definition of a user operator in an A+ AST.
    /// </summary>
    public class UserDefOperator : Node
    {
        #region Variables

        private Identifier name;
        private Identifier function;
        private Identifier condition;
        private Identifier leftArgument;
        private Identifier rightArgument;
        private Node codeblock;
        private string codeText;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.UserDefOperator; }
        }

        /// <summary>
        /// Gets the name of the user defined operator.
        /// </summary>
        public Identifier Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> of the function for the user defined operator.
        /// </summary>
        public Identifier Function
        {
            get { return this.function; }
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> of the condition for the user defined operator.
        /// </summary>
        public Identifier Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> of the left argument for the user defined operator.
        /// </summary>
        public Identifier LeftArgument
        {
            get { return this.leftArgument; }
            set { this.leftArgument = value; }
        }

        /// <summary>
        /// Gets the <see cref="Identifier"/> of the right argument for the user defined operator.
        /// </summary>
        public Identifier RightArgument
        {
            get { return this.rightArgument; }
            set { this.rightArgument = value; }
        }

        /// <summary>
        /// Gets the code block of the user defined operator.
        /// </summary>
        public Node Codeblock
        {
            get { return this.codeblock; }
            set { this.codeblock = value; }
        }

        /// <summary>
        /// Gets the string representation of the user defined operator.
        /// </summary>
        public string CodeText
        {
            get { return this.codeText; }
            set { this.codeText = value; }
        }

        /// <summary>
        /// Specifeis if the user defined operator is a dyadic operator.
        /// </summary>
        public bool IsDyadicOperator
        {
            get { return this.condition != null; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="UserDefOperator"/> AST node.
        /// </summary>
        /// <param name="name">The name of the user defined operator.</param>
        /// <param name="function">The function parameter of the user defined operator.</param>
        /// <param name="codeblock">The code block of the user defined operator.</param>
        /// <param name="codeText">The string representation of the user defined operator.</param>
        public UserDefOperator(Identifier name, Identifier function, Node codeblock, string codeText)
        {
            this.name = name;
            this.function = function;
            this.codeblock = codeblock;
            this.codeText = codeText;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            Aplus runtime = scope.GetRuntime();

            if (scope.IsEval && scope.IsMethod)
            {
                // we are inside a function and an eval block.

                // override the original eval scope
                // create a top level scope
                scope = new AplusScope(null, "_EVAL_FUNC_SCOPE_",
                    scope.GetRuntime(),
                    scope.GetRuntimeExpression(),
                    scope.Parent.GetModuleExpression(),
                    scope.ReturnTarget,
                    isEval: true
                );
            }

            string operatorName = this.name.BuildQualifiedName(runtime.CurrentContext);

            string scopename = String.Format("__operator_{0}_scope__", this.name.Name);
            AplusScope methodScope = new AplusScope(scope, scopename,
                runtimeParam: DLR.Expression.Parameter(typeof(Aplus), "_EXTERNAL_RUNTIME_"),
                moduleParam: DLR.Expression.Parameter(typeof(DYN.ExpandoObject), scopename),
                returnTarget: DLR.Expression.Label(typeof(AType), "RETURN"),
                isMethod: true
            );

            // create a result parameter
            DLR.ParameterExpression resultParameter = DLR.Expression.Parameter(typeof(AType), "__RESULT__");

            // create function's parameters
            LinkedList<DLR.ParameterExpression> methodParameters = new LinkedList<DLR.ParameterExpression>();

            // add parameters to the linkedlist
            BuildParameterExpression(methodScope, methodParameters, leftArgument);
            BuildParameterExpression(methodScope, methodParameters, function);
            BuildParameterExpression(methodScope, methodParameters, condition);
            BuildParameterExpression(methodScope, methodParameters, rightArgument);

            // add parameter for AplusEnviroment
            methodParameters.AddFirst(methodScope.RuntimeExpression);

            // create the lambda method for the function
            DLR.LambdaExpression method = DLR.Expression.Lambda(
                DLR.Expression.Block(
                    new DLR.ParameterExpression[] { methodScope.ModuleExpression, resultParameter },
                // add the local scope's store
                    DLR.Expression.Assign(methodScope.ModuleExpression, DLR.Expression.Constant(new DYN.ExpandoObject())),
                // set AplusEnviroment's function scope reference
                    DLR.Expression.Assign(
                        DLR.Expression.Property(methodScope.RuntimeExpression, "FunctionScope"),
                        methodScope.ModuleExpression
                    ),
                // calculate the result of the defined function
                    DLR.Expression.Assign(
                        resultParameter,
                        DLR.Expression.Label(methodScope.ReturnTarget, this.codeblock.Generate(methodScope))
                    ),
                // reset  AplusEnviroment's function scope reference
                    DLR.Expression.Assign(
                        DLR.Expression.Property(methodScope.RuntimeExpression, "FunctionScope"),
                        DLR.Expression.Constant(null, typeof(DYN.ExpandoObject))
                    ),
                // return the result
                    resultParameter
                ),
                operatorName,
                methodParameters
            );

            // wrap the lambda method inside an AFunc
            DLR.Expression wrappedLambda = DLR.Expression.Call(
                typeof(AFunc).GetMethod("CreateUserOperator"),
                DLR.Expression.Constant(operatorName),
                DLR.Expression.Constant(this.IsDyadicOperator),
                method,
                DLR.Expression.Constant(methodParameters.Count),
                DLR.Expression.Constant(this.codeText)
            );

            // set the variable to the lambda function
            DLR.Expression setMethod = VariableHelper.SetVariable(
                runtime,
                scope.ModuleExpression,
                this.name.CreateContextNames(runtime.CurrentContext),
                wrappedLambda
            );

            // ensure the result type to be an AType
            DLR.Expression result = DLR.Expression.Convert(setMethod, typeof(AType));

            return result;
        }

        /// <summary>
        /// Adds the passed parameter's name to the scope, and the parameterexpression to the expression linkedlist.
        /// </summary>
        /// <param name="methodScope">The scope.</param>
        /// <param name="methodParameters">The LinkedList to add.</param>
        /// <param name="parameter">The parameter to add.</param>
        private static void BuildParameterExpression(
            AplusScope methodScope, LinkedList<DLR.ParameterExpression> methodParameters, Identifier parameter)
        {
            if (parameter != null)
            {
                string name = parameter.Name;
                DLR.ParameterExpression expression = DLR.Expression.Parameter(typeof(AType), name);
                methodScope.Variables[name] = expression;

                methodParameters.AddFirst(expression);
            }
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return string.Format("OperatorDef({0} {1} {2} {3} {4} {5})",
                this.name, this.function, this.condition, this.leftArgument, this.rightArgument, this.codeblock);
        }

        public override bool Equals(object obj)
        {
            if (obj is UserDefOperator)
            {
                UserDefOperator other = (UserDefOperator)obj;
                return (this.name == other.name) &&
                    (this.function == other.function) && (this.condition == other.condition) &&
                    (this.leftArgument == other.leftArgument) && (this.rightArgument == other.rightArgument) &&
                    (this.codeblock == other.codeblock);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int result = this.name.GetHashCode() ^ this.function.GetHashCode() ^ this.codeblock.GetHashCode();

            if (this.leftArgument != null)
            {
                result ^= this.leftArgument.GetHashCode();
            }

            if (this.rightArgument != null)
            {
                result ^= this.rightArgument.GetHashCode();
            }

            if (this.condition != null)
            {
                result ^= this.condition.GetHashCode();
            }

            return result;
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds a <b>monadic</b> user defined operator with a <b>monadic</b> derived function.
        /// </summary>
        /// <remarks>
        /// Builds a representation of a <see cref="UserDefOperator"/> of the following structure:
        /// <example>
        /// (f OP) b: { ... }
        /// </example>
        /// </remarks>
        /// <param name="name">The name of the user defined operator.</param>
        /// <param name="function">The name of the function in the user defined operator.</param>
        /// <param name="argument">The name of the monadic argument of the user defined operator.</param>
        /// <param name="codeblock">The codeblock of the user defined operator.</param>
        /// <param name="codeText">The string representation of the user defined operator.</param>
        /// <returns>Returns a monadic case of <see cref="UserDefOperator"/> AST node.</returns>
        public static UserDefOperator MonadicUserDefOperator(
            Identifier name,
            Identifier function,
            Identifier argument,
            Node codeblock, string codeText = "")
        {

            UserDefOperator result = new UserDefOperator(name, function, codeblock, codeText)
            {
                RightArgument = argument
            };

            return result;
        }

        /// <summary>
        /// Builds a <b>monadic</b> user defined operator with a <b>dyadic</b> derived function.
        /// </summary>
        /// <remarks>
        /// Builds a representation of a <see cref="UserDefOperator"/> of the following structure:
        /// <example>
        /// a (f OP) b: { ... }
        /// </example>
        /// </remarks>
        /// <param name="name">The name of the user defined operator.</param>
        /// <param name="function">The name of the function in the user defined operator.</param>
        /// <param name="leftArgument">The name of left argument of the user defined operator.</param>
        /// <param name="rightArgument">The name of the right argument of the user defined operator.</param>
        /// <param name="codeblock">The codeblock of the user defined operator.</param>
        /// <param name="codeText">The string representation of the user defined operator.</param>
        /// <returns>Returns a monadic case of <see cref="UserDefOperator"/> AST node.</returns>
        public static UserDefOperator MonadicUserDefOperator(
            Identifier name,
            Identifier function,
            Identifier leftArgument, Identifier rightArgument,
            Node codeblock, string codeText = "")
        {
            UserDefOperator result = MonadicUserDefOperator(name, function, rightArgument, codeblock, codeText);
            result.LeftArgument = leftArgument;

            return result;
        }

        /// <summary>
        /// Builds a <b>dyadic</b> user defined operator with a <b>monadic</b> derived function.
        /// </summary>
        /// <remarks>
        /// Builds a representation of a <see cref="UserDefOperator"/> of the following structure:
        /// <example>
        /// (f OP h) b: { ... }
        /// </example>
        /// </remarks>
        /// <param name="name">The name of the user defined operator.</param>
        /// <param name="function">The name of the function in the user defined operator.</param>
        /// <param name="condition">The name of the condition in the user defined operator.</param>
        /// <param name="argument">The name of the monadic argument of the user defined operator.</param>
        /// <param name="codeblock">The codeblock of the user defined operator.</param>
        /// <param name="codeText">The string representation of the user defined operator.</param>
        /// <returns>Returns a dyadic case of <see cref="UserDefOperator"/> AST node.</returns>
        public static UserDefOperator DyadicUserDefOperator(
            Identifier name,
            Identifier function, Identifier condition,
            Identifier argument,
            Node codeblock, string codeText = "")
        {
            UserDefOperator result = MonadicUserDefOperator(name, function, argument, codeblock, codeText);
            result.Condition = condition;

            return result;
        }

        /// <summary>
        /// Builds a <b>dyadic</b> user defined operator with a <b>dyadic</b> derived function.
        /// </summary>
        /// <remarks>
        /// Builds a representation of a <see cref="UserDefOperator"/> of the following structure:
        /// <example>
        /// a (f OP h) b: { ... }
        /// </example>
        /// </remarks>
        /// <param name="name">The name of the user defined operator.</param>
        /// <param name="function">The name of the function in the user defined operator.</param>
        /// <param name="condition">The name of the condition in the user defined operator.</param>
        /// <param name="leftArgument">The name of left argument of the user defined operator.</param>
        /// <param name="rightArgument">The name of the right argument of the user defined operator.</param>
        /// <param name="codeblock">The codeblock of the user defined operator.</param>
        /// <param name="codeText">The string representation of the user defined operator.</param>
        /// <returns>Returns a dyadic case of <see cref="UserDefOperator"/> AST node.</returns>
        public static UserDefOperator DyadicUserDefOperator(
            Identifier name,
            Identifier function, Identifier condition,
            Identifier leftArgument, Identifier rightArgument,
            Node codeblock, string codeText = "")
        {
            UserDefOperator result = DyadicUserDefOperator(name, function, condition, rightArgument, codeblock, codeText);
            result.LeftArgument = leftArgument;

            return result;
        }
    }

    #endregion
}
