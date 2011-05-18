using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Runtime;
using System.Collections;

namespace AplusCore.Types.MemoryMapped
{
    class MMAArray : AArray
    {
        #region Variables

        private MappedFile mappedFile;
        private Dictionary<int, AType> items;

        #endregion

        #region Construction

        private MMAArray(MappedFile mappedFile) : base(ATypes.AArray)
        {
            this.mappedFile = mappedFile;
            this.items = new Dictionary<int, AType>();
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

        public override string MemoryMappedFile
        {
            get
            {
                return this.mappedFile.Name;
            }
        }

        #endregion

        #region Indexing

        public override AType this[int index]
        {
            get
            {
                if (index >= 0 && this.Length > index)
                {
                    if (!this.items.ContainsKey(index))
                    {
                        this.items[index] = this.mappedFile.ReadCell(index);
                    }

                    return this.items[index];
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

        #endregion
    }
}
