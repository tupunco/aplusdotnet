using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPMultiply : OuterProduct
    {
        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.Multiply.Execute(right, left, env);
        }
    }
}
