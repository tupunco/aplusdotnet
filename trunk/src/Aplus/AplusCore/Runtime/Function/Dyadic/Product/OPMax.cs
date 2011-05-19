using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPMax : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.Max.Execute(right, left, env);
        }
    }
}
