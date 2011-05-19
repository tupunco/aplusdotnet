using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPSubtract : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.Subtract.Execute(right, left, env);
        }
    }
}
