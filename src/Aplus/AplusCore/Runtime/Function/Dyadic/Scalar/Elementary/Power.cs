using System;

using AplusCore.Types;

/*
Defintion : The number x raised to the power y.

================================
		x = (any value) :
================================
	y = 0	    |	1
	
================================
			x = -Inf :
================================
	y < 0		|	0
--------------------------------
	y > 0 and	|	-Inf
	y is even	|
--------------------------------
	y > 0 and	|	Inf
	y is odd	|
--------------------------------
	y = Inf		|	Inf
--------------------------------
	y = -Inf    	|	0
	
================================
			x < -1 :
================================
	y < 0 and 	|	x ^ y
	y is whole	|
--------------------------------
	y > 0 and 	|	x ^ y
	y is whole	|
--------------------------------
	y = Inf		|	Inf
--------------------------------
	y = -Inf	    |	0
	
================================
			x = -1 :
================================
y < 0 and 		|	-1
y is whole and	|
y is odd		|
--------------------------------
y < 0 and 		|	1
y is whole and	|
y is even		|
--------------------------------
y > 0 and 		|	1
y is whole and	|
y is even		|
--------------------------------
y > 0 and 		|	-1
y is whole and	|
y is odd		|
--------------------------------
	y = Inf		|	1
--------------------------------
	y = -Inf	    |	1
	
================================
			-1 < x < 0 :
================================
y < 0 and 		|	x ^ y
y is whole		|
--------------------------------
y > 0 and 		|	x ^ y
y is whole		|
--------------------------------
	y = Inf		|	Inf
--------------------------------
	y = -Inf    	|	0
	
================================
			x = 0 :
================================
	y < 0	 	|	Inf
--------------------------------
	y > 0	 	|	0
--------------------------------
	y = Inf		|	0
--------------------------------
	y = -Inf	    |	Inf
	
================================
			0 < x < 1 :
================================
	y < 0	 	|	x ^ y
--------------------------------
	y > 0	 	|	x ^ y
--------------------------------
	y = Inf		|	0
--------------------------------
	y = -Inf    	|	Inf
	
================================
			x = 1 :
================================
	y = Inf		|	Domain Error
--------------------------------
	y = -Inf    	|	Domain Error
--------------------------------
y = (any value)	|	1

================================
			x > 1 :
================================
	y < 0	 	|	x ^ y
--------------------------------
	y > 0	 	|	x ^ y
--------------------------------
	y = Inf		|	Inf
--------------------------------
	y = -Inf	    |	0
	
================================
			x = Inf :
================================
	y < 0	 	|	0
--------------------------------
	y > 0	 	|	Inf
--------------------------------
	y = Inf		|	Inf
--------------------------------
	y = -Inf    	|	0
 
A+ Power function compatible with C# Power function:
 
C# Math.Power source: http://msdn.microsoft.com/en-us/library/system.math.pow.aspx

*/

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Elementary
{
    class Power : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return CalculatePower(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return CalculatePower(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return CalculatePower(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return CalculatePower(rightArgument, leftArgument);
        }

        private AType CalculatePower(AType right, AType left)
        {
            double x = left.asFloat;
            double y = right.asFloat;
            double result;

            if(x == -1 && (y == Double.PositiveInfinity || y == Double.NegativeInfinity))
            {
                result = 1;
            }
            else if (x == 1 && (y == Double.PositiveInfinity || y == Double.NegativeInfinity))
            {
                throw new Error.Domain(DomainErrorText);
            }
            else
            {
                result = Math.Pow(x, y);
            }

            if (Double.IsNaN(result))
            {
                throw new Error.Domain(DomainErrorText);
            }

            return AFloat.Create(result);
        }
    }
}
