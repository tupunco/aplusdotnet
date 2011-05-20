using AplusCore.Runtime;

namespace AplusCore.Types.MemoryMapped
{
    class MMAFloat : AFloat, IMapped
    {
        #region Variables

        private long position;
        private MappedFile mappedFile;

        #endregion

        #region Properties

        public override double asFloat
        {
            get { return this.mappedFile.ReadDouble(this.position); }
        }

        public override bool IsMemoryMappedFile
        {
            get { return true; }
        }

        #endregion

        #region Constructor

        private MMAFloat(long position, MappedFile mappedFile)
            : base(0)
        {
            this.position = position;
            this.mappedFile = mappedFile;
        }

        public static AType Create(long position, MappedFile mappedFile)
        {
            return new AReference(new MMAFloat(position, mappedFile));
        }

        #endregion

        #region Methods

        public void Update(AType value)
        {
            this.mappedFile.WriteDouble(this.position, value.asFloat);
        }

        #endregion
    }
}
