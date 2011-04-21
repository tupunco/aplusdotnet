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
            // Invalidate any dependencies using the variable
            InvalidateDependencies(variableName);

            DependencyItem item = new DependencyItem(variableName, dependentItems, function);
            this.mapping[variableName] = item;
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
        /// <remarks>Only valid dependencies will be marked as invalid.</remarks>
        /// <param name="variableName">The name of the variable to check for.</param>
        public void InvalidateDependencies(string variableName)
        {
            foreach (DependencyItem item in this.mapping.Values)
            {
                if (item.ContainsVariable(variableName) && item.State == DependencyState.Valid)
                {
                    item.MarkAsInvalid();
                }
            }
        }

        /// <summary>
        /// Mark dependencies invalid, based on the variables.
        /// </summary>
        /// <remarks>Only valid dependencies will be marked as invalid.</remarks>
        /// <param name="variableNames">Array of the variableNames to check for.</param>
        public void InvalidateDependencies(string[] variableNames)
        {
            foreach (DependencyItem item in this.mapping.Values)
            {
                if (item.ContainsVariable(variableNames) && item.State == DependencyState.Valid)
                {
                    item.MarkAsInvalid();
                }
            }
        }

        /// <summary>
        /// Mark a dependency valid.
        /// </summary>
        /// <param name="variableName">The variable name of the dependency to mark valid.</param>
        public void ValidateDependency(string variableName)
        {
            if (this.mapping.ContainsKey(variableName))
            {
                this.mapping[variableName].Mark(DependencyState.Valid);
            }
        }

        /// <summary>
        /// Mark a dependencies valid.
        /// </summary>
        /// <param name="variableNames">The array of variable names of the dependencies to mark valid.</param>
        public void ValidateDependencies(string[] variableNames)
        {
            foreach (string varName in variableNames)
            {
                if (this.mapping.ContainsKey(varName))
                {
                    this.mapping[varName].Mark(DependencyState.Valid);
                }
            }
        }

        #endregion
    }
}
