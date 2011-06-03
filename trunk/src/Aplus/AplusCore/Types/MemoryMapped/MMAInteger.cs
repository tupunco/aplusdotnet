using AplusCore.Runtime;

namespace AplusCore.Types.MemoryMapped
{
    public class MMAInteger : AInteger, IMapped
    {
        #region Variables

        private long position;
        private MappedFile mappedFile;

        #endregion

        #region Properties

        public override int asInteger
        {
            get { return this.mappedFile.ReadInt32(this.position); }
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

        #region Constructor

        private MMAInteger(long position, MappedFile mappedFile)
            : base(0)
        {
            this.position = position;
            this.mappedFile = mappedFile;
        }

        public static AType Create(long position, MappedFile mappedFile)
        {
            return new AReference(new MMAInteger(position, mappedFile));
        }

        #endregion

        #region Methods

        public void Update(AType value)
        {
            this.mappedFile.WriteInt32(this.position, value.asInteger);
        }

        #endregion
    }
}
