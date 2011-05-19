using System;
using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class Cast : AbstractDyadicFunction
    {
        // Declare delegate
        private delegate AType Converter(AType argument);

        #region Variables

        private Converter converter;
        private ATypes type;
        private int longestSymbolLength;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            return Compute(left, right);
        }

        #endregion

        #region Computation

        private AType Compute(AType left, AType right)
        {
            //Type check
            if (left.Type != ATypes.ASymbol)
            {
                throw new Error.Domain(DomainErrorText);
            }

            AType scalar;

            // Get the first scalar value with length check on
            if (!left.TryFirstScalar(out scalar, true))
            {
                throw new Error.Domain(DomainErrorText);
            }

            AType result;

            //Select the correspond function for convert.
            switch (scalar.asString)
            {
                case "int":
                    result = IntegerCase(right);
                    break;
                case "float":
                    result = FloatCase(right);
                    break;
                case "sym":
                    result = SymbolCase(right);
                    break;
                case "char":
                    result = CharCase(right);
                    break;
                default:
                    throw new Error.Domain(DomainErrorText);
            }

            return result;
        }

        #region Common

        /// <summary>
        /// Walk argument array/scalar and convert each item
        /// with the converter function.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Walker(AType argument)
        {
            if (argument.IsArray)
            {
                if (argument.Rank == 1 && this.type == ATypes.ASymbol)
                {
                    return this.converter(argument);
                }
                else
                {
                    AType result = AArray.Create(this.type);

                    foreach (AType item in argument)
                    {
                        result.Add(Walker(item));
                    }

                    return result;
                }
            }
            else
            {
                return this.converter(argument);
            }
        }

        /// <summary>
        /// Convert number to 'primitve' char.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private char ConvertNumberConstantToChar(AType argument)
        {
            byte result;

            if (argument.Type == ATypes.AInteger)
            {
                result = (byte)(argument.asInteger % 256);
            }
            else
            {
                result = (byte)(Math.Round(argument.asFloat, MidpointRounding.AwayFromZero) % 256);
            }

            return Convert.ToChar(result);
        }

        #endregion

        #region Symbol

        /// <summary>
        /// If argument is:
        /// - char then we use Pack nonscalar function.
        /// - null or symbol we just clone it.
        /// - float or integer we use ConverToSymbolConstant function.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType SymbolCase(AType argument)
        {
            switch (argument.Type)
            {
                case ATypes.ASymbol:
                case ATypes.ANull:

                    return argument.Clone();

                case ATypes.AChar:

                    return MonadicFunctionInstance.Pack.Execute(argument);

                case ATypes.AInteger:
                case ATypes.AFloat:

                    this.converter = new Converter(ConvertNumberConstantToSymbolConstant);
                    this.type = ATypes.ASymbol;

                    return Walker(argument);

                default:

                    throw new Error.Domain(DomainErrorText);
            }
        }

        /// <summary>
        /// Convert number array/scalar to symbol constant.
        /// (Regular method that matches signature.)
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ConvertNumberConstantToSymbolConstant(AType argument)
        {
            string symbol = "";

            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    symbol += ConvertNumberConstantToChar(item);
                }
            }
            else
            {
                symbol += ConvertNumberConstantToChar(argument);
            }

            return ASymbol.Create(symbol);
        }

        #endregion

        #region Char

        /// <summary>
        /// If argument is:
        /// - char we clone it.
        /// - null then we return char null.
        /// - symbol then we use Unpack nonscalar function.
        /// - null or symbol we just clone it.
        /// - float or integer we use ConvertToCharConstant function.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType CharCase(AType argument)
        {
            switch (argument.Type)
            {
                case ATypes.AChar:

                    return argument.Clone();

                case ATypes.ANull:

                    return Utils.ANull(ATypes.AChar);

                case ATypes.ASymbol:

                    return MonadicFunctionInstance.Unpack.Execute(argument);

                case ATypes.AInteger:
                case ATypes.AFloat:

                    this.converter = new Converter(ConvertNumberConstantToCharConstant);
                    this.type = ATypes.AChar;

                    return Walker(argument);

                default:

                    throw new Error.Domain(DomainErrorText);
            }
        }

        /// <summary>
        /// Convert number to char constant.
        /// (Regular method that matches signature.)
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ConvertNumberConstantToCharConstant(AType argument)
        {
            return AChar.Create(ConvertNumberConstantToChar(argument));
        }

        #endregion

        #region Number

        /// <summary>
        /// If argument is:
        /// - float the we clone it.
        /// - null we return float null.
        /// - integer we use ConvertIntegerConstantToFloatConstant function.
        /// - char we use ConvertCharConstantToNumberConstant function.
        /// - symbol then we use ConvertSymbolConstantToNumber function.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType FloatCase(AType argument)
        {
            this.type = ATypes.AFloat;

            switch (argument.Type)
            {
                case ATypes.AFloat:

                    return argument.Clone();

                case ATypes.ANull:

                    return Utils.ANull(ATypes.AFloat);

                case ATypes.AInteger:

                    this.converter = new Converter(ConvertIntegerConstantToFloatConstant);
                    return Walker(argument);

                case ATypes.AChar:

                    this.converter = new Converter(ConvertCharConstantToNumberConstant);
                    return Walker(argument);

                case ATypes.ASymbol:

                    if (!argument.SimpleSymbolArray())
                    {
                        throw new Error.Type(TypeErrorText);
                    }

                    DetectLongestSymbol(argument);
                    this.converter = new Converter(ConvertSymbolConstantToNumber);
                    return Walker(argument);

                default:

                    throw new Error.Domain(DomainErrorText);
            }
        }

        /// <summary>
        /// If argument is:
        ///  - integer then we clone it.
        ///  - null we return integer null.
        ///  - float then we use ConvertFloatConstantToIntegerConstant function.
        ///  - char we call ConvertCharConstantToNumberConstant function.
        ///  - symbol then we use ConvertSymbolConstantToNumber function.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType IntegerCase(AType argument)
        {
            this.type = ATypes.AInteger;

            switch (argument.Type)
            {
                case ATypes.AInteger:

                    return argument.Clone();

                case ATypes.ANull:

                    return Utils.ANull(ATypes.AInteger);

                case ATypes.AFloat:

                    this.converter = new Converter(ConvertFloatConstantToIntegerConstant);
                    return Walker(argument);

                case ATypes.AChar:

                    this.converter = new Converter(ConvertCharConstantToNumberConstant);
                    return Walker(argument);

                case ATypes.ASymbol:

                    if (!argument.SimpleSymbolArray())
                    {
                        throw new Error.Type(TypeErrorText);
                    }

                    DetectLongestSymbol(argument);
                    this.converter = new Converter(ConvertSymbolConstantToNumber);
                    return Walker(argument);

                default:

                    throw new Error.Domain(DomainErrorText);
            }
        }

        /// <summary>
        /// Find the longest symbol in the symbol array.
        /// </summary>
        /// <param name="argument"></param>
        private void DetectLongestSymbol(AType argument)
        {
            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    DetectLongestSymbol(item);
                }
            }
            else
            {
                this.longestSymbolLength = Math.Max(argument.asString.Length, this.longestSymbolLength);
            }
        }

        /// <summary>
        /// Convert symbol array/scalar to float or integer constant.
        /// (Regular method that matches signature.)
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ConvertSymbolConstantToNumber(AType argument)
        {
            AType result = AArray.Create(this.type);

            string tmp = argument.asString.PadRight(this.longestSymbolLength);

            foreach (char c in tmp)
            {
                result.AddWithNoUpdate(ConvertCharToNumberConstant(c));
            }

            result.Length = tmp.Length;
            result.Shape = new List<int>() { tmp.Length };
            result.Rank = 1;

            return result;
        }

        /// <summary>
        /// Convert float constant to integer constant.
        /// (Regular method that matches signature.)
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ConvertFloatConstantToIntegerConstant(AType argument)
        {
            double item = Math.Round(argument.asFloat, MidpointRounding.AwayFromZero);

            if (int.MinValue <= item && item <= int.MaxValue)
            {
                return AInteger.Create((int)item);
            }
            else
            {
                throw new Error.Domain(DomainErrorText);
            }
        }

        /// <summary>
        /// Convert integer constant to float constant.
        /// (Regular method that matches signature.)
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ConvertIntegerConstantToFloatConstant(AType argument)
        {
            return AFloat.Create(argument.asInteger);
        }

        /// <summary>
        /// Convert char constant to float or integer constant.
        /// (Regular method that matches signature.)
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ConvertCharConstantToNumberConstant(AType argument)
        {
            return ConvertCharToNumberConstant(argument.asChar);
        }

        /// <summary>
        /// Convert char to float or integer constant.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private AType ConvertCharToNumberConstant(char c)
        {
            int number = Convert.ToInt32(c);

            if (this.type == ATypes.AInteger)
            {
                return AInteger.Create(number);
            }
            else
            {
                return AFloat.Create(number);
            }
        }

        #endregion

        #endregion
    }
}
