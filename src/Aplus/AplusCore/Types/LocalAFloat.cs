namespace AplusCore.Types
{
    internal class LocalAFloat : AFloat
    {
        #region Variables

        private double value;

        #endregion

        #region Properties

        public override double asFloat
        {
            get { return this.value; }
        }

        #endregion

        #region Constructors

        private LocalAFloat(double number)
            : base()
        {
            this.value = number;
        }

        public new static AType Create(double number)
        {
            return new AReference(new LocalAFloat(number));
        }

        #endregion

        #region Overrides

        public override AType Clone()
        {
            return new LocalAFloat(this.asFloat);
        }

        #endregion
    }
}
