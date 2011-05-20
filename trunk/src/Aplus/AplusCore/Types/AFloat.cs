using System;
using System.Collections.Generic;
using System.Globalization;

namespace AplusCore.Types
{
    public class AFloat : AValue
    {
        #region Variables

        private double value;

        #endregion

        #region Constructor

        protected AFloat(double number)
        {
            this.value = number;

            this.length = 1;
            this.shape = new List<int>();
            this.rank = 0;

            this.type = ATypes.AFloat;
        }

        public static AType Create(double number)
        {
            return new AReference(new AFloat(number));
        }

        #endregion

        #region Properties

        public override bool IsNumber { get { return true; } }

        public override bool IsTolerablyWholeNumber
        {
            get
            {
                return Utils.ComparisonTolerance(this.asFloat, Math.Round(this.asFloat));
            }
        }

        #endregion

        #region Converter Properties

        public override int asInteger { get { return (int)this.asFloat; } }
        public override double asFloat { get { return this.value; } }
        public override string asString { get { return this.asFloat.ToString(); } }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new AFloat(this.asFloat);
        }

        public override bool Equals(object obj)
        {
            if (obj is AFloat)
            {
                AFloat other = (AFloat)obj;
                return this.asFloat == other.value;
            }
            else if (obj is AInteger)
            {
                AInteger other = (AInteger)obj;
                return this.IsTolerablyWholeNumber && (this.asInteger == other.asInteger);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.asFloat.GetHashCode();
        }

        public override string ToString()
        {
            return this.asFloat.ToString(CultureInfo.InvariantCulture);
        }

        public override bool ConvertToRestrictedWholeNumber(out int result)
        {
            double roundedValue = Math.Round(this.asFloat);
            if (Math.Abs(this.asFloat - roundedValue) < 1e-13)
            {
                return Utils.ConvertDoubleToInteger(roundedValue, out result);
            }
            result = -1;
            return false;
        }

        public override int CompareTo(AType other)
        {
            return this.asFloat.CompareTo(other.asFloat);
        }

        #endregion
    }
}
