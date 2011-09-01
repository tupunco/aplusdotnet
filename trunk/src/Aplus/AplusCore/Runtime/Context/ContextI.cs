using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCore.Runtime.Context
{
    [AplusContext("i")]
    public static class ContextI
    {
        [AplusContextInit]
        public static void InitContext(Aplus environment)
        {
            AipcService service = new AipcService(environment);
            service.StartNetworkLoop();

            environment.SetService<AipcService>(service);
        }

        [AplusContextFunction("syncsend", "i.syncsend{scalar int;any;any} returns any")]
        public static AType SyncSend(Aplus environment, AType timeout, AType message, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.syncsend");
            return environment.GetService<AipcService>().SyncSend(handleNumber, message, timeout);
        }

        [AplusContextFunction("syncread", "i.syncread{scalar int;any} returns any")]
        public static AType SyncRead(Aplus environment, AType timeout, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.syncread");
            return environment.GetService<AipcService>().SyncRead(handleNumber, timeout);
        }

        [AplusContextFunction("send", "i.send{scalar int;any} returns scalar int")]
        public static AType Send(Aplus environment, AType message, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.send");
            return environment.GetService<AipcService>().Send(handleNumber, message);
        }

        [AplusContextFunction("open", "i.open{scalar int} returns scalar int")]
        public static AType Open(Aplus environment, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.open");
            return environment.GetService<AipcService>().Open(handleNumber);
        }

        [AplusContextFunction("destroy", "i.destroy{scalar int} returns scalar int")]
        public static AType Destroy(Aplus environment, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.destroy");
            return environment.GetService<AipcService>().Destroy(handleNumber);
        }

        [AplusContextFunction("close", "i.close{scalar int} returns scalar int")]
        public static AType Close(Aplus environment, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.close");
            return environment.GetService<AipcService>().Close(handleNumber);
        }

        [AplusContextFunction("listenNPP", "i.listenNPP{any;any;scalar int;any} returns scalar int")]
        public static AType ListenNPP(Aplus environment, AType protocol, AType port, AType name, AType function)
        {
            int portNumber = ExtractInteger(port, "i.listenNPP");

            if (protocol.Type != ATypes.ASymbol || name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return environment.GetService<AipcService>().Listen(
                function, name, ConnectionAttribute.DEFAULT_HOST, portNumber, protocol);
        }

        [AplusContextFunction("listenN", "i.listenN{any;any} returns scalar int")]
        public static AType ListenN(Aplus environment, AType name, AType function)
        {
            if (name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return environment.GetService<AipcService>().Listen(function, name);
        }

        [AplusContextFunction("listenNP", "i.listenNP{any;any;any} returns scalar int")]
        public static AType ListenNP(Aplus environment, AType protocol, AType name, AType function)
        {
            if (protocol.Type != ATypes.ASymbol || name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return environment.GetService<AipcService>().Listen(function, name, protocol);
        }

        [AplusContextFunction("connectNHPP", "i.connectNHPP{any;any;any;scalar int;any} returns scalar int")]
        public static AType ConnectNHPP(
            Aplus environment, AType protocol, AType port, AType host, AType name, AType function)
        {
            int portNumber = ExtractInteger(port, "connectNHPP");
            return environment.GetService<AipcService>().Connect(function, name, host, portNumber, protocol);
        }

        [AplusContextFunction("connectNP", "i.connectNP{any;any;any} returns scalar int")]
        public static AType ConnectNP(Aplus environment, AType protocol, AType name, AType function)
        {
            return environment.GetService<AipcService>().Connect(function, name, protocol);
        }

        [AplusContextFunction("connectN", "i.connectN{any;any} returns scalar int")]
        public static AType ConnectN(Aplus environment, AType name, AType function)
        {
            return environment.GetService<AipcService>().Connect(function, name);
        }

        [AplusContextFunction("timeout", "i.timeout{any} returns any")]
        public static AType Timeout(Aplus environment, AType argument)
        {
            return environment.GetService<AipcService>().GetTimeout(argument);
        }

        [AplusContextFunction("attrs", "i.attrs{scalar int} returns any")]
        public static AType Attributes(Aplus environment, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.attrs");
            return environment.GetService<AipcService>().Attributes(handleNumber);
        }

        [AplusContextFunction("getattr", "i.getattr{scalar int;any} returns any")]
        public static AType GetAttribute(Aplus environment, AType attributeName, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.getattr");
            return environment.GetService<AipcService>().GetAttribute(handleNumber, attributeName);
        }

        [AplusContextFunction("setattr", "i.setattr{scalar int;any;any} returns scalar int")]
        public static AType SetAttribute(Aplus environment, AType value, AType attributeName, AType handle)
        {
            ExtractInteger(handle, "i.setattr");

            return environment.GetService<AipcService>().SetAttribute(handle.asInteger, attributeName, value);
        }

        [AplusContextFunction("whatis", "i.whatis{scalar int} returns any")]
        public static AType WhatIs(Aplus environment, AType handle)
        {
            int handleNumber = ExtractInteger(handle, "i.whatis");
            return environment.GetService<AipcService>().WhatIs(handleNumber);
        }

        [AplusContextFunction("roster", "i.roster{} returns any")]
        public static AType WhatIs(Aplus environment)
        {
            return environment.GetService<AipcService>().Roster();
        }

        #region Utility

        /// <summary>
        /// Returns the integer extracted from the argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="function"></param>
        /// <exception cref="Error.Type">If the argument is not a tolerably whole number.</exception>
        /// <exception cref="Error.Length">If the argument is not a one element array or scalar</exception>
        private static int ExtractInteger(AType argument, string function)
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

            return result.asInteger;
        }

        #endregion
    }
}
