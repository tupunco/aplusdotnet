using System;
using System.Collections.Generic;
using System.Linq;

namespace AplusCore.Types
{
    public class ABox : AValue
    {
        #region Variables

        private AType value;

        #endregion

        #region Constructor

        private ABox(AType item, ATypes type = ATypes.ABox)
        {
            this.value = item;
            this.length = 1;
            this.shape = new List<int>();
            this.rank = 0;

            this.type = item.Data is AFunc ? ATypes.AFunc : type;
        }

        public static AType Create(AType item, ATypes type = ATypes.ABox)
        {
            return new AReference(new ABox(item, type));
        }

        #endregion

        #region Properties

        public override AType NestedItem
        {
            get { return this.value; }
        }

        public override bool IsBox
        {
            get { return true; }
        }

        public override bool IsFunctionScalar
        {
            get
            {
                return this.value.Data is AFunc;
            }
        }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new ABox(
                String.IsNullOrEmpty(this.value.MemoryMappedFile) ?
                this.value.Clone() :
                this.value
            );
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ABox))
            {
                return false;
            }
            ABox other = (ABox)obj;

            return this.value.Equals(other.value);

        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode() ^ 0x0B; // BOX reversed :)
        }

        public override string ToString()
        {
            return String.Format("< {0}", this.value);
        }

        #endregion
    }
}
