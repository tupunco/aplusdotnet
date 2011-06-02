using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AplusCore.Compiler.Grammar;
using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    public class Dependency : Node
    {
        #region Variables

        private Identifier variable;
        private Node functionBody;
        private string codeText;
        private Variables variables;
        private Identifier indexer;

        #endregion

        #region Properties

        internal Identifier Indexer
        {
            get { return this.indexer; }
            set { this.indexer = value; }
        }

        internal bool IsItemwise
        {
            get { return this.indexer != null; }
        }

        #endregion

        #region Constructors

        public Dependency(Identifier variable, Node functionBody, string codeText, Variables variables)
        {
            this.variable = variable;
            this.functionBody = functionBody;
            this.codeText = codeText;
            this.variables = variables;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            PreprocessVariables(scope);

            DLR.Expression result = null;
            Aplus runtime = scope.GetRuntime();

            if (scope.IsEval && scope.IsMethod)
            {
                // override the original scope
                // create a top level scope
                scope = new AplusScope(null, "_EVAL_DEP_SCOPE_",
                    scope.GetRuntime(),
                    scope.GetRuntimeExpression(),
                    scope.Parent.GetModuleExpression(),
                    scope.GetAplusEnvironment(),
                    scope.ReturnTarget,
                    isEval: true
                );
            }

            // 1. Create a function scope
            string dependencyName = this.variable.BuildQualifiedName(runtime.CurrentContext);
            string scopename = String.Format("__dependency_{0}_scope__", this.variable.Name);
            AplusScope dependencyScope = new AplusScope(scope, scopename,
                moduleParam: DLR.Expression.Parameter(typeof(DYN.ExpandoObject), scopename),
                returnTarget: DLR.Expression.Label(typeof(AType), "RETURN"),
                enviromentParam: DLR.Expression.Parameter(typeof(AplusEnvironment), "_EXTERNAL_EVN_"),
                isMethod: true
            );
            // 1.5. Method for registering dependencies
            MethodInfo registerMethod;

            // 2. Prepare the method arguments (AplusEnvironment)
            DLR.ParameterExpression[] methodParameters;

            if (this.IsItemwise)
            {
                DLR.ParameterExpression index = 
                    DLR.Expression.Parameter(typeof(AType), string.Format("__INDEX[{0}]__", this.Indexer.Name));
                dependencyScope.Variables.Add(this.Indexer.Name, index);

                methodParameters = new DLR.ParameterExpression[] { dependencyScope.AplusEnvironment, index };
                registerMethod = typeof(DependencyManager).GetMethod("RegisterItemwise");
            }
            else
            {
                methodParameters = new DLR.ParameterExpression[] { dependencyScope.AplusEnvironment };
                registerMethod = typeof(DependencyManager).GetMethod("Register");
            }

            // 2.5  Create a result parameter
            DLR.ParameterExpression resultParameter = DLR.Expression.Parameter(typeof(AType), "__RESULT__");

            // 2.75 Get the dependency informations
            DLR.Expression dependencyInformation =
                DLR.Expression.Call(
                    DLR.Expression.Property(dependencyScope.GetRuntimeExpression(), "DependencyManager"),
                    typeof(DependencyManager).GetMethod("GetDependency"),
                    DLR.Expression.Constant(dependencyName)
                );

            // 3. Build the method
            DLR.LambdaExpression method = DLR.Expression.Lambda(
                DLR.Expression.Block(
                    new DLR.ParameterExpression[] { dependencyScope.ModuleExpression, resultParameter },
                    // Add the local scope's store
                    DLR.Expression.Assign(dependencyScope.ModuleExpression, DLR.Expression.Constant(new DYN.ExpandoObject())),
                    // set AplusEnviroment's function scope reference
                    DLR.Expression.Assign(
                        DLR.Expression.Property(dependencyScope.AplusEnvironment, "FunctionScope"),
                        dependencyScope.ModuleExpression
                    ),
                    // Mark the dependency as under evaluation
                    DLR.Expression.Call(
                        dependencyInformation,
                        typeof(DependencyItem).GetMethod("Mark"),
                        DLR.Expression.Constant(DependencyState.Evaluating)
                    ),
                    // Calculate the result of the defined function
                    DLR.Expression.Assign(
                        resultParameter,
                        DLR.Expression.Label(dependencyScope.ReturnTarget, this.functionBody.Generate(dependencyScope))
                    ),
                    // Mark the dependency as valid
                    DLR.Expression.Call(
                        dependencyInformation,
                        typeof(DependencyItem).GetMethod("Mark"),
                        DLR.Expression.Constant(DependencyState.Valid)
                    ),
                    // reset  AplusEnviroment's function scope reference
                    DLR.Expression.Assign(
                        DLR.Expression.Property(dependencyScope.AplusEnvironment, "FunctionScope"),
                        DLR.Expression.Constant(null, typeof(DYN.ExpandoObject))
                    ),
                    // Return the result
                    resultParameter
                ),
                dependencyName,
                methodParameters
            );

            DLR.Expression wrappedLambda = DLR.Expression.Call(
                typeof(AFunc).GetMethod("Create"),
                DLR.Expression.Constant(dependencyName),
                method,
                DLR.Expression.Constant(methodParameters.Length),
                DLR.Expression.Constant(this.codeText)
            );

            // 3.5 Build dependant set
            // filter out the variables from the dependant set if it is a local variable
            HashSet<string> dependents = new HashSet<string>(
                from pair in this.variables.Accessing                           // get all variables
                where !this.variables.LocalAssignment.ContainsKey(pair.Key)     // but skip the local variables 
                select pair.Value[0].BuildQualifiedName(runtime.CurrentContext) // then build the correct name
            );

            // 4. Register the method for the Dependency manager
            DLR.ParameterExpression dependencyMethodParam = DLR.Expression.Parameter(typeof(AType), "__DEP._METHOD__");
            DLR.Expression dependencyManager = DLR.Expression.Property(scope.GetRuntimeExpression(), "DependencyManager");
            result = DLR.Expression.Block(
                new DLR.ParameterExpression[] { dependencyMethodParam },
                DLR.Expression.Assign(dependencyMethodParam, wrappedLambda),
                DLR.Expression.Call(
                    dependencyManager,
                    registerMethod,
                    DLR.Expression.Constant(dependencyName, typeof(string)),
                    DLR.Expression.Constant(dependents, typeof(HashSet<string>)),
                    dependencyMethodParam
                ),
                dependencyMethodParam
            );

            return result;
        }

        private void PreprocessVariables(AplusScope scope)
        {
            if (this.IsItemwise)
            {
                this.variables.Accessing.Remove(this.indexer.Name);
            }

            if (this.variables.LocalAssignment.ContainsKey(this.variable.Name))
            {
                if (!this.variables.GlobalAssignment.ContainsKey(this.variable.Name))
                {
                    this.variables.GlobalAssignment[this.variable.Name] = new List<Identifier>();
                }

                foreach (Identifier variable in this.variables.LocalAssignment[this.variable.Name])
                {
                    variable.Name = variable.BuildQualifiedName(scope.GetRuntime().CurrentContext);
                    variable.Type = IdentifierType.QualifiedName;

                    this.variables.GlobalAssignment[this.variable.Name].Add(variable);
                }

                this.variables.LocalAssignment.Remove(this.variable.Name);    
            }

            if (this.variables.Accessing.ContainsKey(this.variable.Name))
            {
                foreach (Identifier variable in this.variables.Accessing[this.variable.Name])
                {
                    variable.Name = variable.BuildQualifiedName(scope.GetRuntime().CurrentContext);
                    variable.Type = IdentifierType.QualifiedName;
                }
            }
        }

        #endregion
    }

    #region Construction helper

    partial class Node
    {
        public static Dependency Dependency(Identifier variable, Node functionBody, string codeText, Variables variables)
        {
            return new Dependency(variable, functionBody, codeText, variables);
        }
    }

    #endregion
}
