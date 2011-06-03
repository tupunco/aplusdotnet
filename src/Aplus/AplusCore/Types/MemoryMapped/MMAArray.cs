using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AplusCore.Runtime;

namespace AplusCore.Types.MemoryMapped
{
    class MMAArray : AArray, IMapped
    {
        #region Variables

        private Dictionary<int, ValueType> indexCache;

        private MappedFile mappedFile;
        private ConditionalWeakTable<ValueType, AType> items;

        #endregion

        #region Construction

        private MMAArray(MappedFile mappedFile)
            : base(ATypes.AArray)
        {
            this.mappedFile = mappedFile;
            this.items = new ConditionalWeakTable<ValueType, AType>();
            this.indexCache = new Dictionary<int, ValueType>();
        }

        public static AType Create(MappedFile mappedFile)
        {
            return new AReference(new MMAArray(mappedFile));
        }

        #endregion

        #region Properties

        public override int Length
        {
            get { return this.mappedFile.Length; }
        }

        public override List<int> Shape
        {
            get { return this.mappedFile.Shape; }
        }

        public override int Rank
        {
            get { return this.mappedFile.Rank; }
        }

        public override ATypes Type
        {
            get { return this.mappedFile.Type; }
        }

        public override bool IsBox
        {
            get { return false; }
        }

        public override bool IsMemoryMappedFile
        {
            get { return true; }
        }

        public MemoryMappedFileMode Mode
        {
            get { return this.mappedFile.Mode; }
        }

        #endregion

        #region Indexing

        public override AType this[int index]
        {
            get
            {
                if (index >= 0 && this.Length > index)
                {
                    AType item;

                    ValueType indexValue = GetIndex(index);

                    if (!this.items.TryGetValue(indexValue, out item))
                    {
                        item = this.mappedFile.ReadCell(index);
                        this.items.Add(indexValue, item);
                    }

                    return item;
                }
                else
                {
                    throw new Error.Index("[]");
                }
            }
        }

        #endregion

        #region Enumerator

        public override IEnumerator<AType> GetEnumerator()
        {
            return new MMAArrayIterator(this);
        }

        #endregion

        #region Methods

        public override void Add(AType item)
        {
            this.mappedFile.Add(item);
        }

        public override void AddRange(IEnumerable<AType> items)
        {
            foreach (AType item in items)
            {
                Add(item);
            }
        }

        public override AType Clone()
        {
            AType result = AArray.Create(this.Type);

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

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.items.GetHashCode();
        }

        private ValueType GetIndex(int index)
        {
            if (!this.indexCache.ContainsKey(index))
            {
                this.indexCache[index] = index;
            }

            return this.indexCache[index];
        }

        public void Update(AType value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
