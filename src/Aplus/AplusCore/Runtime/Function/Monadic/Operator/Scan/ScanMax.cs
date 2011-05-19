using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Scan
{
    class ScanMax : Scan
    {
        protected override void SetVariables(AType argument)
        {
            this.function = DyadicFunctionInstance.Max;

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
