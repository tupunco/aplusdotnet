using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Miscellaneous
{
    [DefaultResult(ATypes.AFloat)]
    class AbsoluteValue : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, Aplus environment = null)
        {
            double result = Math.Abs(argument.asFloat);
            return Utils.CreateATypeResult(result);
        }

        public override AType ExecutePrimitive(AFloat argument, Aplus environment = null)
        {
            return AFloat.Create(Math.Abs(argument.asFloat));
        }
    }
}
