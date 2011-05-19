using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPLessEqual : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.LessThanOrEqualTo.Execute(right, left, env);
        }
    }
}
