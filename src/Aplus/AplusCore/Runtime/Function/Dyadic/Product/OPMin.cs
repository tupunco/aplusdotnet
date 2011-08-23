using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPMin : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, Aplus env)
        {
            return DyadicFunctionInstance.Min.Execute(right, left, env);
        }
    }
}
