using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPGreater : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.GreaterThan.Execute(right, left, env);
        }
    }
}
