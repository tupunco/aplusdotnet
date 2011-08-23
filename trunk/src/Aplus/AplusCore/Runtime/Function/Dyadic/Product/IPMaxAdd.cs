using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    /// <summary>
    /// Performs inner product  max.+  operator
    /// </summary>
    class IPMaxAdd : InnerProduct
    {
        protected override AType Calculate(AType left, AType right, Aplus env)
        {
            AType inner = DyadicFunctionInstance.Add.Execute(right, left, env);
            AType result = MonadicFunctionInstance.ReduceMax.Execute(inner, env);
            return result;
        }
    }
}
