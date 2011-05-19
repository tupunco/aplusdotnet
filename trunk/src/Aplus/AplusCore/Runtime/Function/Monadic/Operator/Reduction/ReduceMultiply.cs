using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Reduction
{
    class ReduceMultiply : Reduction
    {
        protected override void SetVariables(ATypes type)
        {
            if (type == ATypes.AFloat)
            {
                this.type = ATypes.AFloat;
                this.fillerElement = AFloat.Create(1);
            }
            else
            {
                this.type = ATypes.AInteger;
                this.fillerElement = AInteger.Create(1);
            }

            this.function = DyadicFunctionInstance.Multiply;
        }
    }
}
