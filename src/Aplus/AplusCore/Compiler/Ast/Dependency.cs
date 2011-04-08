using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCore.Compiler.AST
{
    public class Dependency : Node
    {
        #region Variables

        private Identifier variable;
        private Node functionBody;
        private string codeText;
        private HashSet<Identifier> dependantSet;

        #endregion

        #region Constructors

        public Dependency(AST.Identifier variable, Node functionBody, string codeText, HashSet<Identifier> dependantSet)
        {
            this.variable = variable;
            this.functionBody = functionBody;
            this.codeText = codeText;
            this.dependantSet = dependantSet;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result = null;
            Aplus runtime = scope.GetRuntime();
   
            // 1. Create a function scope
            string dependencyName = this.variable.BuildQualifiedName(runtime.CurrentContext);
            string scopename = String.Format("__dependency_{0}_scope__", this.variable.Name);
            AplusScope dependencyScope = new AplusScope(scope, scopename,
                moduleParam: DLR.Expression.Parameter(typeof(DYN.ExpandoObject), scopename),
                returnTarget: DLR.Expression.Label(typeof(AType), "RETURN"),
                enviromentParam: DLR.Expression.Parameter(typeof(AplusEnvironment), "_EXTERNAL_EVN_"),
                isMethod: true
            );

            // 2. Prepare the method arguments (AplusEnvironment)
            DLR.ParameterExpression[] methodParameters = new DLR.ParameterExpression[] {  dependencyScope.AplusEnvironment };
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
                typeof(AFunc).GetMethod("Create", new Type[] { typeof(string), typeof(object), typeof(int), typeof(string) }),
                DLR.Expression.Constant(dependencyName),
                method,
                DLR.Expression.Constant(methodParameters.Length),
                DLR.Expression.Constant(this.codeText)
            );

            // 3.5 Build dependant set
            HashSet<string> dependats = new HashSet<string>(
                this.dependantSet.Select(item => { return item.BuildQualifiedName(runtime.CurrentContext); })
            );

            // 4. Register the method for the Dependency manager
            DLR.Expression dependencyManager = DLR.Expression.Property(scope.RuntimeExpression, "DependencyManager");
            result = DLR.Expression.Block(
                DLR.Expression.Call(
                    dependencyManager,
                    typeof(DependencyManager).GetMethod("Register"),
                    DLR.Expression.Constant(dependencyName, typeof(string)),
                    DLR.Expression.Constant(dependats, typeof(HashSet<string>)),
                    wrappedLambda
                ),
                wrappedLambda
            );

            return result;
        }

        #endregion

        #region Dependency calculation

        internal static void UpdateDependantSet(
            Dictionary<string, List<Identifier>> localAssignments,
            Dictionary<string, List<Identifier>> globalAssignments,
            HashSet<Identifier> variableAccessing)
        {
            // FIX: currently we treat every variable as a global variable. 
            // TODO: Remove the local variables from the set.
        }

        #endregion
    }

    #region Construction helper
    partial class Node
    {
        public static Dependency Dependency(
            Node variable, Node functionBody, string codeText, HashSet<Identifier> dependantSet)
        {
            Debug.Assert(variable is Identifier);

            return new Dependency((Identifier)variable, functionBody, codeText, dependantSet);
        }
    }
    #endregion
}
