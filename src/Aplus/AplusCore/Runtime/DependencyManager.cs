using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;

namespace AplusCore.Runtime
{
    public class DependencyManager
    {
        #region Variables

        /// <summary>
        /// variablename -> dependency information mapping
        /// </summary>
        private Dictionary<string, DependencyItem> mapping;

        #endregion

        #region Properties
        #endregion

        #region Constructors

        public DependencyManager()
        {
            this.mapping = new Dictionary<string, DependencyItem>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register a dependency.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="dependentItems"></param>
        /// <param name="function"></param>
        /// <returns>Returns the registerd dependency information.</returns>
        public DependencyItem Register(string variableName, HashSet<string> dependentItems, AType function)
        {
            DependencyItem item = new DependencyItem(variableName, dependentItems, function);
            this.mapping[variableName] = item;

            // Invalidate any dependencies using this one
            InvalidateDependencies(variableName);
            return item;
        }

        /// <summary>
        /// Check if the variable's dependency is invalid.
        /// </summary>
        /// <param name="variableName">The name of the variable to check for.</param>
        /// <returns>
        /// True: if the dependency of the variable is diry.
        /// False: otherwise or does not exists.
        /// </returns>
        public bool IsInvalid(string variableName)
        {
            return this.mapping.ContainsKey(variableName) &&
                this.mapping[variableName].State == DependencyState.Invalid;
        }


        /// <summary>
        /// Returns the dependency information associated to the variable.
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public DependencyItem GetDependency(string variableName)
        {
            return this.mapping[variableName];
        }

        /// <summary>
        /// Mark dependencies invalid, based on the variable.
        /// </summary>
        /// <param name="variableName">The name of the variable to check for.</param>
        /// <returns>Number of dependencies marked invalid.</returns>
        public int InvalidateDependencies(string variableName)
        {
            int markedCount = 0;

            foreach (DependencyItem item in this.mapping.Values)
            {
                if (item.ContainsVariable(variableName))
                {
                    item.MarkAsInvalid();
                    markedCount++;
                }
            }

            return markedCount;
        }

        #endregion
    }
}
