using System;

namespace AplusCore.Runtime.Function.Dyadic.Scalar
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class DyadicScalarMethodAttribute : Attribute
    {

        public DyadicScalarMethodAttribute()
        {
        }

    }
}
