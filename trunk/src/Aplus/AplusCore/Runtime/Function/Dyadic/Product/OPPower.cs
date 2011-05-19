using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPPower : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.Power.Execute(right, left, env);
        }
    }
}
