using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AplusCore.Runtime.Function
{
    /// <summary>
    /// Mark method as a System function to be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class SystemFunctionAttribute : Attribute
    {
        /// <summary>
        /// Name of the system function to be used in the system.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Desctiption of the system function.
        /// </summary>
        public string Description { get; private set; }


        /// <summary>
        /// Initialises a new <see cref="SystemFunctionAttribute"/> with the given parameters.
        /// </summary>
        /// <param name="name">Name of the system function.</param>
        /// <param name="description">Desctiption of the system function.</param>
        public SystemFunctionAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
