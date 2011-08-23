using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPGreaterEqual : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, Aplus env)
        {
            return DyadicFunctionInstance.GreaterThanOrEqualTo.Execute(right, left, env);
        }
    }
}
