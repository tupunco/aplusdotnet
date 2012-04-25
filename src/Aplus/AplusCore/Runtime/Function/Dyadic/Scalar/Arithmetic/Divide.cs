using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Arithmetic
{
    /// <remarks>
    /// Input:                      Output:
    /// (AInteger,AInteger)     ->  AFloat
    /// (AFloat,AFloat)         ->  AFloat
    /// (AFloat,AInteger)       ->  AFloat
    /// (AInteger,AFloat)       ->  AFloat
    /// 
    /// Exceptions: Type, Domain
    /// </remarks>
    [DefaultResult(ATypes.AFloat)]
    class Divide : DyadicScalar
    {

        /// <summary>
        /// Checks if the division's arguments are 0
        /// </summary>
        private void CheckZeroPerZero(AType right, AType left)
        {
            if (right.asFloat == 0.0 && left.asFloat == 0.0)
            {
                throw new Error.Domain(DomainErrorText);
            }
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            CheckZeroPerZero(rightArgument, leftArgument);
            return AFloat.Create(leftArgument.asInteger / rightArgument.asFloat);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            CheckZeroPerZero(rightArgument, leftArgument);
            return AFloat.Create(leftArgument.asFloat / rightArgument.asFloat);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            CheckZeroPerZero(rightArgument, leftArgument);
            return AFloat.Create(leftArgument.asFloat / rightArgument.asInteger);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            CheckZeroPerZero(rightArgument, leftArgument);
            return AFloat.Create(leftArgument.asInteger / rightArgument.asFloat);
        }
       
    }
}
