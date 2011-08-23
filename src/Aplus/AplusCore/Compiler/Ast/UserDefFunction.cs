using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using AplusCore.Compiler.Grammar;
using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    public class UserDefFunction : Node
    {
        #region Variables

        private Identifier name;
        private ExpressionList parameters;
        private Node codeblock;
        private string code;

        #endregion

        #region Properties

        public Node Codeblock { get { return this.codeblock; } }
        public ExpressionList Parameters { get { return this.parameters; } }
        public Variables Variables { get; set; }

        #endregion

        #region Constructor

        public UserDefFunction(Identifier name, ExpressionList parameters, Node codeblock, string code)
        {
            this.name = name;
            this.parameters = parameters;
            this.codeblock = codeblock;
            this.code = code;
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

            string methodName = this.name.BuildQualifiedName(runtime.CurrentContext);

            // 1. Create a new scope for the function
            string scopename = String.Format("__method_{0}_scope__", this.name.Name);
            AplusScope methodScope = new AplusScope(scope, scopename,
                runtimeParam: DLR.Expression.Parameter(typeof(Aplus), "_EXTERNAL_RUNTIME_"),
                moduleParam: DLR.Expression.Parameter(typeof(DYN.ExpandoObject), scopename),
                returnTarget: DLR.Expression.Label(typeof(AType), "RETURN"),
                isMethod: true
            );

            // 1.5 Create a result parameter
            DLR.ParameterExpression resultParameter = DLR.Expression.Parameter(typeof(AType), "__RESULT__");

            // 2. Create function's parameters
            LinkedList<DLR.ParameterExpression> methodParameters = new LinkedList<DLR.ParameterExpression>();

            foreach (Node parameter in this.parameters.Items)
            {
                string parameterName = ((Identifier)parameter).Name;
                DLR.ParameterExpression parameterExpression = DLR.Expression.Parameter(typeof(AType), parameterName);

                // Add parameter to the scope's variable list
                methodScope.Variables[parameterName] = parameterExpression;

                // Add parameters in !reverse! order
                methodParameters.AddFirst(parameterExpression);
            }

            // Add parameter for AplusEnviroment
            methodParameters.AddFirst(methodScope.RuntimeExpression);

            // Create a return label for exiting from the function
            //methodScope.ReturnTarget = ;

            // 3. Create the lambda method for the function
            DLR.LambdaExpression method = DLR.Expression.Lambda(
                DLR.Expression.Block(
                    new DLR.ParameterExpression[] { methodScope.ModuleExpression, resultParameter },
                // Add the local scope's store
                    DLR.Expression.Assign(methodScope.ModuleExpression, DLR.Expression.Constant(new DYN.ExpandoObject())),
                // set AplusEnviroment's function scope reference
                    DLR.Expression.Assign(
                        DLR.Expression.Property(methodScope.RuntimeExpression, "FunctionScope"),
                        methodScope.ModuleExpression
                    ),
                // Calculate the result of the defined function
                    DLR.Expression.Assign(
                        resultParameter,
                        DLR.Expression.Label(methodScope.ReturnTarget, this.codeblock.Generate(methodScope))
                    ),
                // reset  AplusEnviroment's function scope reference
                    DLR.Expression.Assign(
                        DLR.Expression.Property(methodScope.RuntimeExpression, "FunctionScope"),
                        DLR.Expression.Constant(null, typeof(DYN.ExpandoObject))
                    ),
                // Return the result
                    resultParameter
                ),
                methodName,
                methodParameters
            );

            // 3.5. Wrap the lambda method inside an AFunc
            DLR.Expression wrappedLambda = DLR.Expression.Call(
                typeof(AFunc).GetMethod("Create", new Type[] { typeof(string), typeof(object), typeof(int), typeof(string) }),
                DLR.Expression.Constant(methodName),
                method,
                DLR.Expression.Constant(methodParameters.Count),
                DLR.Expression.Constant(this.code)
            );

            // 4. Set the variable to the lambda function
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

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("FunctionDef({0} {1} {2})", this.name, this.parameters, this.codeblock);
        }

        public override bool Equals(object obj)
        {
            if (obj is UserDefFunction)
            {
                UserDefFunction other = (UserDefFunction)obj;
                return (this.name == other.name) && (this.parameters == other.parameters) &&
                    (this.codeblock == other.codeblock);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode() ^ this.parameters.GetHashCode() ^ this.codeblock.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static UserDefFunction UserDefFunction(Node name, ExpressionList parameters, Node codeblock, string code = "")
        {
            Debug.Assert(name is Identifier);

            foreach (Node item in parameters.Items)
            {
                if (!(item is Identifier))
                {
                    throw new ParseException(":header?", false);
                }
            }

            return new UserDefFunction((Identifier)name, parameters, codeblock, code);
        }
    }

    #endregion
}
