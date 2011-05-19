using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Bitwise
{
    /// <summary>
    /// Bitwise Less for input x,y => (~x) & y
    /// </summary>
    class BitwiseLess : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return AInteger.Create((~leftArgument.asInteger) & rightArgument.asInteger);
        }
    }
}
