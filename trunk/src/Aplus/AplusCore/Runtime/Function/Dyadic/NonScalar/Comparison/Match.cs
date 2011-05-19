using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Comparison
{
    class Match : AbstractDyadicFunction
    {
        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            return AInteger.Create(left.Equals(right) ? 1 : 0);   
        }
    }
}
