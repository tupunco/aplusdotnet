using System;
using System.Collections.Generic;
using System.Text;
using AplusCore.Runtime;
using System.Collections;
using System.Linq;

namespace AplusCore.Types
{
    public class AArray : AValue, IEnumerable<AType>
    {
        #region Variables

        private List<AType> items;

        #endregion

        #region Properties

        public override bool IsArray { get { return true; } }
        public override bool IsNumber
        {
            get { return (this.type == ATypes.AInteger) || (this.type == ATypes.AFloat); }
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

        public override List<AType> Container { get { return items; } }

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
                if (idx < 0 || this.length < idx)
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

        #region Methods

        // TODO: temporarly moved to Utils..

        #endregion

        #region IEnumerable<AType> methods

        public override IEnumerator<AType> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Creates an AArray with type AInteger from the input list of integers
        /// </summary>
        /// <param name="list">List of Integers</param>
        /// <returns></returns>
        public static AType FromIntegerList(IEnumerable<int> list)
        {
            AArray array = new AArray(ATypes.AInteger);

            foreach (int item in list)
            {
                array.AddWithNoUpdate(AInteger.Create(item));
            }
            array.UpdateInfo();

            return new AReference(array);
        }

        public static AType ANull(ATypes type = ATypes.ANull)
        {
            return new AReference(new AArray(type));
        }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            AArray result = new AArray(ATypes.AArray);

            foreach (AType item in items)
            {
                result.AddWithNoUpdate(item.Clone());
            }
            result.UpdateInfo(this);
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
            if (this.items.Count != other.items.Count || this.type != other.type)
            {
                return false;
            }

            // Check items one-by-one
            for (int i = 0; i < this.items.Count; i++)
            {
                if (!this.items[i].Equals(other.items[i]))
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
            if (this.items.Count > 0 && this.items[0].IsArray)
            {
                StringBuilder str = new StringBuilder();
                foreach (AType item in this.items)
                {
                    str.AppendFormat("{0}\n", item);
                }
                return str.ToString();
            }
            else
            {
                string separator;
                switch (this.type)
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
                return String.Join(separator, this.items.ToStringArray<AType>());
            }
        }

        public override int CompareTo(AType other)
        {
            if (this.length != other.Length)
            {
                throw new Error.Length("[]");
            }
            else
            {
                int result;
                for (int i = 0; i < this.length; i++)
                {
                    result = this.items[i].CompareTo(other[i]);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                return 0;
            }
        }

        public override bool ComparisonToleranceCompareTo(AType other)
        {
            for (int i = 0; i < this.length; i++)
            {
                if (!this.items[i].ComparisonToleranceCompareTo(other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
