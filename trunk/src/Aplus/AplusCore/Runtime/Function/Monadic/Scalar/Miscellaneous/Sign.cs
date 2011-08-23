using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Miscellaneous
{
    [DefaultResult(ATypes.AInteger)]
    class Sign : MonadicScalar
    {

        public override AType ExecutePrimitive(AInteger argument, Aplus environment = null)
        {
            return calculateSignum(argument.asFloat);
        }

        public override AType ExecutePrimitive(AFloat argument, Aplus environment = null)
        {
            return calculateSignum(argument.asFloat);
        }

        private AType calculateSignum(double number)
        {
            return AInteger.Create(Math.Sign(number));
        }
    }
}
