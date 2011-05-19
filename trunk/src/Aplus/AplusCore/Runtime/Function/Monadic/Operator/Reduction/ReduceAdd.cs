using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Reduction
{
    class ReduceAdd : Reduction
    {
        protected override void SetVariables(ATypes type)
        {
            if (type == ATypes.AFloat)
            {
                this.type = ATypes.AFloat;
                this.fillerElement = AFloat.Create(0);
            }
            else
            {
                this.type = ATypes.AInteger;
                this.fillerElement = AInteger.Create(0);
            }

            this.function = DyadicFunctionInstance.Add;
        }
    }
}
