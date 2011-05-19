using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Arithmetic
{
    class Identity : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, AplusEnvironment environment = null)
        {
            return AInteger.Create(argument.asInteger);
        }

        public override AType ExecutePrimitive(AFloat argument, AplusEnvironment environment = null)
        {
            return AFloat.Create(argument.asFloat);
        }

        public override AType ExecutePrimitive(AChar argument, AplusEnvironment environment = null)
        {
            return AChar.Create(argument.asChar);
        }

        public override AType ExecutePrimitive(ASymbol argument, AplusEnvironment environment = null)
        {
            return ASymbol.Create(argument.asString);
        }

        public override AType ExecutePrimitive(ABox argument, AplusEnvironment environment = null)
        {
            return ABox.Create(argument.NestedItem);
        }
    }
}
