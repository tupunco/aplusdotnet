using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Callback
{
    /// <summary>
    /// Represents a callback item.
    /// </summary>
    public class CallbackItem
    {
        #region Variables

        private string variableName;
        private AType callbackFunction;
        private AType staticData;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the global variable name related for the callback.
        /// </summary>
        public string VariableName
        {
            get { return variableName; }
        }

        /// <summary>
        /// Gets the callback function.
        /// </summary>
        public AType CallbackFunction
        {
            get { return callbackFunction; }
        }

        /// <summary>
        /// Gets the static data for the callback.
        /// </summary>
        public AType StaticData
        {
            get { return staticData; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a <see cref="CallbackItem"/> class.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="callbackFunction"></param>
        /// <param name="staticData"></param>
        public CallbackItem(string variableName, AType callbackFunction, AType staticData)
        {
            this.variableName = variableName;
            this.callbackFunction = callbackFunction;
            this.staticData = staticData;
        }

        #endregion
    }
}
