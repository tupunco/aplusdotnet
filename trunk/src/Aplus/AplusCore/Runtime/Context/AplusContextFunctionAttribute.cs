using System;

namespace AplusCore.Runtime.Context
{
    /// <summary>
    /// Specifies the method as a function inside a Context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class AplusContextFunctionAttribute : Attribute
    {
        /// <summary>
        /// Gets the functions name inside a Context.
        /// </summary>
        internal string ContextName { get; private set; }

        /// <summary>
        /// Gets the description for the method.
        /// </summary>
        internal string Description { get; private set; }

        internal AplusContextFunctionAttribute(string contextName, string description)
        {
            this.ContextName = contextName;
            this.Description = description;
        }
    }
}
