using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Reduction
{
    class ReduceMax : Reduction
    {
        protected override void SetVariables(ATypes type)
        {
            this.type = ATypes.AFloat;
            this.fillerElement = AFloat.Create(double.NegativeInfinity);
            this.function = DyadicFunctionInstance.Max;
        }
    }
}
