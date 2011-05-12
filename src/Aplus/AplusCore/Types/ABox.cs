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

            /*if (!item.IsBox && item.Type == ATypes.AFunc)
            {
                this.type = ATypes.AFunc;
            }
            else
            {
                this.type = type;
            }*/

            this.type = item.Data is AFunc ? ATypes.AFunc : type;

            if (!String.IsNullOrEmpty(item.MemoryMappedFile))
            {
                this.MemoryMappedFile = item.MemoryMappedFile;
            }
        }

        public static AType Create(AType item, ATypes type = ATypes.ABox)
        {
            return new AReference(new ABox(item, type));
        }

        #endregion

        #region Properties

        //public override AValue Data
        //{
        //    get { return this; }
        //}

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
                //return this.value.Type == ATypes.AFunc && !this.value.IsBox;
                return this.value.Data is AFunc;
            }
        }

        #endregion

        #region Overrides

        public override AType Clone(bool isMemmoryMapped = false)
        {
            return new ABox(this.value.Clone(isMemmoryMapped));
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
            return this.value.GetHashCode() ^ 0x0B; // BOX reverseed :)
        }

        public override string ToString()
        {
            return String.Format("< {0}", this.value);
        }

        #endregion

    }
}
