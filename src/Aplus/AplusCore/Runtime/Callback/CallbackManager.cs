using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Callback
{
    public class CallbackManager
    {
        #region Variables

        /// <summary>
        /// Maps the global variable to an AFunc.
        /// </summary>
        private Dictionary<string, CallbackItem> mapping;
        private Dictionary<string, CallbackItem> presetMapping;

        #endregion

        #region Constructors

        public CallbackManager()
        {
            this.mapping = new Dictionary<string, CallbackItem>();
            this.presetMapping = new Dictionary<string, CallbackItem>();
        }

        #endregion

        #region Register related

        /// <summary>
        /// Check if there is a callback for the given global variable name.
        /// </summary>
        /// <param name="variableName">Global variable name.</param>
        /// <returns>True if there is a callback for the global variable, false otherwise.</returns>
        public bool Contains(string variableName)
        {
            return this.mapping.ContainsKey(variableName);
        }

        /// <summary>
        /// Gets the <see cref="CallbackItem"/> for the given global variable name.
        /// </summary>
        /// <param name="variableName">Global variable name.</param>
        /// <param name="callback"></param>
        /// <returns>True if the is a callback for the given global variable, otherwise false.</returns>
        public bool TryGetCallback(string variableName, out CallbackItem callback)
        {
            return this.mapping.TryGetValue(variableName, out callback);
        }

        /// <summary>
        /// Gets the <see cref="CallbackItem"/> for the given global variable name.
        /// </summary>
        /// <param name="variableName">Global variable name.</param>
        /// <param name="callback"></param>
        /// <returns>True if the is a preset callback for the given global variable, otherwise false.</returns>
        public bool TryGetPresetCallback(string variableName, out CallbackItem callback)
        {
            return this.presetMapping.TryGetValue(variableName, out callback);
        }

        /// <summary>
        /// Register a callback to a global variable.
        /// </summary>
        /// <param name="variableName">Global variable name.</param>
        /// <param name="callbackFunction"></param>
        public void Register(string variableName, AType callbackFunction, AType staticData)
        {
            this.mapping[variableName] = new CallbackItem(variableName, callbackFunction, staticData);
        }

        /// <summary>
        /// Register a preset callback to a global variable.
        /// </summary>
        /// <param name="variableName">Global variable name.</param>
        /// <param name="callbackFunction"></param>
        public void RegisterPreset(string variableName, AType callbackFunction, AType staticData)
        {
            this.presetMapping[variableName] = new CallbackItem(variableName, callbackFunction, staticData);
        }

        /// <summary>
        /// Removes a callback for the given global variable.
        /// </summary>
        /// <param name="variableName">Global variable name.</param>
        /// <returns>True if there was a global variable with a callback, false otherwise.</returns>
        public bool UnRegister(string variableName)
        {
            return this.mapping.Remove(variableName);
        }

        /// <summary>
        /// Removes a preset callback for the given global variable.
        /// </summary>
        /// <param name="variableName">Global variable name.</param>
        /// <returns>True if there was a global variable with a preset callback, false otherwise.</returns>
        public bool UnRegisterPreset(string variableName)
        {
            return this.presetMapping.Remove(variableName);
        }

        #endregion
    }
}
