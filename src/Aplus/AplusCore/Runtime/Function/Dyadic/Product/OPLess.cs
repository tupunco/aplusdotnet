using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPLess : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, Aplus env)
        {
            return DyadicFunctionInstance.LessThan.Execute(right, left, env);
        }
    }
}
