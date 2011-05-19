using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Logical
{
    class Or : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return CalculateOr(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return CalculateOr(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return CalculateOr(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return CalculateOr(rightArgument, leftArgument);
        }

        private AType CalculateOr(AType right, AType left)
        {
            int a, b;
            if (right.ConvertToRestrictedWholeNumber(out b) && left.ConvertToRestrictedWholeNumber(out a))
            {
                int result = (a != 0 || b != 0) ? 1 : 0;
                return AInteger.Create(result);
            }

            throw new Error.Type(this.TypeErrorText);
        }
    }
}
