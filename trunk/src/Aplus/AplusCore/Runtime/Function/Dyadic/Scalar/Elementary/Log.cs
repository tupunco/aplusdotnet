using System;

using AplusCore.Types;

/*
Defintion : The logarithm of x to the base y.

================================
			y < 0 :
================================
x = (any value)	|	Domain Error

================================
			y = 0 :
================================
	x = 0		|	Domain Error
--------------------------------
	x > 0		|	0
--------------------------------
	x = Inf		|	Domain Error

================================
		(0 < y < 1) :
================================
	x = 0		|	Inf
--------------------------------
	x > 0		|	log_y x
--------------------------------
	x = Inf		|	-Inf

================================
			y = 1 :
================================
	x > 1		|	Inf
--------------------------------
	x = 1		|	Domain Error
--------------------------------
	0 <= x < 1	|	-Inf

================================
			y > 1 :
================================
	x = 0		|	-Inf
--------------------------------
	x > 0		|	log_y x
--------------------------------
	x = Inf		|	Inf

================================
			y = Inf :
================================
	x = 0		|	Domain Error
--------------------------------
	x > 0		|	0
--------------------------------
	x = Inf		|	Domain Error

================================
		y = (any value)	:
================================
	x < 0		|	Domain Error

A+ Logarithm function compatible with C# Logarithm function:

C# Math.Log source: http://msdn.microsoft.com/en-us/library/hd50b6h5.aspx

*/

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Elementary
{
    [DefaultResult(ATypes.AFloat)]
    class Log : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            return CalculateLog(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            return CalculateLog(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            return CalculateLog(rightArgument, leftArgument);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            return CalculateLog(rightArgument, leftArgument);
        }

        private AType CalculateLog(AType right, AType left)
        {
            double x = right.asFloat;
            double y = left.asFloat;
            double result;

            if (y < 0 || x < 0)
            {
                throw new Error.Domain(DomainErrorText);
            }
            else if (y == 0)
            {
                if(x == 0 || x == Double.PositiveInfinity)
                {
                    throw new Error.Domain(DomainErrorText);
                }
                else //x > 0
                {
                    result = 0;
                }
            }
            else if (y > 0 && y < 1)
            {
                if (x == 0)
                {
                    result = Double.PositiveInfinity;
                }
                else if (x == Double.PositiveInfinity)
                {
                    result = Double.NegativeInfinity;
                }
                else //x > 0
                {
                    result = Math.Log(x, y);
                }
            }
            else if (y == 1)
            {
                if (x == 1)
                {
                    throw new Error.Domain(DomainErrorText);
                }
                else if (x > 1)
                {
                    result = Double.PositiveInfinity;
                }
                else //0 <= x < 1
                {
                    result = Double.NegativeInfinity;
                }
            }
            else if (y == Double.PositiveInfinity)
            {
                if (x == 0 || x == Double.PositiveInfinity)
                {
                    throw new Error.Domain(DomainErrorText);
                }
                else //x > 0
                {
                    result = 0;
                }
            }
            else // y > 1
            {
                if(x == Double.PositiveInfinity)
                {
                    result = Double.PositiveInfinity;
                }
                else if(x == 0)
                {
                    result = Double.NegativeInfinity;
                }
                else //x > 0
                {
                    result = Math.Log(x, y);
                }
            }
            return AFloat.Create(result);
        }
    }
}
