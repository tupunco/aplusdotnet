using System;
using System.Collections.Generic;
using System.Globalization;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class DefaultFormat : AbstractMonadicFunction
    {
        #region Format information

        class FormatInformation
        {
            private int index;
            private List<string> items;
            private ATypes type;
            private int intervalMax;
            private int fractionMax;
            private bool dotFound;
            private int precision;

            internal FormatInformation(ATypes type, int printingPrecision)
            {
                this.type = type;
                this.precision = printingPrecision;

                this.index = 0;
                this.items = new List<string>();
                this.intervalMax = -1;
                this.fractionMax = -1;
                this.dotFound = false;
            }

            internal int Precision
            {
                get { return this.precision; }
            }

            internal bool DotFound
            {
                get { return dotFound; }
                set { dotFound = value; }
            }

            internal int FractionMax
            {
                get { return fractionMax; }
                set { fractionMax = value; }
            }

            internal int IntervalMax
            {
                get { return intervalMax; }
                set { intervalMax = value; }
            }

            internal ATypes Type
            {
                get { return type; }
                set { type = value; }
            }

            internal List<string> Items
            {
                get { return items; }
            }

            internal int Index
            {
                get { return index; }
                set { index = value; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType argument, Aplus environment = null)
        {
            if (argument.IsFunctionScalar)
            {
                return GetFunction(argument);
            }
            else if (argument.IsBox)
            {
                if (argument.IsArray)
                {
                    throw new Error.Rank(RankErrorText);
                }
                else
                {
                    throw new Error.Type(TypeErrorText);
                }
            }
            else if (argument.Type == ATypes.ANull)
            {
                throw new Error.Rank(RankErrorText);
            }
            else if (argument.Type == ATypes.AChar)
            {
                return argument.Clone();
            }

            // use Printing precision system variable
            int printingPrecision = (environment != null) ? environment.SystemVariables["pp"].asInteger : -1;

            switch (printingPrecision)
            {
                case -1:
                    // system variable doesn't exist, so we write whole float number.
                    printingPrecision = 0;
                    break;
                case 0:
                    // if `pp is 0, then it is reated as if it were one.
                    printingPrecision = 1;
                    break;
            }

            return Compute(argument, printingPrecision);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Create character array from argument.
        /// If argument is primitve, the resut is its symbol.
        /// If argument is defined function, the result is its defintion.
        /// Comment: Argument need not be function scalar.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType GetFunction(AType argument)
        {
            AFunc function = (AFunc)argument.Data.NestedItem.Data;

            return Helpers.BuildString(function.IsDerived ? "*derived*" : argument.NestedItem.ToString());
        }

        /// <summary>
        /// Walk the whole (array) argument to find the longest interval and fraction.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="formatInfo"></param>
        private void DetectLongestSymbol(AType argument, FormatInformation formatInfo)
        {
            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    DetectLongestSymbol(item, formatInfo);
                }
            }
            else
            {
                formatInfo.Items.Add(GetItem(argument, formatInfo));
            }
        }

        /// <summary>
        /// Convert item to string.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="info"></param>
        private string GetItem(AType item, FormatInformation info)
        {
            string result;

            switch (item.Type)
            {
                case ATypes.AInteger:
                    result = item.asInteger.ToString();
                    info.IntervalMax = Math.Max(result.Length, info.IntervalMax);
                    break;
                case ATypes.ASymbol:
                    result = "`" + item.asString;
                    info.IntervalMax = Math.Max(result.Length, info.IntervalMax);
                    break;
                default:
                    result = item.asFloat.ToString("g" + info.Precision, CultureInfo.InvariantCulture);
                    int dotPostion = result.IndexOf(".");

                    if (dotPostion != -1)
                    {
                        info.IntervalMax = Math.Max(dotPostion, info.IntervalMax);
                        info.FractionMax = Math.Max(result.Length - (dotPostion + 1), info.FractionMax);
                        info.DotFound = true;
                    }
                    else
                    {
                        info.IntervalMax = Math.Max(result.Length, info.IntervalMax);
                    }
                    break;
            }

            return result;
        }

        #endregion

        #region Computation

        private AType Compute(AType item, int printingPrecision)
        {
            FormatInformation formatInfo = new FormatInformation(item.Type, printingPrecision);

            DetectLongestSymbol(item, formatInfo);

            if (item.Rank < 2)
            {
                formatInfo.IntervalMax = -1;
                formatInfo.FractionMax = -1;
            }

            return FormatArray(item.Shape, formatInfo);
        }

        /// <summary>
        /// Execute padding on all items and make the result character array.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="arguments">Instead of class variables.</param>
        /// <returns></returns>
        private AType FormatArray(List<int> shape, FormatInformation arguments)
        {
            AType result = AArray.Create(ATypes.AChar);
            int rank = shape.Count;

            if (rank > 0)
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    if (rank > 1)
                    {
                        result.AddWithNoUpdate(FormatArray(shape.GetRange(1, rank - 1), arguments));
                    }
                    else
                    {
                        result.AddRangeWithNoUpdate(FormatScalar(arguments));
                    }
                }

                result.UpdateInfo();
            }
            else
            {
                result.AddRange(FormatScalar(arguments));
            }

            return result;
        }

        /// <summary>
        /// Execute padding on actual item (arguments.Items[index]) and
        /// create a character array.
        /// </summary>
        /// <param name="info">Instead of class variables.</param>
        /// <returns></returns>
        private AType[] FormatScalar(FormatInformation info)
        {
            int blankFront;
            int blankEnd;
            int counter = 0;

            switch (info.Type)
            {
                case ATypes.ASymbol:
                    FormatSymbol(info, out blankFront, out blankEnd);
                    break;
                case ATypes.AInteger:
                    FormatInteger(info, out blankFront, out blankEnd);
                    break;
                default:
                    FormatFloat(info, out blankFront, out blankEnd);
                    break;
            }

            AType[] result = new AType[blankFront + info.Items[info.Index].Length + blankEnd];

            for (int i = 0; i < result.Length; i++)
            {
                if (i >= blankFront && i < blankFront + info.Items[info.Index].Length)
                {
                    result[i] = AChar.Create(info.Items[info.Index][counter++]);
                }
                else
                {
                    result[i] = AChar.Create(' ');
                }
            }

            info.Index++;

            return result;
        }

        /// <summary>
        /// Determine right and left padding of symbol.
        /// </summary>
        /// <param name="arguments">Instead of class variables.</param>
        /// <param name="blankFront"></param>
        /// <param name="blankEnd"></param>
        private void FormatSymbol(FormatInformation arguments, out int blankFront, out int blankEnd)
        {
            blankFront = 1;
            blankEnd = (arguments.IntervalMax == -1)
                ? 0
                : (arguments.IntervalMax - arguments.Items[arguments.Index].Length);
        }

        /// <summary>
        /// Determine right and left padding of integer.
        /// </summary>
        /// <param name="arguments">Instead of class variables.</param>
        /// <param name="blankFront"></param>
        /// <param name="blankEnd"></param>
        private void FormatInteger(FormatInformation arguments, out int blankFront, out int blankEnd)
        {
            blankFront = (arguments.IntervalMax == -1)
                ? 1
                : (1 + arguments.IntervalMax - arguments.Items[arguments.Index].Length);
            blankEnd = 0;
        }

        /// <summary>
        /// Determine right and left padding of float.
        /// </summary>
        /// <param name="arguments">Instead of class variables.</param>
        /// <param name="blankFront"></param>
        /// <param name="blankEnd"></param>
        private void FormatFloat(FormatInformation arguments, out int blankFront, out int blankEnd)
        {
            int dotPosition = arguments.Items[arguments.Index].IndexOf(".");

            if (dotPosition != -1)
            {
                blankFront = (arguments.IntervalMax == -1)
                    ? 1
                    : (1 + arguments.IntervalMax - dotPosition);
                blankEnd = (arguments.FractionMax == -1)
                    ? 0
                    : (arguments.FractionMax - (arguments.Items[arguments.Index].Length - (dotPosition + 1)));
            }
            else
            {
                blankFront = (arguments.IntervalMax == -1)
                    ? 1
                    : 1 + (arguments.IntervalMax - arguments.Items[arguments.Index].Length);
                blankEnd = (arguments.FractionMax == -1)
                    ? 0
                    : arguments.FractionMax;

                if (arguments.DotFound)
                {
                    blankEnd++;
                }
            }
        }

        #endregion
    }
}
