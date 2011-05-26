using System;

using AplusCore.Types;
using AplusCore.Runtime.Function.Dyadic;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class MapIn : AbstractMonadicFunction
    {
        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            return DyadicFunctionInstance.Map.Execute(argument, AInteger.Create(0), environment);
        }
    }
}
