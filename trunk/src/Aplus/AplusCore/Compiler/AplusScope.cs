using System.Collections.Generic;

using AplusCore.Runtime;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler
{
    
    public class AplusScope
    {
        #region Variables

        private AplusScope parent;
        private string name;
        private Aplus runtime;
        private DLR.ParameterExpression runtimeParam;
        private DLR.ParameterExpression moduleParam;
        private Dictionary<string, DLR.ParameterExpression> variables;

        private DLR.LabelTarget returnTarget;

        private bool iseval;

        private bool ismethod;

        private bool isAssignment;

        #endregion

        #region Constructor

        public AplusScope(AplusScope parent,
            string name,
            Aplus runtime = null,
            DLR.ParameterExpression runtimeParam = null,
            DLR.ParameterExpression moduleParam = null,
            DLR.LabelTarget returnTarget = null,
            bool isEval = false,
            bool isMethod = false,
            bool isAssignment = false)
        {
            this.parent = parent;
            this.name = name;
            this.runtime = runtime;
            this.runtimeParam = runtimeParam;
            this.moduleParam = moduleParam;

            this.returnTarget = returnTarget;

            this.variables = new Dictionary<string, DLR.ParameterExpression>();

            this.iseval = isEval;

            this.ismethod = isMethod;
            this.isAssignment = isAssignment;

            InheritProperties(parent);
        }

        #endregion

        #region Properties

        internal AplusScope Parent { get { return this.parent; } }
        internal bool IsModule { get { return this.moduleParam != null; } }
        internal DLR.ParameterExpression RuntimeExpression { get { return this.runtimeParam; } }
        internal DLR.ParameterExpression ModuleExpression { get { return this.moduleParam; } }
        internal bool IsMethod { get { return this.ismethod; } }
        internal bool IsEval { get { return this.iseval; } }

        internal bool IsAssignment
        {
            get { return this.isAssignment; }
            set { this.isAssignment = value; }
        }

        /// <summary>
        /// Target of return inside of a user defined function
        /// </summary>
        internal DLR.LabelTarget ReturnTarget
        {
            get { return this.returnTarget; }
            set { this.returnTarget = value; }
        }

        internal Dictionary<string, DLR.ParameterExpression> Variables
        {
            get { return this.variables; }
        }

        #endregion

        #region private methods

        private void InheritProperties(AplusScope parentScope)
        {
            if (parentScope == null)
            {
                return;
            }

            this.iseval |= parentScope.iseval;
            this.ismethod |= parentScope.IsMethod;
            this.isAssignment |= parentScope.IsAssignment;
        }

        #endregion

        #region Helper methods

        public Aplus GetRuntime()
        {
            AplusScope currentScope = this;
            while (currentScope.runtime == null)
            {
                currentScope = currentScope.parent;
            }

            return currentScope.runtime;
        }

        public DLR.ParameterExpression GetRuntimeExpression()
        {
            AplusScope currentScope = this;
            while (currentScope.RuntimeExpression == null)
            {
                currentScope = currentScope.parent;
            }

            return currentScope.RuntimeExpression;
        }

        public DLR.ParameterExpression GetModuleExpression()
        {
            AplusScope currentScope = this;
            while (!(currentScope.IsModule))
            {
                currentScope = currentScope.parent;
            }
            return currentScope.ModuleExpression;
        }

        public DLR.Expression FindIdentifier(string name)
        {
            AplusScope currentScope = this;
            DLR.ParameterExpression result;
            while (currentScope != null)
            {
                if (currentScope.Variables.TryGetValue(name, out result))
                {
                    return result;
                }

                currentScope = currentScope.parent;
            }
            return null;
        }

        #endregion
    }
}
