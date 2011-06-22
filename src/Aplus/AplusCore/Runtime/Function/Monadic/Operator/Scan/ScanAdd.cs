using System.Linq;

using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Scan
{
    class ScanAdd : Scan
    {
        protected override void SetVariables(AType argument)
        {
            this.function = DyadicFunctionInstance.Add;

            if (argument.Length == 0)
            {
                this.type = argument.Type == ATypes.AFloat ? ATypes.AFloat : ATypes.AInteger;
            }
            else
            {
                this.type = argument.Type;
            }
        }

        protected override AType PostProcess(AType argument, AType result)
        {
            // if there is any float in the integer list, convert the whole list to float
            if (argument.Type == ATypes.AInteger && result.Any(item => item.Type == ATypes.AFloat))
            {
                result.ConvertToFloat();
            }

            return result;
        }
    }
}
