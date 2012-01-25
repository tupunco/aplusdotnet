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

        private long offset;
        private int depth;

        #endregion

        #region Properties

        public override int Length
        {
            get { return this.mappedFile.Shape[depth]; }
        }

        public override List<int> Shape
        {
            get
            {
                List<int> actualShape = this.mappedFile.Shape;
                return actualShape.GetRange(depth, actualShape.Count - depth);
            }
        }

        public override int Rank
        {
            get { return this.mappedFile.Rank - depth; }
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
                if (index < 0 || this.Length <= index)
                //if (index >= 0 && this.Length > index)
                {
                    throw new Error.Index("[]");
                }

                AType item;

                ValueType indexValue = GetIndex(index);

                if (!this.items.TryGetValue(indexValue, out item))
                {
                    if (this.Rank > 1)
                    {
                        List<int> cuttedShape = this.Shape.GetRange(1, this.Shape.Count - 1);
                        long subDimensionOffset = index * cuttedShape.Product() * this.mappedFile.Size;
                        item = Create(this.mappedFile, this.depth + 1, this.offset + subDimensionOffset);
                    }
                    else
                    {
                        int elementSize = this.mappedFile.Size;
                        int elementOffset = index * elementSize;
                        item = this.mappedFile.ReadCell(this.offset + elementOffset);
                    }

                    this.items.Add(indexValue, item);
                }

                return item;
            }
        }

        public override AType this[List<AType> indexers]
        {
            get
            {
                return base[indexers];
            }
            set
            {
                if (indexers.Count == 1)
                {
                    Utils.PerformAssign(this[indexers[0]], value);
                }
                else
                {
                    base[indexers] = value;
                }
            }
        }

        #endregion

        #region Construction

        private MMAArray(MappedFile mappedFile, int depth, long position)
            : base(ATypes.AArray)
        {
            this.mappedFile = mappedFile;
            this.items = new ConditionalWeakTable<ValueType, AType>();
            this.indexCache = new Dictionary<int, ValueType>();
            this.depth = depth;
            this.offset = position;
        }

        private MMAArray(MappedFile mappedFile, int depth = 0)
            : this(mappedFile, depth, MappedFileInfo.HeaderSize)
        {
        }

        public new static AType Create(MappedFile mappedFile)
        {
            return new AReference(new MMAArray(mappedFile));
        }

        private static AType Create(MappedFile mappedFile, int depth, long position)
        {
            return new AReference(new MMAArray(mappedFile, depth, position));
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
