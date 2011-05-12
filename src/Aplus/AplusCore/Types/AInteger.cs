﻿using System;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;

namespace AplusCore.Types
{
    public class AInteger : AValue
    {
        #region Variables

        private int value;

        #endregion

        #region Constructor

        protected AInteger(int number)
        {
            this.value = number;
            this.length = 1;
            this.shape = new List<int>();
            this.rank = 0;

            this.type = ATypes.AInteger;
        }

        public static AType Create(int number)
        {
            return new AReference(new AInteger(number));
        }

        #endregion

        #region Properties

        public override bool IsNumber { get { return true; } }
        public override bool IsTolerablyWholeNumber { get { return true; } }

        #endregion

        #region Converter Properties

        public override int asInteger { get { return this.value; } }
        public override double asFloat { get { return (double)this.value; } }
        public override string asString { get { return this.value.ToString(); } }

        #endregion

        #region Overrides

        public override AType Clone(bool isMemmoryMapped = false)
        {
            return new AInteger(this.value);
        }

        public override bool Equals(object obj)
        {
            if (obj is AInteger)
            {
                AInteger other = (AInteger)obj;
                return this.asInteger == other.asInteger;
            }
            else if (obj is AFloat)
            {
                AFloat other = (AFloat)obj;
                return other.IsTolerablyWholeNumber && (this.asInteger == other.asInteger);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override string ToString()
        {
            return this.value.ToString();
        }

        public override bool ConvertToRestrictedWholeNumber(out int result)
        {
            result = value;
            return true;
        }

        public override int CompareTo(AType other)
        {
            return asFloat.CompareTo(other.asFloat);
        }

        #endregion
    }
}
