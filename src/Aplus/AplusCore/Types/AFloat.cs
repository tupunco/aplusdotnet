using System;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;

namespace AplusCore.Types
{
    public class AFloat : AValue
    {
        #region Variables

        private double value;

        #endregion

        #region Constructor

        private AFloat(double number)
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
                return Utils.ComparisonTolerance(this.value, Math.Round(this.value));
            }
        }

        #endregion

        #region Converter Properties

        public override int asInteger { get { return (int)this.value; } }
        public override double asFloat { get { return this.value; } }
        public override string asString { get { return this.value.ToString(); } }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new AFloat(this.value);
        }

        public override bool Equals(object obj)
        {
            if (obj is AFloat)
            {
                AFloat other = (AFloat)obj;
                return this.value == other.value;
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
            return this.value.GetHashCode();
        }

        public override string ToString()
        {
            return this.value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool ConvertToRestrictedWholeNumber(out int result)
        {
            double roundedValue = Math.Round(this.value);
            if (Math.Abs(this.value - roundedValue) < 1e-13)
            {
                return Utils.ConvertDoubleToInteger(roundedValue, out result);
            }
            result = -1;
            return false;
        }

        public override int CompareTo(AType other)
        {
            return this.value.CompareTo(other.asFloat);
        }

        #endregion
    }
}
