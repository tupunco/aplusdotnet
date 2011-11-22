namespace AplusCore.Types
{
    internal class LocalAChar : AChar
    {
        #region Variables

        private char value;

        #endregion

        #region Properties

        public override char asChar
        {
            get { return this.value; }
        }

        #endregion

        #region Constructors

        private LocalAChar(char text)
            : base()
        {
            this.value = text;
        }

        public new static AType Create(char ch)
        {
            return new AReference(new LocalAChar(ch));
        }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new LocalAChar(this.asChar);
        }

        #endregion
    }
}
