using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Reduction
{
    class ReduceAnd : Reduction
    {
        protected override void SetVariables(ATypes type)
        {
            this.type = ATypes.AInteger;
            this.fillerElement = AInteger.Create(1);
            this.function = DyadicFunctionInstance.And;
        }

        protected override AType Process(AType argument)
        {
            int result;
            if (argument.ConvertToRestrictedWholeNumber(out result))
            {
                if (result != 0)
                {
                    return AInteger.Create(1);
                }
                else
                {
                    return AInteger.Create(0);
                }
            }
            else
            {
                throw new Error.Type(TypeErrorText);
            }
        }
    }
}
