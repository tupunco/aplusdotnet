using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AplusCore.Types
{
    public class ASymbol : AValue
    {
        #region Variables

        private string symbolName;

        #endregion

        #region Constructor

        private ASymbol(string name)
        {
            this.symbolName = name;

            this.length = 1;
            this.shape = new List<int>();
            this.rank = 0;

            this.type = ATypes.ASymbol;
        }

        public static AType Create(string name)
        {
            return new AReference(new ASymbol(name));
        }

        #endregion

        #region Converter Properties

        public override string asString { get { return this.symbolName; } }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new ASymbol(symbolName);
        }

        public override bool Equals(object obj)
        {
            if (obj is ASymbol)
            {
                ASymbol other = (ASymbol)obj;
                return this.symbolName == other.symbolName;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.symbolName.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("`{0}", symbolName);
        }

        public override int CompareTo(AType other)
        {
            return this.symbolName.CompareTo(other.asString);
        }

        #endregion
    }
}
