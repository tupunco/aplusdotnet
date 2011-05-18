using System;
using System.Collections.Generic;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public enum IdentifierType
    {
        UnQualifiedName, QualifiedName, SystemName
    }

    public class Identifier : Node
    {
        #region Variables

        private string variableName;
        private IdentifierType type;
        private bool isEnclosed;

        #endregion

        #region Constructor

        public Identifier(string varibleName, IdentifierType type)
        {
            this.variableName = varibleName;
            this.type = type;
            this.isEnclosed = false;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return this.variableName; }
            set { this.variableName = value; }
        }

        public IdentifierType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Specifies if the identifier (during the parsing of code) was enclosed inside parentheses.
        /// </summary>
        public bool IsEnclosed
        {
            get { return this.isEnclosed; }
            set { this.isEnclosed = value; }
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            Aplus runtime = scope.GetRuntime();
            DLR.Expression variableContainer = scope.GetModuleExpression();

            string[] contextParts = CreateContextNames(runtime.CurrentContext);
            DLR.Expression result;

            // Check if the scope is a method
            if (scope.IsMethod)
            {
                DLR.Expression parentVariableContainer = scope.Parent.GetModuleExpression();

                // Check for variable in method scope
                // (maybe the variable is defined in the method header)
                DLR.Expression localVariable = scope.FindIdentifier(this.variableName);
                if (localVariable != null)
                {
                    // Found a variable defined in the method's header
                    return localVariable;
                }

                if (this.type == IdentifierType.UnQualifiedName)
                {
                    // 1). we check if the variable exists in the function's scope
                    // 2). check if the variable exits in the current context (error if not found)
                    //
                    // if(((IDictionary<String, Object>)($FunctionScope).ContainsKey($VARIABLE))
                    // {
                    //      return $FunctionScope.$VARIABLE;
                    // }
                    // else
                    // {
                    //      return $GlobalScope.$VARIABLE
                    // }
                    //
                    DLR.Expression getVariable = DLR.Expression.Condition(
                        DLR.Expression.Call(
                            DLR.Expression.Convert(variableContainer, typeof(IDictionary<string, object>)),
                            typeof(IDictionary<string, object>).GetMethod("ContainsKey"),
                            DLR.Expression.Constant(this.variableName)
                        ),
                        // True case:
                        DLR.Expression.Dynamic(
                            runtime.GetMemberBinder(this.variableName),
                            typeof(object),
                            variableContainer
                        ),
                        // False case:
                        BuildGlobalAccessor(scope, runtime, parentVariableContainer, contextParts),
                        // resulting type
                        typeof(object)
                    );

                    result = DLR.Expression.Dynamic(
                        runtime.ConvertBinder(typeof(AType)),
                        typeof(AType),
                        getVariable
                    );

                    return result;
                }
                else if (this.type == IdentifierType.QualifiedName)
                {
                    // Found a variable like: .var
                    // for this check the parent's variables
                    variableContainer = parentVariableContainer;
                    // Fallback to the non-method case
                }

            }

            result = BuildGlobalAccessor(scope, runtime, variableContainer, contextParts);

            return result;
        }

        private DLR.Expression BuildGlobalAccessor(
            AplusScope scope, Aplus runtime, DLR.Expression variableContainer, string[] contextParts)
        {
            DLR.Expression name = DLR.Expression.Constant(BuildQualifiedName(runtime.CurrentContext));
            DLR.Expression dependencyManager = DLR.Expression.Property(scope.GetRuntimeExpression(), "DependencyManager");
            // Build the ET for getting the dependecy for the variable
            DLR.Expression dependencyInformation =
                DLR.Expression.Call(
                    dependencyManager,
                    typeof(DependencyManager).GetMethod("GetDependency"),
                    name
                );

            // Build the ET for invoking the dependecy.
            DLR.Expression dependencyEvaulate =
                AST.UserDefInvoke.BuildInvoke(
                    runtime,
                    new DLR.Expression[] 
                    {
                        DLR.Expression.Property(dependencyInformation, "Function"),
                        scope.GetAplusEnvironment()
                    }
                );

            /* 
             * Simplified code of the resulting ET:
             * 
             * result = $runtime.DependecyManager.IsInvalid($$variable)
             *          ? ($$variable = $runtime.DependencyManager.GetDependency($$name).Function())
             *          : $$variable
             */
            DLR.Expression result =
                DLR.Expression.Condition(
                    DLR.Expression.Call(
                        dependencyManager,
                        typeof(DependencyManager).GetMethod("IsInvalid"),
                        name
                    ),
                    VariableHelper.SetVariable(
                            runtime,
                            variableContainer,
                            contextParts,
                            dependencyEvaulate
                    ),
                    VariableHelper.GetVariable(
                            runtime,
                            variableContainer,
                            contextParts
                    ),
                    typeof(object)
                ).ConvertToAType(runtime);

            return result;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// returns a Qualified Name based on the variable's type and the current context
        /// </summary>
        /// <param name="currentContext">The current context to use for UnQualified names</param>
        /// <returns>Fully Qualified variable name</returns>
        internal string BuildQualifiedName(string currentContext)
        {
            if (this.type == IdentifierType.UnQualifiedName)
            {
                // Join the current context and variable name together with a dot,
                //  if the current context is "." then use an empty string instead
                return String.Join(".",
                    (currentContext != ".") ? currentContext : "",
                    this.variableName);
            }

            return this.variableName;
        }

        /// <summary>
        /// Returns a pair of strings, containing a Context Name and a Variable name
        /// </summary>
        /// <param name="currentContext"></param>
        /// <returns></returns>
        internal string[] CreateContextNames(string currentContext)
        {
            if (this.type == IdentifierType.QualifiedName)
            {
                return Util.SplitUserName(this.variableName);
            }

            return new string[] { currentContext, this.variableName };
        }


        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Identifier({0})", this.variableName);

        }

        public override bool Equals(object obj)
        {
            if (obj is Identifier)
            {
                Identifier other = (Identifier)obj;
                return (this.variableName == other.variableName) && (this.type == other.type);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.type.GetHashCode() ^ this.variableName.GetHashCode();
        }

        #endregion

        #region GraphViz output (Only in DEBUG)

#if DEBUG
        static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder text)
        {
            string name = String.Format("ID{0}", counter++);
            text.AppendFormat("  {0} [label=\"{1}\"];\n", name, this.variableName);
            return name;
        }
#endif

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static Identifier Identifier(string variableName, IdentifierType type)
        {
            return new Identifier(variableName, type);
        }

        public static Identifier QualifiedName(string variableName)
        {
            return new Identifier(variableName, IdentifierType.QualifiedName);
        }

        public static Identifier UnQualifiedName(string variableName)
        {
            return new Identifier(variableName, IdentifierType.UnQualifiedName);
        }

        public static Identifier SystemName(string varibaleName)
        {
            return new Identifier(varibaleName, IdentifierType.SystemName);
        }

    }

    #endregion
}
