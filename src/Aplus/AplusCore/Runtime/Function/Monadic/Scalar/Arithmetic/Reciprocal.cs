﻿using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Arithmetic
{
    class Reciprocal : MonadicScalar
    {
        public override AType ExecutePrimitive(AInteger argument, AplusEnvironment environment = null)
        {
            return calculateReciprocal(argument.asFloat);
        }

        public override AType ExecutePrimitive(AFloat argument, AplusEnvironment environment = null)
        {
            return calculateReciprocal(argument.asFloat);
        }

        private AType calculateReciprocal(double number)
        {
            if (number == 0)
            {
                return AFloat.Create(Double.PositiveInfinity);
            }
            return AFloat.Create(1 / number);
        }
    }
}
