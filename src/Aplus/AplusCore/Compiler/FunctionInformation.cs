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
        private HashSet<string> monadicFunctions;
        private HashSet<string> dyadicFunctions;

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
            this.monadicFunctions = new HashSet<string>();
            this.dyadicFunctions = new HashSet<string>();
            this.context = context;
        }

        #endregion

        #region Query utils

        public bool IsOperator(string id)
        {
            //return this.OperatorFunctions.ContainsKey(QualifiedName(id));
            return false;
        }

        public bool IsDyadic(string id)
        {
            return this.dyadicFunctions.Contains(QualifiedName(id));
        }

        public bool IsMonadic(string id)
        {
            return this.monadicFunctions.Contains(QualifiedName(id));
        }

        public void LoadInfo(ScopeStorage scope)
        {
            Dictionary<string, AType> functions = ExtractFunctions(scope);

            foreach (KeyValuePair<string, AType> function in functions)
            {
                AFunc func = function.Value.Data as AFunc;

                switch (func.Valence)
                {
                    case 2:
                        this.monadicFunctions.Add(function.Key);
                        break;
                    case 3:
                        this.dyadicFunctions.Add(function.Key);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Function registration

        /// <summary>
        /// Register the given <see cref="id"/> as a Monadic function.
        /// </summary>
        /// <param name="id">The name of the Monadic function.</param>
        public void RegisterMonadic(string id)
        {
            this.monadicFunctions.Add(QualifiedName(id));
        }

        /// <summary>
        /// Register the given <see cref="id"/> as a Dyadic function.
        /// </summary>
        /// <param name="id">The name of the Dyadic function.</param>
        public void RegisterDyadic(string id)
        {
            this.dyadicFunctions.Add(QualifiedName(id));
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
