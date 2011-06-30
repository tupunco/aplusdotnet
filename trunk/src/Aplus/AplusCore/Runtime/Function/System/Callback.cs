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
        internal static AType SetCallback(AplusEnvironment environment, AType callbackInfo, AType symbol)
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
                throw new Error.NonFunction("_scb");
            }

            AType staticData = MonadicFunctionInstance.Disclose.Execute(callbackInfo[1], environment);

            environment.Runtime.CallbackManager.Register(qualifiedName, callbackFunction.NestedItem, staticData);

            return Utils.ANull();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        [SystemFunction("_gcb", "_gcb{y}: returns the callback info for the 'x' global variable.")]
        internal static AType GetCallback(AplusEnvironment environment, AType symbol)
        {
            string qualifiedName;

            if (!TryQualifiedName(environment, symbol, out qualifiedName))
            {
                throw new Error.Domain("_scb");
            }

            AType result;
            CallbackItem callbackItem;

            if(environment.Runtime.CallbackManager.TryGetCallback(qualifiedName, out callbackItem))
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
