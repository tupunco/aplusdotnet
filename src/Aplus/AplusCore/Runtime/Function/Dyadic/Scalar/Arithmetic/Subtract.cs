using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Arithmetic
{
    /// <remarks>
    /// Input:                      Output:
    /// (AInteger,AInteger)     ->  AInteger
    /// (AFloat,AFloat)         ->  AFloat
    /// (AFloat,AInteger)       ->  AFloat
    /// (AInteger,AFloat)       ->  AFloat
    /// 
    /// Exceptions: Type
    /// </remarks>

    class Subtract : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            double result = leftArgument.asFloat - rightArgument.asFloat;
            return Utils.CreateATypeResult(result);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return AFloat.Create(leftArgument.asFloat - rightArgument.asFloat);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return AFloat.Create(leftArgument.asFloat - rightArgument.asInteger);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return AFloat.Create(leftArgument.asInteger - rightArgument.asFloat);
        }
    }
}
