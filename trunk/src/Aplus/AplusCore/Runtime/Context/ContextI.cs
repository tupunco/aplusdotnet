using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCore.Runtime.Context
{
    [AplusContext("i")]
    public static class ContextI
    {
        [AplusContextFunction("syncsend", "i.syncsend{scalar int;any;any} returns any")]
        public static AType SyncSend(Aplus environment, AType timeout, AType message, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.syncsend").asInteger;
            return AipcService.Instance.SyncSend(handleNumber, message, timeout);
        }

        [AplusContextFunction("syncread", "i.syncread{scalar int;any} returns any")]
        public static AType SyncRead(Aplus environment, AType timeout, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.syncread").asInteger;
            return AipcService.Instance.SyncRead(handleNumber, timeout);
        }

        [AplusContextFunction("send", "i.send{scalar int;any} returns scalar int")]
        public static AType Send(Aplus environment, AType message, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.send").asInteger;
            return AipcService.Instance.Send(handleNumber, message);
        }

        [AplusContextFunction("open", "i.open{scalar int} returns scalar int")]
        public static AType Open(Aplus environment, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.open").asInteger;
            return AipcService.Instance.Open(handleNumber);
        }

        [AplusContextFunction("destroy", "i.destroy{scalar int} returns scalar int")]
        public static AType Destroy(Aplus envrionment, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.destroy").asInteger;
            return AipcService.Instance.Destroy(handleNumber);
        }

        [AplusContextFunction("close", "i.close{scalar int} returns scalar int")]
        public static AType Close(Aplus environment, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.close").asInteger;
            return AipcService.Instance.Close(handleNumber);
        }

        [AplusContextFunction("listenNPP", "i.listenNPP{any;any;scalar int;any} returns scalar int")]
        public static AType ListenNPP(Aplus environment, AType protocol, AType port, AType name, AType function)
        {
            int portNumber = IsScalarAInteger(port, "i.listenNPP").asInteger;

            if (protocol.Type != ATypes.ASymbol || name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return AipcService.Instance.Listen(function, name, ConnectionAttribute.DEFAULT_HOST, portNumber, protocol);

        }

        [AplusContextFunction("listenN", "i.listenN{any;any} returns scalar int")]
        public static AType ListenN(Aplus environment, AType name, AType function)
        {
            if (name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return AipcService.Instance.Listen(function, name);
        }

        [AplusContextFunction("listenNP", "i.listenNP{any;any;any} returns scalar int")]
        public static AType ListenNP(Aplus environment, AType protocol, AType name, AType function)
        {
            if (protocol.Type != ATypes.ASymbol || name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return AipcService.Instance.Listen(function, name, protocol);
        }

        [AplusContextFunction("connectNHPP", "i.connectNHPP{any;any;any;scalar int;any} returns scalar int")]
        public static AType ConnectNHPP(
            Aplus environment, AType protocol, AType port, AType host, AType name, AType function)
        {
            int portNumber = IsScalarAInteger(port, "connectNHPP").asInteger;
            return AipcService.Instance.Connect(function, name, host, portNumber, protocol);
        }

        [AplusContextFunction("connectNP", "i.connectNP{any;any;any} returns scalar int")]
        public static AType ConnectNP(Aplus environment, AType protocol, AType name, AType function)
        {
            return AipcService.Instance.Connect(function, name, protocol);
        }

        [AplusContextFunction("connectN", "i.connectN{any;any} returns scalar int")]
        public static AType ConnectN(Aplus environment, AType name, AType function)
        {
            return AipcService.Instance.Connect(function, name);
        }

        [AplusContextFunction("timeout", "i.timeout{any} returns any")]
        public static AType Timeout(Aplus environment, AType argument)
        {
            return AipcService.Instance.GetTimeout(argument);
        }

        [AplusContextFunction("attrs", "i.attrs{scalar int} returns any")]
        public static AType Attributes(Aplus environment, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.attrs").asInteger;
            return AipcService.Instance.Attributes(handleNumber);
        }

        [AplusContextFunction("getattr", "i.getattr{scalar int;any} returns any")]
        public static AType GetAttribute(Aplus environment, AType attributeName, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.getattr").asInteger;
            return AipcService.Instance.GetAttribute(handleNumber, attributeName);
        }

        [AplusContextFunction("setattr", "i.setattr{scalar int;any;any} returns scalar int")]
        public static AType SetAttribute(Aplus environment, AType value, AType attributeName, AType handle)
        {
            IsScalarAInteger(handle, "i.setattr");

            return AipcService.Instance.SetAttribute(handle.asInteger, attributeName, value);
        }

        [AplusContextFunction("whatis", "i.whatis{scalar int} returns any")]
        public static AType WhatIs(Aplus environment, AType handle)
        {
            int handleNumber = IsScalarAInteger(handle, "i.whatis").asInteger;
            return AipcService.Instance.WhatIs(handleNumber);
        }

        [AplusContextFunction("roster", "i.roster{} returns any")]
        public static AType WhatIs(Aplus environment)
        {
            return AipcService.Instance.Roster();
        }

        #region Utility

        /// <summary>
        /// Checks if the argument is a valid scalar integer and returns it.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="function"></param>
        /// <exception cref="Error.Type">If the argument is not a tolerably whole number.</exception>
        /// <exception cref="Error.Length">If the argument is not a one element array or scalar</exception>
        private static AType IsScalarAInteger(AType argument, string function)
        {
            if (!argument.IsTolerablyWholeNumber)
            {
                throw new Error.Type(function);
            }

            AType result;

            if (!argument.TryFirstScalar(out result, true))
            {
                throw new Error.Length(function);
            }

            return result;
        }

        #endregion
    }
}
