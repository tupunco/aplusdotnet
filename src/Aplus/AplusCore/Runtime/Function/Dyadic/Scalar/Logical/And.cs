using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Logical
{
    /// <remarks>
    /// Input:
    /// (AInteger,AInteger)                                     ->  AInteger: 0,1
    /// (AFloat: TolerablyInteger, AFloat: TolerablyInteger)    ->  AInteger: 0,1
    /// (AInteger,AFloat: TolerablyInteger)                     ->  AInteger: 0,1
    /// (AFloat: TolerablyInteger, AInteger)                    ->  AInteger: 0,1
    /// 
    /// Exceptions: Type
    /// </remarks>

    class And : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return CalculateAnd(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return CalculateAnd(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return CalculateAnd(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return CalculateAnd(rightArgument, leftArgument);
        }

        private AType CalculateAnd(AType right, AType left)
        {
            int a, b;
            if (right.ConvertToRestrictedWholeNumber(out b) && left.ConvertToRestrictedWholeNumber(out a))
            {
                int result = (a != 0 && b != 0) ? 1 : 0;
                return AInteger.Create(result);
            }

            throw new Error.Type(this.TypeErrorText);
        }

    }
}
