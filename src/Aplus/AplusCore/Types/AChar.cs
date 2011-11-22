using System.Collections.Generic;

using AplusCore.Runtime;
using AplusCore.Types.MemoryMapped;

namespace AplusCore.Types
{
    public abstract class AChar : AValue
    {
        #region Constructor

        protected AChar()
        {
            this.length = 1;
            this.shape = new List<int>();
            this.rank = 0;

            this.type = ATypes.AChar;
        }

        public static AType Create(char ch)
        {
            return LocalAChar.Create(ch);
        }

        public static AType Create(long position, MappedFile mappedFile)
        {
            return MMAChar.Create(position, mappedFile);
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is AChar)
            {
                AChar other = (AChar)obj;
                return this.asChar == other.asChar;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.asChar.GetHashCode();
        }

        public override string ToString()
        {
            return this.asChar.ToString();
        }

        public override int CompareTo(AType other)
        {
            return this.asChar.CompareTo(other.asChar);
        }

        public override AType Clone()
        {
            return LocalAChar.Create(this.asChar).Data;
        }

        #endregion
    }
}
