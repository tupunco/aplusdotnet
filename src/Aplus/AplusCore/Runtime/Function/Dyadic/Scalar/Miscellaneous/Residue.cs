using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Miscellaneous
{
    class Residue : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            int result;
            int x = rightArgument.asInteger;
            int y = leftArgument.asInteger;
            if (y == 0)
            {
                result = x;
            }
            else
            {
                result = (int)ModularArithmetic(x, y);
            }
            return AInteger.Create(result);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return CalculateResidue(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return CalculateResidue(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return CalculateResidue(rightArgument, leftArgument);
        }

        private AType CalculateResidue(AType right, AType left)
        {
            double result;
            double x = right.asFloat;
            double y = left.asFloat;

            if (y == 0)
            {
                result = x;
            }
            else if (y == Double.PositiveInfinity || y == Double.NegativeInfinity)
            {
                result = 0;
            }
            else
            {
                double roundedx;
                if (Utils.TryComprasionTolarence(x, out roundedx))
                {
                    result = ModularArithmetic(roundedx,y);
                }
                else
                {
                    result = ModularArithmetic(x, y); ;
                }
            }
            return AFloat.Create(result);
        }

        private double ModularArithmetic(double x, double y)
        {
            return x - y * Math.Floor(x / y);
        }
    }
}
