using System;
using System.Collections.Generic;
using System.Globalization;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class Format : AbstractDyadicFunction
    {
        #region Variables

        private List<int[]> formaters;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            Checks(left, right);

            this.formaters = new List<int[]>();

            PrepareFormaters(left);

            return FormatArray(right);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Type and length check.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void Checks(AType left, AType right)
        {
            if(!Util.TypeCorrect(right.Type, left.Type,
                "FF", "II", "FI", "IF", "FS", "IS", "NI", "NF", "NS", "FN", "IN", "NN"))
            {
                throw new Error.Type(TypeErrorText);
            }

            if (right.Type == ATypes.ASymbol && !right.SimpleSymbolArray())
            {
                throw new Error.Type(TypeErrorText);
            }

            if (left.IsArray)
            {
                int length = 1;

                for (int i = 0; i < left.Shape.Count; i++)
                {
                    length *= left.Shape[i];
                }

                if(right.Shape.Count == 0 || length != right.Shape[right.Rank -1])
                {
                    throw new Error.Length(LengthErrorText);
                }
            }
        }

        /// <summary>
        /// Walk each item in the array and call PrepareItem function.
        /// </summary>
        /// <param name="left"></param>
        private void PrepareFormaters(AType left)
        {
            if (left.IsArray)
            {
                foreach (AType item in left)
                {
                    PrepareFormaters(item);
                }
            }
            else
            {
                PrepareItem(left);
            }
        }

        /// <summary>
        /// Cut argument 2 part: length and precesion.
        /// If argument is integer than precision is undefined.
        /// </summary>
        /// <param name="argument"></param>
        private void PrepareItem(AType argument)
        {
            string item = argument.asFloat.ToString(CultureInfo.InvariantCulture);
            int index = item.IndexOf(".");
            int[] pair = new int[2];

            if (index != -1)
            {
                pair[0] = int.Parse(item.Substring(0, index));
                pair[1] = int.Parse(item.Substring(index + 1, 1));
            }
            else
            {
                pair[0] = int.Parse(item);
                pair[1] = int.MaxValue;
            }

            this.formaters.Add(pair);
        }

        #endregion

        #region Computation

        /// <summary>
        /// Walk each item in the argument array and call FormatNumber function,
        /// if items are floats or integers else call FormatSymbol function.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType FormatArray(AType argument)
        {
            AType result = AArray.Create(ATypes.AChar);
            AType[] r;
            int index;

            if (argument.IsArray)
            {
                for (int i = 0; i < argument.Length; i++)
                {
                    if (argument.Rank > 1)
                    {
                        result.AddWithNoUpdate(FormatArray(argument[i]));
                    }
                    else
                    {
                        index = this.formaters.Count > 1 ? i : 0;

                        r = argument.IsNumber ? FormatNumber(argument[i], index) : FormatSymbol(argument[i], index);

                        if (r != null)
                        {
                            result.AddRangeWithNoUpdate(r);
                        }
                    }
                }

                result.UpdateInfo();
            }
            else
            {
                r = argument.IsNumber ? FormatNumber(argument, 0) : FormatSymbol(argument, 0);

                if (r != null)
                {
                    result.AddRange(r);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert string argument to Character array.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType[] ConvertToCharArray(string argument)
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
        /// <param name="index"></param>
        /// <returns></returns>
        private AType[] FormatSymbol(AType argument, int index)
        {
            string result = this.formaters[index][0] < 0 ? ' ' + argument.asString : argument.asString;

            return ConvertToCharArray(Pad(result, index));
        }

        /// <summary>
        /// Exponential format: the result for a postive number is padded on the left with two blanks
        /// and for a negative with one, and this padded result is left justified within thw width, after
        /// being truncated on the right if it is too long.
        /// Otherwise: the result is right justified within the width, after being truncated on the right
        /// if it is too long.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private AType[] FormatNumber(AType argument, int index)
        {
            if (this.formaters[index][0] == 0)
            {
                return null;
            }

            string result;

            if (this.formaters[index][0] < 0)
            {
                string formatString = "0.";

                if (this.formaters[index][1] != int.MaxValue)
                {
                    formatString = formatString.PadRight(2 + this.formaters[index][1], '0');
                }

                formatString += "e+00";
                result = argument.asFloat.ToString(formatString, CultureInfo.InvariantCulture);
                result = result.PadLeft(result.Length + (result[0] == '-' ? 1 : 2));
                result = Pad(result, index);
            }
            else
            {
                if (this.formaters[index][1] != int.MaxValue)
                {
                    result = argument.asFloat.ToString("f" + this.formaters[index][1], CultureInfo.InvariantCulture);
                }
                else
                {
                    result = Math.Round(argument.asFloat).ToString(CultureInfo.InvariantCulture);
                }

                result = Pad(result, index);
            }

            return ConvertToCharArray(result);
        }

        /// <summary>
        /// (Left or right) Pad the string argument with formaters[index],
        /// and if the result length/width is too long than we cut it.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string Pad(string argument, int index)
        {
            int length = Math.Abs(this.formaters[index][0]);

            string result = argument;

            if (result.Length < length)
            {
                result = this.formaters[index][0] < 0 ? result.PadRight(length) : result.PadLeft(length);
            }
            else if (result.Length > length)
            {
                result = result.Substring(0, length);
            }

            return result;
        }

        #endregion
    }
}
