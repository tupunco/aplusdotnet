using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Miscellaneous
{
    class Min : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return AInteger.Create(Math.Min(leftArgument.asInteger, rightArgument.asInteger));
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return AFloat.Create(Math.Min(leftArgument.asFloat, rightArgument.asFloat));
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return AFloat.Create(Math.Min(leftArgument.asFloat, rightArgument.asFloat));
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return AFloat.Create(Math.Min(leftArgument.asFloat, rightArgument.asFloat));
        }
    }
}
