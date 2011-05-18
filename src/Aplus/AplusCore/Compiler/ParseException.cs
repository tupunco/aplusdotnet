using System;

namespace AplusCore.Compiler
{
    public class ParseException : Exception
    {
        #region Variables

        private bool cancontinue;

        #endregion

        #region Properties

        public bool CanContinue { get { return this.cancontinue; } }

        #endregion

        #region Constructors

        public ParseException(string message)
            : base(message)
        {
            this.cancontinue = false;
        }

        public ParseException(string message, bool canContinue)
            : base(message)
        {
            this.cancontinue = canContinue;
        }

        public ParseException(string message, bool canContinue, Exception innerException)
            : base(message, innerException)
        {
            this.cancontinue = canContinue;
        }

        #endregion
    }
}
