using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Scan
{
    class ScanMin : Scan
    {
        protected override void SetVariables(AType argument)
        {
            this.function = DyadicFunctionInstance.Min;

            if (argument.Length == 0)
            {
                this.type = argument.Type == ATypes.AFloat ? ATypes.AFloat : ATypes.AInteger;
            }
            else
            {
                this.type = argument.Type;
            }
        }
    }
}
