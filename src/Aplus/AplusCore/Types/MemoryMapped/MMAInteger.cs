using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;
using AplusCore.Runtime;

namespace AplusCore.Types.MemoryMapped
{
    public class MMAInteger : AInteger
    {
        #region Variables

        private long position;
        private MappedFile mappedFile;

        #endregion

        #region Constructor

        private MMAInteger(long position, MappedFile mappedFile, string memoryMappedFileName)
            : base(0)
        {
            this.position = position;
            this.mappedFile = mappedFile;
            this.memoryMappedFile = memoryMappedFileName;
        }

        public static AType Create(long position, MappedFile mappedFile, string memoryMappedFileName)
        {
            return new AReference(new MMAInteger(position, mappedFile, memoryMappedFileName));
        }

        #endregion

        #region Methods

        public void SetValue(int value)
        {
            this.mappedFile.WriteInt32(this.position, value);
        }

        #endregion

        #region Converter Properties

        public override int asInteger
        {
            get { return this.mappedFile.ReadInt32(this.position); }
        }

        public override double asFloat
        {
            get { return this.asInteger; }
        }

        public override string asString
        {
            get { return this.asInteger.ToString(); }
        }

        public override string MemoryMappedFile
        {
            get { return this.mappedFile.Name; }
        }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return AInteger.Create(this.asInteger).Data;
        }

        public override bool Equals(object obj)
        {
            if (obj is AInteger)
            {
                AInteger other = (AInteger)obj;
                return this.asInteger == other.asInteger;
            }
            else if (obj is AFloat)
            {
                AFloat other = (AFloat)obj;
                return other.IsTolerablyWholeNumber && (this.asInteger == other.asInteger);
            }

            //obj is MMAInteger

            return false;
        }

        public override int GetHashCode()
        {
            return this.asInteger.GetHashCode();
        }

        public override string ToString()
        {
            return this.asInteger.ToString();
        }

        public override bool ConvertToRestrictedWholeNumber(out int result)
        {
            result = this.asInteger;
            return true;
        }

        public override int CompareTo(AType other)
        {
            return this.asFloat.CompareTo(other.asFloat);
        }

        public override bool ComparisonToleranceCompareTo(AType other)
        {
            if (other.Type != ATypes.AFloat && other.Type != ATypes.AInteger)
            {
                return false;
            }

            return Utils.ComparisonTolerance(this.asFloat, other.asFloat);
        }

        #endregion
    }
}
