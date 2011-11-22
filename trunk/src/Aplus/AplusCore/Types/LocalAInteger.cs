namespace AplusCore.Types
{
    internal class LocalAInteger : AInteger
    {
        #region Variables

        private int value;

        #endregion

        #region Properties

        public override int asInteger
        {
            get { return this.value; }
        }

        #endregion

        #region Constructors

        private LocalAInteger(int number)
            : base()
        {
            this.value = number;
        }

        public new static AType Create(int number)
        {
            return new AReference(new LocalAInteger(number));
        }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new LocalAInteger(this.asInteger);
        }

        #endregion
    }
}
