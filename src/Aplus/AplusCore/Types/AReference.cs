using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AplusCore.Types
{
    public class AReference : AType
    {
        #region Variables

        private AValue data;

        #endregion

        #region Properties

        public AValue Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        #endregion

        #region Constructor

        public AReference(AValue data)
        {
            this.Data = data;
        }

        #endregion

        #region AType Properties

        public bool IsArray
        {
            get { return this.Data.IsArray; }
        }

        public bool IsPrimitive
        {
            get { return this.Data.IsPrimitive; }
        }

        public bool IsBox
        {
            get { return this.Data.IsBox; }
        }

        public bool IsFunctionScalar
        {
            get { return this.Data.IsFunctionScalar; }
        }

        public bool IsNumber
        {
            get { return this.Data.IsNumber; }
        }

        public bool IsTolerablyWholeNumber
        {
            get { return this.Data.IsTolerablyWholeNumber; }
        }

        public ATypes Type
        {
            get
            {
                return this.Data.Type;
            }
            set
            {
                this.Data.Type = value;
            }
        }

        public int Length
        {
            get
            {
                return this.Data.Length;
            }
            set
            {
                this.Data.Length = value;
            }
        }

        public List<int> Shape
        {
            get
            {
                return this.Data.Shape;
            }
            set
            {
                this.Data.Shape = value;
            }
        }

        public int Rank
        {
            get
            {
                return this.Data.Rank;
            }
            set
            {
                this.Data.Rank = value;
            }
        }

        public string Infos
        {
            get { return this.Data.Infos; }
        }

        public string ShapeString
        {
            get { return this.Data.ShapeString; }
        }

        public List<AType> Container
        {
            get { return this.Data.Container; }
        }

        public AType this[int index]
        {
            get { return this.Data[index]; }
            set { this.Data[index] = value; }
        }

        public AType this[AType index]
        {
            get { return this.Data[index]; }
            set { this.Data[index] = value; }
        }

        public AType this[List<AType> indexers]
        {
            get { return this.Data[indexers]; }
            set { this.Data[indexers] = value; }
        }

        public int asInteger
        {
            get { return this.Data.asInteger; }
        }

        public double asFloat
        {
            get { return this.Data.asFloat; }
        }

        public string asString
        {
            get { return this.Data.asString; }
        }

        public char asChar
        {
            get { return this.Data.asChar; }
        }

        public AType NestedItem
        {
            get { return this.Data.NestedItem; }
        }

        #endregion

        #region AType Methods

        public bool TryFirstScalar(out AType result, bool strictLengthCheck = false)
        {
            if (this.Data.IsArray)
            {
                if (this.Data.Length == 0 || (strictLengthCheck && this.Data.Length != 1))
                {
                    result = this;
                    return false;
                }

                return this.Data.Container[0].TryFirstScalar(out result, strictLengthCheck);
            }
            else
            {
                result = this;
                return true;
            }
        }

        public bool ConvertToRestrictedWholeNumber(out int result)
        {
            return this.Data.ConvertToRestrictedWholeNumber(out result);
        }

        public AType Clone()
        {
            // The clone of the 'Data' is an AValue for sure.
            return new AReference((AValue)this.Data.Clone());
        }

        public InfoResult CompareInfos(AType other)
        {
            if (other is AReference)
            {
                return this.Data.CompareInfos(((AReference)other).Data);
            }

            return InfoResult.TypeError;
        }

        public bool ComparisonToleranceCompareTo(AType other)
        {
            if (other is AReference)
            {
                return this.Data.ComparisonToleranceCompareTo(((AReference)other).Data);
            }

            return false;
        }

        #endregion

        #region IComparable<AType> Members

        public int CompareTo(AType other)
        {
            if (other is AReference)
            {
                return this.Data.CompareTo(((AReference)other).Data);
            }

            return -1;
        }

        #endregion

        #region overrides

        public override bool Equals(object obj)
        {
            if (obj is AReference)
            {
                return this.Data.Equals(((AReference)obj).Data);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Data.GetHashCode() ^ 0x1337;
        }

        public override string ToString()
        {
            return this.Data.ToString();
        }

        #endregion
        
        #region IEnumerable<AType> Members

        public IEnumerator<AType> GetEnumerator()
        {
            return this.Data.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Data.GetEnumerator();
        }

        #endregion
    }
}
