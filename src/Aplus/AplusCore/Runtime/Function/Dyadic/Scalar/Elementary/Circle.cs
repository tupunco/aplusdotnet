using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Scalar.Elementary
{
    /// <remarks>
    /// Input:                                  Output:
    /// (ASymbol,AInteger)                  ->  AFloat
    /// (ASymbol,AFloat)                    ->  AFloat
    /// (-7 <= AInteger <= 7, AInteger)     ->  AFloat
    /// (-8.0 < AFloat < 8.0, AInteger)     ->  AFloat
    /// 
    /// ASymbol: sinarccos, sin, cos, tan, secarctan, sinh, cosh, tanh, arcsin, arccos, arctan, tanarcsec, arcsinh, arccosh, arctanh
    /// 
    /// Exeception: Type, Domain
    /// </remarks>
    [DefaultResult(ATypes.AFloat)]
    class Circle : DyadicScalar
    {
        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AInteger leftArgument)
        {
            DomainCheck(leftArgument.asInteger);
            return Calculate(rightArgument.asInteger, leftArgument.asInteger);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AFloat leftArgument)
        {
            DomainCheck(leftArgument.asInteger);
            return Calculate(rightArgument.asFloat, leftArgument.asInteger);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, AFloat leftArgument)
        {
            DomainCheck(leftArgument.asInteger);
            return Calculate(rightArgument.asInteger, leftArgument.asInteger);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, AInteger leftArgument)
        {
            DomainCheck(leftArgument.asInteger);
            return Calculate(rightArgument.asFloat, leftArgument.asInteger);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AInteger rightArgument, ASymbol leftArgument)
        {
            int casenumber = ConvertSymbolName(leftArgument.asString);
            return Calculate(rightArgument.asInteger, casenumber);
        }

        [DyadicScalarMethod]
        public AType ExecutePrimitive(AFloat rightArgument, ASymbol leftArgument)
        {
            int casenumber = ConvertSymbolName(leftArgument.asString);
            return Calculate(rightArgument.asFloat, casenumber);
        }

        /// <summary>
        /// Checks if the input number is a valid method chooses for circle
        /// </summary>
        /// <param name="number"></param>
        /// <exception cref="Error.Domain">If number is not in range [-7;7]</exception>
        private void DomainCheck(int number)
        {
            if (!(-7 <= number && number <= 7))
            {
                throw new Error.Domain(DomainErrorText);
            }
        }

        #region Calculations

        private AType Calculate(double argument, int number)
        {
            Check(argument, number);

            double result = 0;
            switch (number)
            {
                case 0:
                    result = Math.Pow(1 - Math.Pow(argument, 2), 0.5);
                    break;
                case 1:
                    result = Math.Sin(argument);
                    break;
                case 2:
                    result = Math.Cos(argument);
                    break;
                case 3:
                    result = Math.Tan(argument);
                    break;
                case 4:
                    result = Math.Pow(1 + Math.Pow(argument, 2), 0.5);
                    break;
                case 5:
                    result = Math.Sinh(argument);
                    break;
                case 6:
                    result = Math.Cosh(argument);
                    break;
                case 7:
                    result = Math.Tanh(argument);
                    break;
                case -1:
                    result = Math.Asin(argument);
                    break;
                case -2:
                    result = Math.Acos(argument);
                    break;
                case -3:
                    result = Math.Atan(argument);
                    break;
                case -4:
                    result = Math.Pow(-1 + Math.Pow(argument, 2), 0.5);
                    break;
                case -5:
                    //ArcSinh : http://mathworld.wolfram.com/InverseHyperbolicSine.html
                    if (argument == Double.NegativeInfinity)
                    {
                        result = Double.NegativeInfinity;
                    }
                    else
                    {
                        result = Math.Log(argument + Math.Sqrt(1 + Math.Pow(argument, 2)));
                    }
                    break;
                case -6:
                    //ArcCosh : http://mathworld.wolfram.com/InverseHyperbolicCosine.html
                    result = Math.Log(argument + Math.Sqrt(argument + 1) * Math.Sqrt(argument - 1));
                    break;
                case -7:
                    //ArcTanh : http://mathworld.wolfram.com/InverseHyperbolicTangent.html
                    result = 0.5 * (Math.Log(1 + argument) - Math.Log(1 - argument));
                    break;
            }

            if (Double.IsNaN(result))
            {
                throw new Error.Domain(DomainErrorText);
            }
            else
            {
                return AFloat.Create(result);
            }
        }

        //If the argument is incorrent, we throw Domain Error execpetion.
        private void Check(double argument, int number)
        {
            switch (number)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case -7:
                    if (argument == Double.PositiveInfinity || argument == Double.NegativeInfinity)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                    break;
                case -1:
                case -2:
                    if (argument > 1 || argument < -1)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                    break;
                case -6:
                    if (argument == Double.NegativeInfinity)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                    break;

            }
        }

        private int ConvertSymbolName(string argument)
        {
            switch (argument)
            {
                case "sinarccos":
                    return 0;
                case "sin":
                    return 1;
                case "cos":
                    return 2;
                case "tan":
                    return 3;
                case "secarctan":
                    return 4;
                case "sinh":
                    return 5;
                case "cosh":
                    return 6;
                case "tanh":
                    return 7;
                case "arcsin":
                    return -1;
                case "arccos":
                    return -2;
                case "arctan":
                    return -3;
                case "tanarcsec":
                    return -4;
                case "arcsinh":
                    return -5;
                case "arccosh":
                    return -6;
                case "arctanh":
                    return -7;
                default:
                    throw new Error.Domain(DomainErrorText);
            }
        }

        #endregion
    }
}
