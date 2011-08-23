using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPEqual : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, Aplus env)
        {
            return DyadicFunctionInstance.EqualTo.Execute(right, left, env);
        }
    }
}
