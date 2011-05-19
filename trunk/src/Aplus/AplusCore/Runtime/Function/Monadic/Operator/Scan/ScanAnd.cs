using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Scan
{
    class ScanAnd : Scan
    {
        protected override void SetVariables(Types.AType argument)
        {
            this.function = DyadicFunctionInstance.And;
            this.type = ATypes.AInteger;
        }

        //If argument type is float, we have to convert the first item to integer.
        protected override AType PreProcess(AType argument)
        {
            return ConvertToInt(argument);
        }
    }
}
