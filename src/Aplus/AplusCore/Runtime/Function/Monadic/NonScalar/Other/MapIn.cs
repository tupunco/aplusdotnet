using System;

using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

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
