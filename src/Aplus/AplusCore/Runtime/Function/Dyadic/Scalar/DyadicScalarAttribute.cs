using System;

namespace AplusCore.Runtime.Function.Dyadic.Scalar
{
    /// <summary>
    /// Specifies the method as a Dyadic scalar method.
    /// </summary>
    /// <remarks>
    /// Only use for a method with two AType arguments.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class DyadicScalarMethodAttribute : Attribute
    {
        public DyadicScalarMethodAttribute()
        {
        }
    }
}
