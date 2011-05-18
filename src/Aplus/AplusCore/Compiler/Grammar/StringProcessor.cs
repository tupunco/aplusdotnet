using System;
using System.Text;

namespace AplusCore.Compiler.Grammar
{
    class StringProcessor
    {

        /// <summary>
        /// Replaces the APL high minus character to the normal minus sign.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        internal static string ProcessAPLNumber(string number)
        {
            return number.Replace((char)0xA2, '-');
        }

        internal static string ProcessEscapes(string text)
        {
            StringBuilder replacedString = new StringBuilder();
            char currentChar;
            char nextChar;

            for (int i = 0; i < text.Length; i++)
            {
                currentChar = text[i];
                if (text[i] == '\\')
                {
                    nextChar = text[i + 1];
                    if (nextChar == 'n')
                    {
                        replacedString.Append('\n');
                        i++;
                        continue;
                    }
                    else if (char.IsDigit(nextChar))
                    {
                        int sum = ConvertEscapedDigits(text, ref nextChar, ref i, 8, 3);
                        replacedString.Append(Convert.ToChar(sum));
                        continue;
                    }
                    else if (nextChar == 'x' && char.IsDigit(text[i + 2]))
                    {

                        nextChar = text[i + 2];
                        i++;
                        int sum = ConvertEscapedDigits(text, ref nextChar, ref i, 16, 2);
                        replacedString.Append(Convert.ToChar(sum));
                        continue;
                    }
                    i++;
                }
                replacedString.Append(text[i]);
            }
            return replacedString.ToString();
        }


        internal static int ConvertEscapedDigits(string text, ref char nextChar, ref int i, int radix, int digitMaxCount)
        {
            int digitCount = 0;
            int sum = 0;

            while (char.IsDigit(nextChar) && digitCount < digitMaxCount)
            {
                sum = radix * sum + (nextChar - '0');
                digitCount++;
                i++;
                nextChar = text[i + 1];
            }
            return sum;
        }
    }
}
