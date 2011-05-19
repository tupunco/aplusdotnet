using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    class OPDivide : OuterProduct
    {
        protected override ATypes NullType { get { return ATypes.AFloat; } }

        protected override AType Calculate(AType left, AType right, AplusEnvironment env)
        {
            return DyadicFunctionInstance.Divide.Execute(right, left, env);
        }
    }
}
