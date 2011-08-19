using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AplusCore.Runtime.Function.ADAP
{
    internal enum ADAPExceptionType
    {
        Import,
        Export
    }

    internal class ADAPException : Exception
    {
        #region Variables

        private ADAPExceptionType type;

        #endregion

        #region Properties

        internal ADAPExceptionType Type { get { return this.type; } }

        #endregion

        #region Constructors

        internal ADAPException(ADAPExceptionType exceptionType)
            : base()
        {
            this.type = exceptionType;
        }

        internal ADAPException(ADAPExceptionType exceptionType, string message)
            : base(message)
        {
            this.type = exceptionType;
        }

        internal ADAPException(ADAPExceptionType exceptionType, string message, Exception innerEx)
            : base(message, innerEx)
        {
            this.type = exceptionType;
        }

        #endregion
    }
}
