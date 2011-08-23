using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Arithmetic
{
    [DefaultResult(ATypes.AFloat)]
    class Reciprocal : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, Aplus environment = null)
        {
            return calculateReciprocal(argument.asFloat);
        }

        public override AType ExecutePrimitive(AFloat argument, Aplus environment = null)
        {
            return calculateReciprocal(argument.asFloat);
        }

        private AType calculateReciprocal(double number)
        {
            if (number == 0)
            {
                return AFloat.Create(Double.PositiveInfinity);
            }
            return AFloat.Create(1 / number);
        }
    }
}
