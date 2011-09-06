using System;
using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;
using System.Text;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class Cast : AbstractDyadicFunction
    {
        private delegate AType Converter(AType argument, CastInfo arguments);

        #region Nested class for argument pass

        class CastInfo
        {
            private Converter converter;
            private ATypes resultingType;
            private int longestSymbolLength;

            public CastInfo(ATypes resultingType)
            {
                this.resultingType = resultingType;
            }

            internal Converter Converter
            {
                get { return this.converter; }
                set { this.converter = value; }
            }

            internal ATypes ResultingType
            {
                get { return this.resultingType; }
            }

            internal int LongestSymbolLength
            {
                get { return this.longestSymbolLength; }
                set { this.longestSymbolLength = value; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            // type check
            if (left.Type != ATypes.ASymbol)
            {
                throw new Error.Domain(DomainErrorText);
            }

            AType scalar;

            // get the first scalar value with length check on
            if (!left.TryFirstScalar(out scalar, true))
            {
                throw new Error.Domain(DomainErrorText);
            }

            AType result;

            // select the correspond function for convert
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

        #endregion

        #region Common

        /// <summary>
        /// Walk argument array/scalar and convert each item
        /// with the converter function.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private AType Walker(AType argument, CastInfo castInfo)
        {
            AType result;

            if (argument.IsArray)
            {
                if (argument.Rank == 1 && castInfo.ResultingType == ATypes.ASymbol)
                {
                    result = castInfo.Converter(argument, castInfo);
                }
                else
                {
                    result = AArray.Create(castInfo.ResultingType);

                    foreach (AType item in argument)
                    {
                        result.Add(Walker(item, castInfo));
                    }
                }
            }
            else
            {
                result = castInfo.Converter(argument, castInfo);
            }

            return result;
        }

        /// <summary>
        /// Convert number to 'primitve' char.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static char ConvertNumberConstantToChar(AType argument)
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
            AType result;
            CastInfo castInfo = new CastInfo(ATypes.ASymbol);

            switch (argument.Type)
            {
                case ATypes.ASymbol:
                case ATypes.ANull:
                    result = argument.Clone();
                    break;

                case ATypes.AChar:
                    result = MonadicFunctionInstance.Pack.Execute(argument);
                    break;

                case ATypes.AInteger:
                case ATypes.AFloat:
                    castInfo.Converter = new Converter(ConvertNumberConstantToSymbolConstant);
                    result = Walker(argument, castInfo);
                    break;

                default:
                    throw new Error.Domain(DomainErrorText);
            }

            return result;
        }

        /// <summary>
        /// Convert number array/scalar to symbol constant.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private AType ConvertNumberConstantToSymbolConstant(AType argument, CastInfo castInfo)
        {
            StringBuilder builder = new StringBuilder();

            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    builder.Append(ConvertNumberConstantToChar(item));
                }
            }
            else
            {
                builder.Append(ConvertNumberConstantToChar(argument));
            }

            return ASymbol.Create(builder.ToString());
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
            AType result;
            CastInfo castInfo = new CastInfo(ATypes.AChar);

            switch (argument.Type)
            {
                case ATypes.AChar:
                    result = argument.Clone();
                    break;

                case ATypes.ANull:
                    result = Utils.ANull(ATypes.AChar);
                    break;

                case ATypes.ASymbol:
                    result = MonadicFunctionInstance.Unpack.Execute(argument);
                    break;

                case ATypes.AInteger:
                case ATypes.AFloat:
                    castInfo.Converter = new Converter(ConvertNumberToCharacter);

                    result = Walker(argument, castInfo);
                    break;

                default:
                    throw new Error.Domain(DomainErrorText);
            }

            return result;
        }

        /// <summary>
        /// Convert number to char constant.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private AType ConvertNumberToCharacter(AType item, CastInfo castInfo)
        {
            return AChar.Create(ConvertNumberConstantToChar(item));
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
            AType result;
            CastInfo castInfo = new CastInfo(ATypes.AFloat);

            switch (argument.Type)
            {
                case ATypes.AFloat:
                    result = argument.Clone();
                    break;

                case ATypes.ANull:
                    result = Utils.ANull(ATypes.AFloat);
                    break;

                case ATypes.AInteger:
                    castInfo.Converter = new Converter(ConvertIntegerToFloat);
                    result = Walker(argument, castInfo);
                    break;

                case ATypes.AChar:
                    castInfo.Converter = new Converter(ConvertCharacterToNumber);
                    result = Walker(argument, castInfo);
                    break;

                case ATypes.ASymbol:
                    if (!argument.SimpleSymbolArray())
                    {
                        throw new Error.Type(TypeErrorText);
                    }

                    DetectLongestSymbol(argument, castInfo);
                    castInfo.Converter = new Converter(ConvertSymbolConstantToNumber);
                    result = Walker(argument, castInfo);
                    break;

                default:
                    throw new Error.Domain(DomainErrorText);
            }

            return result;
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
        /// <param name="arguments"></param>
        /// <returns></returns>
        private AType IntegerCase(AType argument)
        {
            AType result;
            CastInfo castInfo = new CastInfo(ATypes.AInteger);

            switch (argument.Type)
            {
                case ATypes.AInteger:
                    result = argument.Clone();
                    break;

                case ATypes.ANull:
                    result = Utils.ANull(ATypes.AInteger);
                    break;

                case ATypes.AFloat:
                    castInfo.Converter = new Converter(ConvertToInteger);
                    result = Walker(argument, castInfo);
                    break;

                case ATypes.AChar:
                    castInfo.Converter = new Converter(ConvertCharacterToNumber);
                    result = Walker(argument, castInfo);
                    break;

                case ATypes.ASymbol:
                    if (!argument.SimpleSymbolArray())
                    {
                        throw new Error.Type(TypeErrorText);
                    }

                    DetectLongestSymbol(argument, castInfo);
                    castInfo.Converter = new Converter(ConvertSymbolConstantToNumber);
                    result = Walker(argument, castInfo);
                    break;

                default:
                    throw new Error.Domain(DomainErrorText);
            }

            return result;
        }

        /// <summary>
        /// Find the longest symbol in the symbol array.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="castInfo"></param>
        private void DetectLongestSymbol(AType argument, CastInfo castInfo)
        {
            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    DetectLongestSymbol(item, castInfo);
                }
            }
            else
            {
                castInfo.LongestSymbolLength = Math.Max(argument.asString.Length, castInfo.LongestSymbolLength);
            }
        }

        /// <summary>
        /// Convert symbol array/scalar to float or integer constant.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private AType ConvertSymbolConstantToNumber(AType item, CastInfo castInfo)
        {
            AType result = AArray.Create(castInfo.ResultingType);

            string temp = item.asString.PadRight(castInfo.LongestSymbolLength);

            foreach (char character in temp)
            {
                result.AddWithNoUpdate(ConvertToNumber(character, castInfo));
            }

            result.Length = temp.Length;
            result.Shape = new List<int>() { temp.Length };
            result.Rank = 1;

            return result;
        }

        /// <summary>
        /// Convert float constant to integer constant.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private AType ConvertToInteger(AType item, CastInfo castInfo)
        {
            double value = Math.Round(item.asFloat, MidpointRounding.AwayFromZero);

            if (!(int.MinValue <= value && value <= int.MaxValue && value % 1 == 0))
            {
                throw new Error.Domain(DomainErrorText);
            }

            return AInteger.Create((int)value);
        }

        /// <summary>
        /// Convert integer constant to float constant.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private static AType ConvertIntegerToFloat(AType item, CastInfo castInfo)
        {
            return AFloat.Create(item.asInteger);
        }

        /// <summary>
        /// Convert char constant to float or integer constant.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private static AType ConvertCharacterToNumber(AType item, CastInfo castInfo)
        {
            return ConvertToNumber(item.asChar, castInfo);
        }

        /// <summary>
        /// Convert char to float or integer constant.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="castInfo"></param>
        /// <returns></returns>
        private static AType ConvertToNumber(char character, CastInfo castInfo)
        {
            int number = (int)character;

            AType result = 
                (castInfo.ResultingType == ATypes.AInteger)
                ? result = AInteger.Create(number)
                : result = AFloat.Create(number);

            return result;
        }

        #endregion
    }
}
