using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Miscellaneous
{
    [DefaultResult(ATypes.AInteger)]
    class Ceiling : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, Aplus environment = null)
        {
            return AInteger.Create(argument.asInteger);
        }

        public override AType ExecutePrimitive(AFloat argument, Aplus environment = null)
        {
            double result;

            if (Utils.TryComprasionTolarence(argument.asFloat, out result))
            {
                return Utils.CreateATypeResult(result);
            }

            result = Math.Ceiling(argument.asFloat);
            return Utils.CreateATypeResult(result);
        }
    }
}
