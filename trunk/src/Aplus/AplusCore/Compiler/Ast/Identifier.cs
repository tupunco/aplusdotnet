using System;
using System.Collections.Generic;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Specifies the type of an identifier
    /// </summary>
    public enum IdentifierType
    {
        UnQualifiedName, QualifiedName, SystemName
    }

    /// <summary>
    /// Represents an identifier in an A+ AST.
    /// </summary>
    public class Identifier : Node
    {
        #region Variables

        private string variableName;
        private IdentifierType type;
        private bool isEnclosed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.Identifier; }
        }

        /// <summary>
        /// Gets or sets the name of the identifier.
        /// </summary>
        public string Name
        {
            get { return this.variableName; }
            set { this.variableName = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IdentifierType">type</see> of the identifier.
        /// </summary>
        public IdentifierType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets or sets if the identifier was enclosed inside parantheses.
        /// </summary>
        public bool IsEnclosed
        {
            get { return this.isEnclosed; }
            set { this.isEnclosed = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Identifier"/> AST node.
        /// </summary>
        /// <param name="varibleName">Name of the identifier.</param>
        /// <param name="type">The <see cref="IdentifierType">type</see> of the identifier.</param>
        public Identifier(string varibleName, IdentifierType type)
        {
            this.variableName = varibleName;
            this.type = type;
            this.isEnclosed = false;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;
            Aplus runtime = scope.GetRuntime();

            if (this.Type == IdentifierType.SystemName && runtime.SystemFunctions.ContainsKey(this.Name))
            {
                // Check if the name is a system function's name and we have such system function
                // and return it
                result = DLR.Expression.Constant(runtime.SystemFunctions[this.Name]);

                return result;
            }

            DLR.Expression variableContainer = scope.GetModuleExpression();
            string[] contextParts = CreateContextNames(runtime.CurrentContext);

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

            result = Tools.CloneMemoryMappedFile(BuildGlobalAccessor(scope, runtime, variableContainer, contextParts));

            return result;
        }

        private DLR.Expression BuildGlobalAccessor(
            AplusScope scope, Aplus runtime, DLR.Expression variableContainer, string[] contextParts)
        {
            DLR.ParameterExpression environment = scope.GetRuntimeExpression();
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
                DLR.Expression.Condition(
                    dependencyInformation.Property("IsItemwise"),
                    AST.UserDefInvoke.BuildInvoke(
                        runtime,
                        new DLR.Expression[]
                        {
                            dependencyInformation.Property("Function"),
                            environment,
                            dependencyInformation.Property("InvalidIndex")
                        }
                    ),
                    AST.UserDefInvoke.BuildInvoke(
                        runtime,
                        new DLR.Expression[] 
                        {
                            DLR.Expression.Property(dependencyInformation, "Function"),
                            environment
                        }
                    )
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
                ).ToAType(runtime);

            return result;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Build a Qualified name based on the type of the identifier and the current context.
        /// </summary>
        /// <param name="currentContext">The name of the current context</param>
        /// <returns>Return fully qualified name.</returns>
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
        /// Builds a pair of strings, containing the name of the context and the name of the identifier.
        /// </summary>
        /// <param name="currentContext">The name of the current context.</param>
        /// <returns>Returns a pair of strings, containing the name of the context and the name of the identifier.</returns>
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
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds an <see cref="Identifier"/> for the given name and <see cref="IdentifierType">type</see>see.
        /// </summary>
        /// <param name="variableName">The name of the identifier.</param>
        /// <param name="type">The <see cref="IdentifierType">type</see> of the identifier.</param>
        /// <returns>Returns an <see cref="Identifier"/>.</returns>
        public static Identifier Identifier(string variableName, IdentifierType type)
        {
            return new Identifier(variableName, type);
        }

        /// <summary>
        /// Builds a Qualified name for the given identifier name.
        /// </summary>
        /// <param name="variableName">Name of the identifier.</param>
        /// <returns>
        /// Returns an <see cref="Identifier"/> with <see cref="IdentifierType.QualifiedName"/> as the identifier type.
        /// </returns>
        public static Identifier QualifiedName(string variableName)
        {
            return new Identifier(variableName, IdentifierType.QualifiedName);
        }

        /// <summary>
        /// Builds a Unqualified name for the given identifier name.
        /// </summary>
        /// <param name="variableName">Name of the identifier.</param>
        /// <returns>
        /// Returns an <see cref="Identifier"/> with <see cref="IdentifierType.UnQualifiedName"/> as the identifier type.
        /// </returns>
        public static Identifier UnQualifiedName(string variableName)
        {
            return new Identifier(variableName, IdentifierType.UnQualifiedName);
        }

        /// <summary>
        /// Builds a system name for the given identifier name.
        /// </summary>
        /// <param name="variableName">Name of the identifier.</param>
        /// <returns>
        /// Returns an <see cref="Identifier"/> with <see cref="IdentifierType.UnQualifiedName"/> as the identifier type.
        /// </returns>
        public static Identifier SystemName(string varibaleName)
        {
            return new Identifier(varibaleName, IdentifierType.SystemName);
        }
    }

    #endregion
}
