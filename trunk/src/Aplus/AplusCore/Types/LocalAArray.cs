using System.Collections.Generic;

using AplusCore.Runtime;

namespace AplusCore.Types
{
    internal class LocalAArray : AArray
    {
        #region Variables

        private List<AType> items;

        #endregion

        #region Properties

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
                if (index < 0 || this.length <= index)
                {
                    throw new Error.Index("[]");
                }

                return this.items[index];
            }

            set
            {
                if (index < 0 || this.length <= index)
                {
                    throw new Error.Index("[]");
                }

                this.items[index] = value;
            }
        }

        #endregion

        #region Constructors

        private LocalAArray(ATypes type, params AType[] items)
            : base(type)
        {
            this.items = new List<AType>(items);

            this.UpdateInfo();
        }

        public new static AType Create(ATypes type, params AType[] items)
        {
            return new AReference(new LocalAArray(type, items));
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

        public override AType Clone()
        {
            AArray result = new LocalAArray(ATypes.AArray);

            foreach (AType item in items)
            {
                result.AddWithNoUpdate(item.Clone());
            }

            result.Length = this.Length;
            result.Shape.Clear();
            result.Shape.AddRange(this.Shape);
            result.Type = this.Type;
            result.Rank = this.Rank;

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.items.GetHashCode();
        }

        #endregion
    }
}
