using System;
using System.Collections.Generic;
using System.Globalization;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class DefaultFormat : AbstractMonadicFunction
    {
        #region Variables

        private int index;
        private List<string> items;
        private ATypes type;

        private int intervalMax;
        private int fractionMax;
        private bool dot;

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

            //Use Printing precision systam variable to write float number.
            int tmp = environment != null ? environment.SystemVariables["pp"].asInteger : -1;

            int printingPrecision;

            switch(tmp)
            {
                case -1:
                    //System variable doesn't exist, so we write whole float number.
                    printingPrecision = 0;
                    break;
                case 0:
                    //If `pp is 0, then it is reated as if it were one.
                    printingPrecision = 1;
                    break;
                default:
                    printingPrecision = tmp;
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
        /// <param name="printingPrecision"></param>
        private void DetectLongestSymbol(AType argument, int printingPrecision)
        {
            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    DetectLongestSymbol(item, printingPrecision);
                }
            }
            else
            {
                this.items.Add(GetItem(argument, printingPrecision));
            }
        }

        /// <summary>
        /// Convert items to string (with printing precision).
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="printingPrecision"></param>
        private string GetItem(AType argument, int printingPrecision)
        {
            string result;

            switch (argument.Type)
            {
                case ATypes.AInteger:
                    result = (argument.asInteger).ToString();
                    this.intervalMax = Math.Max(result.Length, this.intervalMax);
                    break;
                case ATypes.ASymbol:
                    result = "`" + argument.asString;
                    this.intervalMax = Math.Max(result.Length, this.intervalMax);
                    break;
                default:
                    result = (argument.asFloat).ToString("g" + printingPrecision, CultureInfo.InvariantCulture);

                    int ind = result.IndexOf(".");

                    if (ind != -1)
                    {
                        this.intervalMax = Math.Max(ind, this.intervalMax);
                        this.fractionMax = Math.Max(result.Length - (ind + 1), this.fractionMax);
                        this.dot = true;
                    }
                    else
                    {
                        this.intervalMax = Math.Max(result.Length, this.intervalMax);
                    }
                    break;
            }

            return result;
        }

        #endregion

        #region Computation

        private AType Compute(AType argument, int printingPrecision)
        {
            this.index = 0;
            this.items = new List<string>();
            this.type = argument.Type;

            this.intervalMax = -1;
            this.fractionMax = -1;
            this.dot = false;

            DetectLongestSymbol(argument, printingPrecision);

            if (argument.Rank < 2)
            {
                this.intervalMax = -1;
                this.fractionMax = -1;
            }

            return FormatArray(argument.Shape);
        }

        /// <summary>
        /// Execute padding on all items and make the result character array.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        private AType FormatArray(List<int> shape)
        {
            AType result = AArray.Create(ATypes.AChar);
            int rank = shape.Count;

            if (rank > 0)
            {
                for (int i = 0; i < shape[0]; i++)
                {
                    if (rank > 1)
                    {
                        result.AddWithNoUpdate(FormatArray(shape.GetRange(1, rank - 1)));
                    }
                    else
                    {
                        result.AddRangeWithNoUpdate(FormatScalar());
                    }
                }

                result.UpdateInfo();
            }
            else
            {
                result.AddRange(FormatScalar());
            }
            return result;
        }

        /// <summary>
        /// Execute padding on actual item (this.items[index]) and
        /// create a character array.
        /// </summary>
        /// <returns></returns>
        private AType[] FormatScalar()
        {
            int blankFront, blankEnd, counter = 0;

            switch (this.type)
            {
                case ATypes.ASymbol:
                    FormatSymbol(out blankFront, out blankEnd);
                    break;
                case ATypes.AInteger:
                    FormatInteger(out blankFront, out blankEnd);
                    break;
                default:
                    FormatFloat(out blankFront, out blankEnd);
                    break;

            }

            AType[] result = new AType[blankFront + this.items[index].Length + blankEnd];

            for (int i = 0; i < result.Length; i++)
            {
                if (i >= blankFront && i < blankFront + this.items[index].Length)
                {
                    result[i] = AChar.Create(this.items[index][counter++]);
                }
                else
                {
                    result[i] = AChar.Create(' ');
                }
            }

            this.index++;

            return result;
        }

        /// <summary>
        /// Determine right and left padding of symbol.
        /// </summary>
        /// <param name="blankFront"></param>
        /// <param name="blankEnd"></param>
        private void FormatSymbol(out int blankFront, out int blankEnd)
        {
            blankFront = 1;
            blankEnd = this.intervalMax == -1 ? 0 : (this.intervalMax - this.items[index].Length);
        }

        /// <summary>
        /// Determine right and left padding of integer.
        /// </summary>
        /// <param name="blankFront"></param>
        /// <param name="blankEnd"></param>
        private void FormatInteger(out int blankFront, out int blankEnd)
        {
            blankFront = this.intervalMax == -1 ? 1 : 1 + (this.intervalMax - this.items[index].Length);
            blankEnd = 0;
        }

        /// <summary>
        /// Determine right and left padding of float.
        /// </summary>
        /// <param name="blankFront"></param>
        /// <param name="blankEnd"></param>
        private void FormatFloat(out int blankFront, out int blankEnd)
        {
            int ind = this.items[index].IndexOf(".");

            if (ind != -1)
            {
                blankFront = this.intervalMax == -1 ? 1 : 1 + (this.intervalMax - ind);

                blankEnd = this.fractionMax == -1 ? 0 : (this.fractionMax - (this.items[index].Length - (ind + 1)));
            }
            else
            {
                blankFront = this.intervalMax == -1 ? 1 : 1 + (this.intervalMax - this.items[index].Length);

                blankEnd = this.fractionMax == -1 ? 0 : this.fractionMax;

                if (dot)
                {
                    blankEnd++;
                }
            }
        }

        #endregion
    }
}
