using System;

namespace AplusCore.Runtime.Context
{
    /// <summary>
    /// Specifies the class that contains methods for the given Context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class AplusContextAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the Context.
        /// </summary>
        internal string ContextName { get; private set; }

        internal AplusContextAttribute(string contextName)
        {
            this.ContextName = contextName;
        }
    }
}
