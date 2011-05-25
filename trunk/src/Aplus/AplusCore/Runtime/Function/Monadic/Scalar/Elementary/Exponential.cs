using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Elementary
{
    [DefaultResult(ATypes.AFloat)]
    class Exponential : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, AplusEnvironment environment = null)
        {
            return calculateExp(argument.asFloat);
        }

        public override AType ExecutePrimitive(AFloat argument, AplusEnvironment environment = null)
        {
            return calculateExp(argument.asFloat);
        }

        private AType calculateExp(double d)
        {
            return AFloat.Create(Math.Exp(d));
        }
    }
}
