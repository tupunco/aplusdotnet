using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Relational
{
    /// <remarks>
    /// Input:
    /// (AInteger,AInteger)                                     ->  AInteger: 0,1
    /// (AFloat, AFloat)                                        ->  AInteger: 0,1
    /// (AInteger,AFloat: TolerablyInteger)                     ->  AInteger: 0,1
    /// (AFloat: TolerablyInteger, AInteger)                    ->  AInteger: 0,1
    /// (ASymbol,ASymbol)                                       ->  AInteger: 0,1
    /// (AChar,AChar)                                           ->  AInteger: 0,1
    /// (ABox,ABox)                                             ->  AInteger: 0,1
    /// </remarks>
    /// 
    class EqualTo : DyadicScalar
    {

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return FloatEqual(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            int number = (leftArgument.asInteger == rightArgument.asInteger) ? 1 : 0;
            return AInteger.Create(number);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return FloatEqual(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return FloatEqual(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(ASymbol rightArgument, ASymbol leftArgument)
        {
            int number = (leftArgument.asString == rightArgument.asString) ? 1 : 0;
            return AInteger.Create(number);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(ABox rightArgument, ABox leftArgument)
        {
            return AInteger.Create((leftArgument == rightArgument) ? 1 : 0);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AChar rightArgument, AChar leftArgument)
        {
            return AInteger.Create((leftArgument == rightArgument) ? 1 : 0);
        }

        // Every other case we return a 0;
        [DyadicScalarMethod]
        public  AType ExecuteDefault(AType rightArgument, AType leftArgument)
        {
            return AInteger.Create(0);
        }

        private AType FloatEqual(AType right, AType left)
        {
            int number = (Utils.ComparisonTolerance(left.asFloat, right.asFloat)) ? 1 : 0;
            return AInteger.Create(number);
        }

    }
}
