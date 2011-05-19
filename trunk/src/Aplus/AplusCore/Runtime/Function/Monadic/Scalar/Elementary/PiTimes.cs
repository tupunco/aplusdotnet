using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Elementary
{
    class PiTimes : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, AplusEnvironment environment = null)
        {
            return calculatePi(argument.asFloat);
        }

        public override AType ExecutePrimitive(AFloat argument, AplusEnvironment environment = null)
        {
            return calculatePi(argument.asFloat);
        }

        private AType calculatePi(double number)
        {
            return AFloat.Create(number * Math.PI);
        }
    }
}
