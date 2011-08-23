using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Logical
{
    [DefaultResult(ATypes.AInteger)]
    class Not : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, Aplus environment = null)
        {
            return calculateNot(argument.asInteger);
        }

        public override AType ExecutePrimitive(AFloat argument, Aplus environment = null)
        {
            int result;
            if(argument.ConvertToRestrictedWholeNumber(out result))
            {
                return calculateNot(result);
            }
            throw new Error.Type(TypeErrorText);
        }

        private AType calculateNot(int number)
        {
            if (number == 0)
            {
                return AInteger.Create(1);
            }
            else
            {
                return AInteger.Create(0);
            }
        }

    }
}
