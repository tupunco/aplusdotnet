using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPLess : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.LessThan.Execute(right, left, env);
        }
    }
}
