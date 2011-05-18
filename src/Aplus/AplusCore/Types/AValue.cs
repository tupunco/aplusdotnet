using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Runtime;

namespace AplusCore.Types
{
    public abstract class AValue : AType
    {
        #region Variables

        /// <summary>
        /// Length of an axis is the number of elements lying along any one of the edge defining that axis
        /// alias: dimension
        /// </summary>
        protected int length;

        /// <summary>
        /// The vector composed of the length of all axes of an array (vector of dimensions),
        /// is called the shape of the array
        /// </summary>
        protected List<int> shape;

        /// <summary>
        /// The rank of an array is the number of its axes.
        /// 
        /// </summary>
        protected int rank;

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="ATypes"/>
        protected ATypes type;

        protected string memoryMappedFile;

        #endregion

        #region AType Properties

        public virtual bool IsArray { get { return false; } }
        public virtual bool IsPrimitive { get { return false; } }
        public virtual bool IsBox { get { return false; } }
        public virtual bool IsFunctionScalar { get { return false; } }

        public virtual string MemoryMappedFile
        {
            get { return this.memoryMappedFile; }
            set { this.memoryMappedFile = value; }
        }

        public virtual bool IsNumber { get { return false; } }
        public virtual bool IsTolerablyWholeNumber { get { return false; } }

        public virtual ATypes Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public virtual int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        public virtual List<int> Shape
        {
            get { return this.shape; }
            set { this.shape = value; }
        }

        public virtual int Rank
        {
            get { return this.rank; }
            set { this.rank = value; }
        }

        public virtual AType this[int index]
        {
            get { throw new Error.Rank("[]"); }
            set { throw new Error.Rank("[]"); }
        }

        public virtual AType this[AType index]
        {
            get { throw new Error.Rank("[]"); }
            set { throw new Error.Rank("[]"); }
        }

        public virtual AType this[List<AType> indexers]
        {
            get { throw new Error.Rank("[]"); }
            set { throw new Error.Rank("[]"); }
        }

        public virtual AValue Data
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }

        public virtual AType NestedItem
        {
            get { throw new InvalidOperationException(); }
        }

        #endregion

        #region Converter Properties

        public virtual int asInteger { get { throw new NotImplementedException("Invalid use-case"); } }
        public virtual double asFloat { get { throw new NotImplementedException("Invalid use-case"); } }
        public virtual string asString { get { throw new NotImplementedException("Invalid use-case"); } }
        public virtual char asChar { get { throw new NotImplementedException("Invalid use-case"); } }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ 0x12345;
        }

        #endregion

        #region Methods

        public virtual void Add(AType item)
        {
            throw new InvalidOperationException();
        }

        public virtual void AddWithNoUpdate(AType item)
        {
            throw new InvalidOperationException();
        }

        public virtual void AddRange(IEnumerable<AType> items)
        {
            throw new InvalidOperationException();
        }

        public virtual void AddRangeWithNoUpdate(IEnumerable<AType> items)
        {
            throw new InvalidOperationException();
        }

        public virtual void UpdateInfo()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the first scalar from the AType.
        /// </summary>
        /// <remarks>
        /// For most ATypes this will return the same AType. But for
        /// AArray it should return the scalar from the most inner element.
        /// </remarks>
        /// <param name="result">The resulting AType</param>
        /// <param name="strictLengthCheck">If true check if the length is 1</param>
        /// <returns>True if we found such scalar</returns>
        public virtual bool TryFirstScalar(out AType result, bool strictLengthCheck = false)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Try conversion of item to integer
        /// </summary>
        /// <param name="result"></param>
        /// <returns>True if the item can be converted to integer otherwise False</returns>
        public virtual bool ConvertToRestrictedWholeNumber(out int result)
        {
            result = -1;
            return false;
        }

        /// <summary>
        /// Clones the AType, returning a new instance containing the same values
        /// </summary>
        /// <returns>Cloned AType</returns>
        public virtual AType Clone()
        {
            throw new NotImplementedException("Invalid use-case");
        }

        /// <summary>
        /// Compare 2 Atype.
        /// Result: -1 if the left AType is less than right.
        ///         0 if the 2 AType equal.
        ///         1 if the right AType is bigger than left.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(AType other)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        public virtual bool ComparisonToleranceCompareTo(AType other)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        #endregion

        #region Operator Overloads

        public static bool operator ==(AValue left, AValue right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(AValue left, AValue right)
        {

            return !(left == right);
        }

        #endregion

        #region Enumerator

        public virtual IEnumerator<AType> GetEnumerator()
        {
            throw new InvalidOperationException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
