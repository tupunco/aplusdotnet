using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    /// <summary>
    /// Class to represent the default result type for scalar methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class DefaultResultAttribute : Attribute
    {
        private ATypes defaultType;

        public ATypes DefaultType
        {
            get { return this.defaultType; }
        }

        public DefaultResultAttribute(ATypes defaultType)
        {
            this.defaultType = defaultType;
        }
    }
}
