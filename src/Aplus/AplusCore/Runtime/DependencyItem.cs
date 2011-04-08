using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;

namespace AplusCore.Runtime
{
    public class DependencyItem
    {
        #region Variables

        private string variableName;
        private HashSet<string> dependentItems;
        private DependencyState state;
        private AType function;

        #endregion

        #region Properties

        /// <summary>
        /// State of the Dependency
        /// </summary>
        public DependencyState State
        {
            get { return this.state; }
        }

        /// <summary>
        /// Function associated for the dependency
        /// </summary>
        public AType Function
        {
            get { return this.function; }
        }

        #endregion

        #region Constructors

        public DependencyItem(string variableName, HashSet<string> dependentItems, AType function)
        {
            this.variableName = variableName;
            this.dependentItems = dependentItems;
            this.function = function;

            MarkAsInvalid();
        }

        #endregion

        #region Methods

        public bool ContainsVariable(string variableName)
        {
            return this.dependentItems.Contains(variableName);
        }

        public void MarkAsInvalid()
        {
            this.state = DependencyState.Invalid;
        }

        public void Mark(DependencyState state)
        {
            this.state = state;
        }

        #endregion
    }
}
