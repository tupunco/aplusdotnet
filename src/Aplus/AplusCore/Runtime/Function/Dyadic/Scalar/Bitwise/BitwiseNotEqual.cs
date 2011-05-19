using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Bitwise
{
    /// <summary>
    /// Bitwise Not Equal for input x,y => (x ^ y))
    /// </summary>
    class BitwiseNotEqual : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return AInteger.Create(leftArgument.asInteger ^ rightArgument.asInteger);
        }
    }
}
