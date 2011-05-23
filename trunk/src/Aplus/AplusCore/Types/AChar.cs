using System.Collections.Generic;

namespace AplusCore.Types
{
    public class AChar : AValue
    {
        #region Variables

        private char value;

        #endregion

        #region Constructor

        protected AChar(char text)
        {
            this.value = text;

            this.length = 1;
            this.shape = new List<int>();
            this.rank = 0;

            this.type = ATypes.AChar;
        }

        public static AType Create(char ch)
        {
            return new AReference(new AChar(ch));
        }

        #endregion

        #region Converter Properties

        public override char asChar { get { return this.value; } }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new AChar(this.asChar);
        }

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

        #endregion
    }
}
