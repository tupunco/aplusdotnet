using System;

using AplusCore.Runtime.Callback;
using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="callbackInfo"></param>
        /// <param name="symbol"></param>
        /// <returns>Always returns ANull.</returns>
        [SystemFunction("_scb", "_scb{y;x}: set a callback for the given 'y' global variable.")]
        internal static AType SetCallback(Aplus environment, AType callbackInfo, AType symbol)
        {
            string qualifiedName;

            if(!TryQualifiedName(environment, symbol, out qualifiedName))
            {
                 throw new Error.Domain("_scb");
            }

            if (callbackInfo.Length != 2)
            {
                throw new Error.NonData("_scb");
            }
            else if (!callbackInfo.IsBox)
            {
                throw new Error.Domain("_scb");
            }

            AType callbackFunction = callbackInfo[0];
            if (!callbackFunction.IsFunctionScalar)
            {
                if (callbackFunction.NestedItem.Type != ATypes.ANull)
                {
                    throw new Error.NonFunction("_scb");
                }

                environment.CallbackManager.UnRegister(qualifiedName);
            }
            else
            {
                AType staticData = MonadicFunctionInstance.Disclose.Execute(callbackInfo[1], environment);

                environment.CallbackManager.Register(qualifiedName, callbackFunction.NestedItem, staticData);
            }

            return Utils.ANull();
        }

        [SystemFunction("_spcb", "_spcb{y;x}: set a preset callback for the given 'y' global variable.")]
        internal static AType SetPresetCallback(Aplus environment, AType callbackInfo, AType symbol)
        {
            string qualifiedName;

            if (!TryQualifiedName(environment, symbol, out qualifiedName))
            {
                throw new Error.Domain("_spcb");
            }

            if (callbackInfo.Length != 2)
            {
                throw new Error.NonData("_spcb");
            }
            else if (!callbackInfo.IsBox)
            {
                throw new Error.Domain("_spcb");
            }

            AType callbackFunction = callbackInfo[0];
            if (!callbackFunction.IsFunctionScalar)
            {
                if (callbackFunction.NestedItem.Type != ATypes.ANull)
                {
                    throw new Error.NonFunction("_spcb");
                }

                environment.CallbackManager.UnRegisterPreset(qualifiedName);
            }
            else
            {
                AType staticData = MonadicFunctionInstance.Disclose.Execute(callbackInfo[1], environment);

                environment.CallbackManager.RegisterPreset(qualifiedName, callbackFunction.NestedItem, staticData);
            }

            return Utils.ANull();
        }


        /// <summary>
        /// Returns with the name of the callback function, and the static data for the given variable.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="symbol">The name of the global variable</param>
        /// <returns></returns>
        [SystemFunction("_gcb", "_gcb{y}: returns the callback info for the 'x' global variable.")]
        internal static AType GetCallback(Aplus environment, AType symbol)
        {
            string qualifiedName;

            if (!TryQualifiedName(environment, symbol, out qualifiedName))
            {
                throw new Error.Domain("_gcb");
            }

            return GetCallback(environment, qualifiedName, false);
        }

        /// <summary>
        /// Returns with the name of the preset callback function, and the static data for the given variable.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="symbol">The name of the global variable</param>
        /// <returns></returns>
        [SystemFunction("_gpcb", "_gpcb{y}: returns the callback info for the 'x' global variable.")]
        internal static AType GetPresetCallback(Aplus environment, AType symbol)
        {
            string qualifiedName;

            if (!TryQualifiedName(environment, symbol, out qualifiedName))
            {
                throw new Error.Domain("_gpcb");
            }

            return GetCallback(environment, qualifiedName, true);
        }


        /// <summary>
        /// Returns with the name of the callback function, and the static data for the given variable.
        /// </summary>
        /// <param name="environment">The Aplus runtime environment.</param>
        /// <param name="qualifiedName">The name of the global variable.</param>
        /// <param name="isPresetCallback">Specifies if the preset callback is required or the simple callback.</param>
        /// <returns>Empyt array if not found, otherwise the callback information.</returns>
        private static AType GetCallback(Aplus environment, string qualifiedName, bool isPresetCallback)
        {
            AType result;
            CallbackItem callbackItem;

            bool found = isPresetCallback
                ? environment.CallbackManager.TryGetPresetCallback(qualifiedName, out callbackItem)
                : environment.CallbackManager.TryGetCallback(qualifiedName, out callbackItem);

            if (found)
            {
                string functionName = ((AFunc)callbackItem.CallbackFunction.Data).Name;
                result = Helpers.BuildStrand(
                    new AType[] { callbackItem.StaticData, Helpers.BuildString(functionName) }
                );
            }
            else
            {
                result = Utils.ANull();
            }

            return result;
        }
    }
}
