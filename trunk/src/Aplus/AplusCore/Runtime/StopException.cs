using System;

namespace AplusCore.Runtime
{
    /// <summary>
    /// Custom exception to represent the stop
    /// </summary>
    class StopException : Exception
    {
        #region Constructors

        public StopException()
            : base()
        {
        }

        public StopException(string message)
            : base(message)
        {
        }

        public StopException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("//[error] : {0}", this.Message);
        }

        #endregion
    }
}
