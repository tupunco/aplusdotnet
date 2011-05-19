using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Bitwise
{
    /// <summary>
    /// Bitwise Greater Equal for input x,y => (x | (~y))
    /// </summary>
    class BitwiseGreaterEqual : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return AInteger.Create(leftArgument.asInteger | ~(rightArgument.asInteger));
        }
    }
}
