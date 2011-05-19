using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPNotEqual : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.NotEqualTo.Execute(right, left, env);
        }
    }
}
