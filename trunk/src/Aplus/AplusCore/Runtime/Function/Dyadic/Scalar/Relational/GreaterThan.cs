using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Relational
{
    /// <remarks>
    /// Input:                  Output:
    /// (AFloat,AFloat)         ->  AInteger : 0,1
    /// (AInteger,AInteger)     ->  AInteger : 0,1
    /// (AFloat,AInteger)       ->  AInteger : 0,1
    /// (AInteger,AFloat)       ->  AInteger : 0,1
    /// (AChar,AChar)           ->  AInteger : 0,1
    /// (ASymbol,ASymbol)       ->  AInteger : 0,1
    /// 
    /// Exceptions: Type
    /// </remarks>
    [DefaultResult(ATypes.AInteger)]
    class GreaterThan : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return FloatGT(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            int number = (leftArgument.asInteger > rightArgument.asInteger) ? 1 : 0;
            return AInteger.Create(number);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return FloatGT(rightArgument, leftArgument);
        }
        
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return FloatGT(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AChar rightArgument, AChar leftArgument)
        {
            int number = (leftArgument.asChar > rightArgument.asChar) ? 1 : 0;
            return AInteger.Create(number);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(ASymbol rightArgument, ASymbol leftArgument)
        {
            int result = String.Compare(leftArgument.asString, rightArgument.asString) == 1 ? 1 : 0;
            return AInteger.Create(result);
        }

        private AType FloatGT(AType right, AType left)
        {
            int number = (!Utils.ComparisonTolerance(left.asFloat, right.asFloat) && left.asFloat > right.asFloat) ? 1 : 0;
            return AInteger.Create(number);
        }

    }
}
