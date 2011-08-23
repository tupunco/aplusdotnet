using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Arithmetic
{
    class Identity : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, Aplus environment = null)
        {
            return AInteger.Create(argument.asInteger);
        }

        public override AType ExecutePrimitive(AFloat argument, Aplus environment = null)
        {
            return AFloat.Create(argument.asFloat);
        }

        public override AType ExecutePrimitive(AChar argument, Aplus environment = null)
        {
            return AChar.Create(argument.asChar);
        }

        public override AType ExecutePrimitive(ASymbol argument, Aplus environment = null)
        {
            return ASymbol.Create(argument.asString);
        }

        public override AType ExecutePrimitive(ABox argument, Aplus environment = null)
        {
            return ABox.Create(argument.NestedItem);
        }
    }
}
