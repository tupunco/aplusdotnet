using System;
using System.Collections.Generic;
using System.Text;
using AplusCore.Runtime;
using System.Collections;
using System.Linq;

namespace AplusCore.Types
{
    public class AArray : AValue
    {
        #region Variables

        private List<AType> items;

        #endregion

        #region Properties

        public override bool IsArray { get { return true; } }
        public override bool IsNumber
        {
            get { return (this.Type == ATypes.AInteger) || (this.Type == ATypes.AFloat); }
        }

        public override bool IsBox
        {
            get
            {
                foreach (AType item in this.items)
                {
                    if (item.IsBox)
                    {
                        return true;
                    }
                }
                return false;
                //return this.type == ATypes.ABox || (this.length > 0 && this.items[0].IsBox);
            }
        }

        #endregion

        #region Indexing

        public override AType this[int index]
        {
            get
            {
                if (index >= 0 && this.length > index)
                {
                    return this.items[index];
                }
                else
                {
                    throw new Error.Index("[]");
                }
            }

            set
            {
                if (index >= 0 && this.length > index)
                {
                    this.items[index] = value;
                }
                else
                {
                    throw new Error.Index("[]");
                }
            }
        }

        public override AType this[List<AType> indexers]
        {
            get
            {
                return this.Indexing(indexers, 0);
            }
            set
            {
                AType target = this.Indexing(indexers, 0);
                Utils.PerformAssign(target, value);
            }
        }

        public override AType this[AType index]
        {
            get
            {
                int idx;
                // only integers allowed for indexing
                if (!index.IsNumber || !Utils.TryComprasionTolarence(index.asFloat, out idx))
                {
                    throw new Error.Type("[]");
                }

                // No negative indexing or over-indexing
                if (idx < 0 || this.Length < idx)
                {
                    throw new Error.Index("[]");
                }

                return this[idx];
            }
        }

        #endregion

        #region Constructor

        protected AArray(ATypes type)
        {
            this.items = new List<AType>();

            this.length = 0;
            this.shape = new List<int>() { 0 };
            this.rank = 1;
            this.type = type;
        }

        private AArray(ATypes type, params AType[] items)
            : this(type)
        {
            this.items.AddRange(items);

            this.UpdateInfo();
        }

        public static AType Create(ATypes type)
        {
            return new AReference(new AArray(type));
        }

        public static AType Create(ATypes type, params AType[] items)
        {
            return new AReference(new AArray(type, items));
        }

        #endregion

        #region Enumerator

        public override IEnumerator<AType> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion

        #region Methods

        public override void Add(AType item)
        {
            this.items.Add(item);
            UpdateInfo();
        }

        public override void AddWithNoUpdate(AType item)
        {
            this.items.Add(item);
        }

        public override void AddRange(IEnumerable<AType> items)
        {
            this.items.AddRange(items);
            UpdateInfo();
        }

        public override void AddRangeWithNoUpdate(IEnumerable<AType> items)
        {
            this.items.AddRange(items);
        }

        public override void UpdateInfo()
        {
            this.Length = this.items.Count;

            this.Shape.Clear();
            this.Shape.Add(this.Length);

            if (this.Length > 0)
            {
                // Update Type info from children
                this.Type = this.items[0].Type;

                // Update Shape info
                if (this.items[0].IsArray)
                {
                    this.Shape.AddRange(this.items[0].Shape);
                }
            }

            this.Rank = this.Shape.Count;
        }

        public override AType Clone(bool isMemmoryMapped = false)
        {
            AArray result = new AArray(ATypes.AArray);

            foreach (AType item in items)
            {
                result.AddWithNoUpdate(item.Clone(isMemmoryMapped));
            }

            result.Length = this.Length;
            result.Shape.Clear();
            result.Shape.AddRange(this.Shape);
            result.Type = this.Type;
            result.Rank = this.Rank;

            return result;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AArray))
            {
                return false;
            }
            AArray other = (AArray)obj;

            // Check if the inner elem count matches and the type is the same
            if (this.Length != other.Length || this.Type != other.Type)
            {
                return false;
            }

            // Check items one-by-one
            for (int i = 0; i < this.Length; i++)
            {
                if (!this[i].Equals(other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.items.GetHashCode();
        }

        public override string ToString()
        {
            if (this.Length > 0 && this[0].IsArray)
            {
                StringBuilder str = new StringBuilder();
                foreach (AType item in this)
                {
                    str.AppendFormat("{0}\n", item);
                }
                return str.ToString();
            }
            else
            {
                string separator;
                switch (this.Type)
                {
                    case ATypes.AChar:
                        separator = "";
                        break;
                    case ATypes.ABox:
                        separator = "\n";
                        break;
                    default:
                        separator = " ";
                        break;
                }

                return String.Join(separator, this.ToStringArray<AType>());
            }
        }

        public override int CompareTo(AType other)
        {
            if (this.Length != other.Length)
            {
                throw new Error.Length("[]");
            }
            else
            {
                int result;
                for (int i = 0; i < this.Length; i++)
                {
                    result = this[i].CompareTo(other[i]);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                return 0;
            }
        }

        #endregion
    }
}
