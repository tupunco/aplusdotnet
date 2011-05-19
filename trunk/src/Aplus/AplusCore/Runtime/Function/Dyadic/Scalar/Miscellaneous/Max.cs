using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Miscellaneous
{
    class Max : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return AInteger.Create(Math.Max(leftArgument.asInteger, rightArgument.asInteger));
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return AFloat.Create(Math.Max(leftArgument.asFloat, rightArgument.asFloat));
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return AFloat.Create(Math.Max(leftArgument.asFloat, rightArgument.asFloat));
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return AFloat.Create(Math.Max(leftArgument.asFloat, rightArgument.asFloat));
        }
    }
}
