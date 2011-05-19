using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Reduction
{
    class ReduceMin : Reduction
    {
        protected override void SetVariables(ATypes type)
        {
            this.type = ATypes.AFloat;
            this.fillerElement = AFloat.Create(double.PositiveInfinity);
            this.function = DyadicFunctionInstance.Min;
        }
    }
}
