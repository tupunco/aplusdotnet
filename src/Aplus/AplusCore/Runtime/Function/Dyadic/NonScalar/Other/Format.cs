using System;
using System.Collections.Generic;
using System.Globalization;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class Format : AbstractDyadicFunction
    {
        #region Format information

        struct FormatInfo
        {
            /// <summary>
            /// Specifes the total length of the resulting format.
            /// </summary>
            internal int TotalLength;

            /// <summary>
            /// Specifies the length for the fractional part of the format.
            /// </summary>
            internal int FractionLength;
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            ValidateInput(left, right);

            List<FormatInfo> formaters = new List<FormatInfo>();
            ExtractFormatInfo(left, formaters);
            return FormatArray(right, formaters);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Perform type and length check for the arguments
        /// </summary>
        /// <param name="left">Format info.</param>
        /// <param name="right">Items to format.</param>
        private void ValidateInput(AType left, AType right)
        {
            bool typeOK = (left.IsNumber || left.Type == ATypes.ANull)
                && (right.IsNumber || right.Type == ATypes.ASymbol || right.Type == ATypes.ANull);

            if (!typeOK)
            {
                throw new Error.Type(TypeErrorText);
            }

            if (right.Type == ATypes.ASymbol && !right.SimpleSymbolArray())
            {
                throw new Error.Type(TypeErrorText);
            }

            if (left.IsArray)
            {
                int length = left.Shape.Product();

                if (right.Shape.Count == 0 || length != right.Shape[right.Rank - 1])
                {
                    throw new Error.Length(LengthErrorText);
                }
            }
        }

        /// <summary>
        /// Extract multiple <see cref="FormatInfo"/> from the <see cref="left">argument</see>.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="formaters">List to store the <see cref="FormatInfo"/> results.</param>
        private static void ExtractFormatInfo(AType left, List<FormatInfo> formaters)
        {
            if (left.IsArray)
            {
                foreach (AType item in left)
                {
                    ExtractFormatInfo(item, formaters);
                }
            }
            else
            {
                formaters.Add(ExtractSingleFormatInfo(left));
            }
        }

        /// <summary>
        /// Cut argument 2 part: length and precesion.
        /// If argument is integer than precision is undefined.
        /// </summary>
        /// <param name="argument"></param>
        private static FormatInfo ExtractSingleFormatInfo(AType argument)
        {
            // TODO: currently this feels a bit hacky...
            string[] items = argument.asFloat.ToString(CultureInfo.InvariantCulture).Split(new char[] { '.' }, 2);

            FormatInfo format = new FormatInfo()
            {
                TotalLength = int.Parse(items[0]),
                FractionLength = int.MaxValue
            };

            if (items.Length == 2)
            {
                // the reference implementation only cares for the first digit, so do we
                format.FractionLength = int.Parse(items[1].Substring(0, 1));
            }

            return format;
        }

        #endregion

        #region Computation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="formaters"></param>
        /// <returns></returns>
        private static AType FormatArray(AType argument, List<FormatInfo> formaters)
        {
            AType result = AArray.Create(ATypes.AChar);
            AType[] formatted;

            if (argument.IsArray)
            {
                for (int i = 0; i < argument.Length; i++)
                {
                    if (argument.Rank > 1)
                    {
                        result.AddWithNoUpdate(FormatArray(argument[i], formaters));
                    }
                    else
                    {
                        FormatInfo format = formaters[formaters.Count > 1 ? i : 0];
                        formatted = argument.IsNumber
                            ? FormatNumber(argument[i], format)
                            : FormatSymbol(argument[i], format);

                        if (formatted != null)
                        {
                            result.AddRangeWithNoUpdate(formatted);
                        }
                    }
                }

                result.UpdateInfo();
            }
            else
            {
                FormatInfo format = formaters[0];
                formatted = argument.IsNumber
                    ? FormatNumber(argument, format)
                    : FormatSymbol(argument, format);

                if (formatted != null)
                {
                    result.AddRange(formatted);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert string argument to AType character array.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static AType[] ConvertToCharArray(string argument)
        {
            AType[] result = new AType[argument.Length];

            for (int i = 0; i < argument.Length; i++)
            {
                result[i] = AChar.Create(argument[i]);
            }

            return result;
        }

        /// <summary>
        /// Argument is formatted in their displayed form with
        /// backquotes removed. Each of them is right justified in
        /// the specified width/length, after being truncated on the right
        /// if it is too long.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static AType[] FormatSymbol(AType argument, FormatInfo format)
        {
            string result = format.TotalLength < 0 ? ' ' + argument.asString : argument.asString;

            return ConvertToCharArray(Pad(result, format.TotalLength));
        }

        /// <summary>
        /// Format an AType scalar number to a AType character representation.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="format"></param>
        /// <remarks>
        /// Exponential format: the result for a postive number is padded on the left with two blanks
        /// and for a negative with one, and this padded result is left justified within thw width, after
        /// being truncated on the right if it is too long.
        /// Otherwise: the result is right justified within the width, after being truncated on the right
        /// if it is too long.
        /// </remarks>
        /// <returns></returns>
        private static AType[] FormatNumber(AType argument, FormatInfo format)
        {
            if (format.TotalLength == 0)
            {
                return null;
            }

            string result;

            if (format.TotalLength < 0)
            {
                string formatString = "0.";

                if (format.FractionLength != int.MaxValue)
                {
                    formatString = formatString.PadRight(2 + format.FractionLength, '0');
                }

                formatString += "e+00";
                result = argument.asFloat.ToString(formatString, CultureInfo.InvariantCulture);
                result = result.PadLeft(result.Length + (result[0] == '-' ? 1 : 2));
                result = Pad(result, format.TotalLength);
            }
            else
            {
                if (format.FractionLength != int.MaxValue)
                {
                    result = argument.asFloat.ToString("f" + format.FractionLength, CultureInfo.InvariantCulture);
                }
                else
                {
                    result = Math.Round(argument.asFloat).ToString(CultureInfo.InvariantCulture);
                }

                result = Pad(result, format.TotalLength);
            }

            return ConvertToCharArray(result);
        }

        /// <summary>
        /// (Left or right) Pad the string argument with the given length
        /// and if the result length is too long then cut it.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="formatLength"></param>
        /// <returns></returns>
        private static string Pad(string argument, int formatLength)
        {
            string result;
            int length = Math.Abs(formatLength);

            if (argument.Length < length)
            {
                result = formatLength < 0
                    ? argument.PadRight(length)
                    : argument.PadLeft(length);
            }
            else if (argument.Length > length)
            {
                result = argument.Substring(0, length);
            }
            else
            {
                result = argument;
            }

            return result;
        }

        #endregion
    }
}
