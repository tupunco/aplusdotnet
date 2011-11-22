using System;
using System.Collections.Generic;
using System.Text;

using AplusCore.Runtime;
using AplusCore.Types.MemoryMapped;

namespace AplusCore.Types
{
    public abstract class AArray : AValue
    {
        #region Properties

        public override bool IsArray { get { return true; } }
        public override bool IsNumber
        {
            get { return (this.Type == ATypes.AInteger) || (this.Type == ATypes.AFloat); }
        }

        #endregion

        #region Indexing

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

        public override AType this[List<AType> indexers]
        {
            get
            {
                if (indexers.Count == 1 && indexers[0].Length == 0 && this.IsMemoryMappedFile)
                {
                    return this;
                }

                return this.Indexing(indexers, 0, false, this.IsMemoryMappedFile);
            }
            set
            {
                AType target = this.Indexing(indexers, 0, true, this.IsMemoryMappedFile);
                Utils.PerformAssign(target, value);
            }
        }

        #endregion

        #region Constructor

        protected AArray(ATypes type)
        {
            this.length = 0;
            this.shape = new List<int>() { 0 };
            this.rank = 1;
            this.type = type;
        }

        public static AType Create(ATypes type, params AType[] items)
        {
            return LocalAArray.Create(type, items);
        }

        public static AType Create(MappedFile mappedFile)
        {
            return MMAArray.Create(mappedFile);
        }

        #endregion

        #region Methods

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
            return base.GetHashCode();
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

        public override AType Clone()
        {
            AType result = LocalAArray.Create(this.Type);

            for (int i = 0; i < this.Length; i++)
            {
                result.AddWithNoUpdate(this[i].Clone());
            }

            result.Length = this.Length;
            result.Shape.Clear();
            result.Shape.AddRange(this.Shape);
            result.Type = this.Type;
            result.Rank = this.Rank;

            return result.Data;
        }

        #endregion
    }
}
