using System.Collections.Generic;

using AplusCore.Runtime;
using AplusCore.Types.MemoryMapped;

namespace AplusCore.Types
{
    public abstract class AInteger : AValue
    {
        #region Constructor

        protected AInteger()
        {
            this.length = 1;
            this.shape = new List<int>();
            this.rank = 0;

            this.type = ATypes.AInteger;
        }

        public static AType Create(int number)
        {
            return LocalAInteger.Create(number);
        }

        public static AType Create(long position, MappedFile mappedFile)
        {
            return MMAInteger.Create(position, mappedFile);
        }

        #endregion

        #region Properties

        public override bool IsNumber { get { return true; } }
        public override bool IsTolerablyWholeNumber { get { return true; } }

        public override double asFloat { get { return (double)this.asInteger; } }
        public override string asString { get { return this.asInteger.ToString(); } }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return LocalAInteger.Create(this.asInteger).Data;
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
            return this.asInteger.GetHashCode();
        }

        public override string ToString()
        {
            return this.asInteger.ToString();
        }

        public override bool ConvertToRestrictedWholeNumber(out int result)
        {
            result = this.asInteger;
            return true;
        }

        public override int CompareTo(AType other)
        {
            return asFloat.CompareTo(other.asFloat);
        }

        #endregion
    }
}
