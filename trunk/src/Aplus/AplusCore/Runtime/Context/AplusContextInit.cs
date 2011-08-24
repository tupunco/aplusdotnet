using System;

namespace AplusCore.Runtime.Context
{
    /// <summary>
    /// Specifies the method that should be invoked on context initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class AplusContextInitAttribute : Attribute
    {
    }
}
