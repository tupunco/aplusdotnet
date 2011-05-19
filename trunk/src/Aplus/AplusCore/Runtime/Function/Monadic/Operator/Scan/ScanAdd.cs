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

        //If the argument type is integer, the result can be float than
        //we have to convert all items to float.
        protected override AType PostProcess(AType argument, AType result)
        {
            if (argument.Type == ATypes.AInteger)
            {
                bool convert = false;

                foreach (AType item in result)
                {
                    if (item.Type == ATypes.AFloat)
                    {
                        convert = true;
                        break;
                    }
                }

                if (convert)
                {
                    result.ConvertToFloat();
                }
            }

            return result;
        }
    }
}
