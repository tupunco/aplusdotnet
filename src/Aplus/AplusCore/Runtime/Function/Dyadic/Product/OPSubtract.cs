using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPSubtract : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, Aplus env)
        {
            return DyadicFunctionInstance.Subtract.Execute(right, left, env);
        }
    }
}
