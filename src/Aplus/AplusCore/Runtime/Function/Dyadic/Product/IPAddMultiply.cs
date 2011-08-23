using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    /// <summary>
    /// Performs inner product  +.*  operator
    /// </summary>
    class IPAddMultiply : InnerProduct
    {
        protected override AType Calculate(AType left, AType right, Aplus env)
        {
            AType inner = DyadicFunctionInstance.Multiply.Execute(right, left, env);
            AType result = MonadicFunctionInstance.ReduceAdd.Execute(inner, env);
            return result;
        }
    }
}
