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
            if (!argument.ConvertToRestrictedWholeNumber(out result))
            {
                throw new Error.Type(TypeErrorText);
            }

            return AInteger.Create((result != 0) ? 1 : 0);
        }
    }
}
