using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Scripting;

using AplusCore.Types;

namespace AplusCore.Compiler
{
    public class FunctionInformation
    {
        #region Variables

        private string context;

        private HashSet<string> localFunctions;
        private HashSet<string> globalFunctions;
        private HashSet<string> dyadicOperators;
        private HashSet<string> monadicOperators;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current name of the context.
        /// </summary>
        public string Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        #endregion

        #region Constructors

        public FunctionInformation(string context)
        {
            this.dyadicOperators = new HashSet<string>();
            this.monadicOperators = new HashSet<string>();
            this.globalFunctions = new HashSet<string>();
            this.localFunctions = new HashSet<string>();
            this.context = context;
        }

        #endregion

        #region Query utils

        public bool IsDyadicOperator(string id)
        {
            return this.dyadicOperators.Contains(QualifiedName(id));
        }

        public bool IsMonadicOperator(string id)
        {
            return this.monadicOperators.Contains(QualifiedName(id));
        }

        public bool IsGlobalFunction(string id)
        {
            return this.globalFunctions.Contains(QualifiedName(id));
        }

        public bool IsLocalFunction(string id)
        {
            return this.localFunctions.Contains(id);
        }

        public void LoadInfo(ScopeStorage scope)
        {
            Dictionary<string, AType> functions = ExtractFunctions(scope);

            foreach (KeyValuePair<string, AType> function in functions)
            {
                AFunc func = function.Value.Data as AFunc;

                if (func.Valence <= 1)
                {
                    // skip the niladic functions
                    continue;
                }

                if (func.IsOperator)
                {
                    if (func.IsDyadic)
                    {
                        this.dyadicOperators.Add(function.Key);
                    }
                    else
                    {
                        this.monadicOperators.Add(function.Key);
                    }
                }
                else
                {
                    this.globalFunctions.Add(function.Key);
                }
            }
        }

        #endregion

        #region Function registration

        /// <summary>
        /// Register the given <see cref="id"/> as a global function.
        /// </summary>
        /// <param name="id">The name of the global function.</param>
        public void RegisterGlobalFunction(string id)
        {
            this.globalFunctions.Add(QualifiedName(id));
        }

        /// <summary>
        /// Register the given <see cref="id"/> as a local function.
        /// </summary>
        /// <param name="id">The name of the local function.</param>
        public void RegisterLocalFunction(string id)
        {
            this.localFunctions.Add(id);
        }

        #endregion

        #region Utilitly

        private string QualifiedName(string id)
        {
            if (id.Contains('.'))
            {
                return id;
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(this.Context);

            if (this.Context != ".")
            {
                builder.Append(".");
            }

            builder.Append(id);
            return builder.ToString();
        }

        private static Dictionary<string, AType> ExtractFunctions(ScopeStorage scope)
        {
            Dictionary<string, AType> functions = new Dictionary<string, AType>();

            if (scope == null)
            {
                return functions;
            }

            foreach (KeyValuePair<string, object> context in scope.GetItems())
            {
                ICollection<KeyValuePair<string, object>> contextItems =
                    context.Value as ICollection<KeyValuePair<string, object>>;

                if (contextItems == null)
                {
                    ScopeStorage test = context.Value as ScopeStorage;
                    if (test == null)
                    {
                        continue;
                    }

                    contextItems = test.GetItems();
                }

                foreach (KeyValuePair<string, object> variableInfo in contextItems)
                {
                    AType variable = variableInfo.Value as AType;

                    if (variable != null && variable.Type == ATypes.AFunc && !variable.IsBox)
                    {
                        string[] parts = VariableHelper.CreateContextParts(context.Key, variableInfo.Key);
                        if (parts[0].Contains("."))
                        {
                            parts[0] = "";
                        }

                        string id = string.Join(".", parts);
                        functions.Add(id, variable);
                    }
                }
            }

            return functions;
        }

        #endregion
    }
}
