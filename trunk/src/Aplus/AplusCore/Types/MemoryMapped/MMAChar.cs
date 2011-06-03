using AplusCore.Runtime;
using System;

namespace AplusCore.Types.MemoryMapped
{
    class MMAChar : AChar, IMapped
    {
        #region Variables

        private long position;
        private MappedFile mappedFile;

        #endregion

        #region Properties

        public override char asChar
        {
            get { return this.mappedFile.ReadChar(this.position); }
        }

        public override bool IsMemoryMappedFile
        {
            get { return true; }
        }

        public MemoryMappedFileMode Mode
        {
            get { return this.mappedFile.Mode;  }
        }

        #endregion

        #region Constructor

        private MMAChar(long position, MappedFile mappedFile)
            : base(' ')
        {
            this.position = position;
            this.mappedFile = mappedFile;
        }

        public static AType Create(long position, MappedFile mappedFile)
        {
            return new AReference(new MMAChar(position, mappedFile));
        }

        #endregion

        #region Methods

        public void Update(AType value)
        {
            this.mappedFile.WriteChar(this.position, value.asChar);
        }

        #endregion
    }
}
