﻿using System;
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
    /// <summary>
    /// Represents a dependeny definition in an A+ AST.
    /// </summary>
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

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.Dependency; }
        }

        /// <summary>
        /// Gets the name of the variable of the dependency.
        /// </summary>
        public Identifier Variable
        {
            get { return this.variable; }
        }

        /// <summary>
        /// Gets the body of the dependency definition.
        /// </summary>
        public Node FunctionBody
        {
            get { return this.functionBody; }
        }

        /// <summary>
        /// Specifies if the dependency definition is an itemwise definition.
        /// </summary>
        public bool IsItemwise
        {
            get { return this.indexer != null; }
        }

        /// <summary>
        /// Gets or sets the name of the itemwise argument.
        /// </summary>
        public Identifier Indexer
        {
            get { return this.indexer; }
            set { this.indexer = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Dependency"/> AST node.
        /// </summary>
        /// <param name="variable">The name of the dependency definition.</param>
        /// <param name="functionBody">The body of the dependency definition.</param>
        /// <param name="codeText">The string representation of the dependency defintion.</param>
        /// <param name="variables">The variables associated with the dependency definition.</param>
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
                    scope.ReturnTarget,
                    isEval: true
                );
            }

            // 1. Create a function scope
            string dependencyName = this.variable.BuildQualifiedName(runtime.CurrentContext);
            string scopename = String.Format("__dependency_{0}_scope__", this.variable.Name);
            AplusScope dependencyScope = new AplusScope(scope, scopename,
                runtimeParam: scope.GetRuntimeExpression(),
                moduleParam: DLR.Expression.Parameter(typeof(DYN.ExpandoObject), scopename),
                returnTarget: DLR.Expression.Label(typeof(AType), "RETURN"),
                isMethod: true
            );
            // 1.5. Method for registering dependencies
            MethodInfo registerMethod;

            // 2. Prepare the method arguments (RuntimeExpression)
            DLR.ParameterExpression[] methodParameters;

            if (this.IsItemwise)
            {
                DLR.ParameterExpression index = 
                    DLR.Expression.Parameter(typeof(AType), string.Format("__INDEX[{0}]__", this.Indexer.Name));
                dependencyScope.Variables.Add(this.Indexer.Name, index);

                methodParameters = new DLR.ParameterExpression[] { dependencyScope.RuntimeExpression, index };
                registerMethod = typeof(DependencyManager).GetMethod("RegisterItemwise");
            }
            else
            {
                methodParameters = new DLR.ParameterExpression[] { dependencyScope.RuntimeExpression };
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
                        DLR.Expression.Property(dependencyScope.RuntimeExpression, "FunctionScope"),
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
                        DLR.Expression.Property(dependencyScope.RuntimeExpression, "FunctionScope"),
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

        #region overrides

        public override string ToString()
        {
            return string.Format("Dependency({0}[{1}] {2})", this.variable, this.indexer, this.functionBody);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Dependency))
            {
                return false;
            }

            Dependency other = (Dependency)obj;
            return this.variable == other.variable && this.indexer == other.indexer &&
                this.functionBody == other.functionBody;
        }

        public override int GetHashCode()
        {
            return this.variable.GetHashCode() ^ this.functionBody.GetHashCode() ^
                (this.IsItemwise ? this.indexer.GetHashCode() : 0);
        }

        #endregion
    }

    #region Construction helper

    partial class Node
    {
        /// <summary>
        /// Builds a <see cref="Dependency"/> node.
        /// </summary>
        /// <param name="variable">The name of the dependency definition.</param>
        /// <param name="functionBody">The body of the dependency definition.</param>
        /// <param name="codeText">The string representation of the dependency defintion.</param>
        /// <param name="variables">The variables associated with the dependency definition.</param>
        /// <returns>Returns a <see cref="Dependency"/> node.</returns>
        public static Dependency Dependency(Identifier variable, Node functionBody, string codeText, Variables variables)
        {
            return new Dependency(variable, functionBody, codeText, variables);
        }
    }

    #endregion
}
