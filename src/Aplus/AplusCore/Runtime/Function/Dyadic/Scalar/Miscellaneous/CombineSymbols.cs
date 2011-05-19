using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Miscellaneous
{
    class CombineSymbols : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(ASymbol rightArgument, ASymbol leftArgument)
        {
            string result = rightArgument.asString.Contains(".") ? rightArgument.asString : leftArgument.asString + "." + rightArgument.asString;
            return ASymbol.Create(result);
        }
    }
}
