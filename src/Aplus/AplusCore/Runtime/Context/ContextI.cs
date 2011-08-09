using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCore.Runtime.Context
{
    [AplusContext("i")]
    public static class ContextI
    {
        [AplusContextFunction("syncsend", "i.syncsend{scalar int;any;any} returns any")]
        public static AType SyncSend(AplusEnvironment environment, AType timeout, AType message, AType handle)
        {
            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("syncsend");
            }

            return AipcService.Instance.SyncSend(handle.asInteger, message, timeout);
        }

        [AplusContextFunction("syncread", "i.syncread{scalar int;any} returns any")]
        public static AType SyncRead(AplusEnvironment environment, AType timeout, AType handle)
        {
            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.syncread");
            }

            return AipcService.Instance.SyncRead(handle.asInteger, timeout);
        }

        [AplusContextFunction("send", "i.send{scalar int;any} returns scalar int")]
        public static AType Send(AplusEnvironment environment, AType message, AType handle)
        {
            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.send");
            }

            return AipcService.Instance.Send(handle.asInteger, message);
        }

        [AplusContextFunction("open", "i.open{scalar int} returns scalar int")]
        public static AType Open(AplusEnvironment environment, AType handle)
        {
            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.open");
            }

            return AInteger.Create(AipcService.Instance.Open(handle.asInteger));
        }

        [AplusContextFunction("destroy", "i.destroy{scalar int} returns scalar int")]
        public static AType Destroy(AplusEnvironment envrionment, AType handle)
        {
            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.destroy");
            }

            return AInteger.Create(AipcService.Instance.Destroy(handle.asInteger));
        }

        [AplusContextFunction("close", "i.close{scalar int} returns scalar int")]
        public static AType Close(AplusEnvironment environment, AType handle)
        {
            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.close");
            }

            return AInteger.Create(AipcService.Instance.Close(handle.asInteger));
        }

        [AplusContextFunction("listenNPP", "i.listenNPP{any;any;scalar int;any} returns scalar int")]
        public static AType ListenNPP(AplusEnvironment environment, AType protocol, AType port, AType name, AType function)
        {
            if (protocol.Type != ATypes.ASymbol || !port.IsTolerablyWholeNumber
                || name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return AInteger.Create(
                AipcService.Instance.Listen(function, name, ConnectionAttribute.DEFAULT_HOST, port.asInteger, protocol)
            );
        }

        [AplusContextFunction("listenN", "i.listenN{any;any} returns scalar int")]
        public static AType ListenN(AplusEnvironment environment, AType name, AType function)
        {
            if (name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return AInteger.Create(AipcService.Instance.Listen(function, name));
        }

        [AplusContextFunction("listenNP", "i.listenNP{any;any;any} returns scalar int")]
        public static AType ListenNP(AplusEnvironment environment, AType protocol, AType name, AType function)
        {
            if (protocol.Type != ATypes.ASymbol || name.Type != ATypes.ASymbol || function.Type != ATypes.AFunc)
            {
                return AInteger.Create(-1);
            }

            return AInteger.Create(AipcService.Instance.Listen(function, name, protocol));
        }

        [AplusContextFunction("connectNHPP", "i.connectNHPP{any;any;any;scalar int;any} returns scalar int")]
        public static AType ConnectNHPP(
            AplusEnvironment environment, AType protocol, AType port, AType host, AType name, AType function)
        {
            AType portNumber;

            if (!port.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.connectNHPP");
            }

            if (!port.TryFirstScalar(out portNumber, true))
            {
                throw new Error.Length("i.connectNHPP");
            }
            
            return AInteger.Create(AipcService.Instance.Connect(function, name, host, port.asInteger, protocol));
        }

        [AplusContextFunction("connectNP", "i.connectNP{any;any;any} returns scalar int")]
        public static AType ConnectNP(AplusEnvironment environment, AType protocol, AType name, AType function)
        {
            return AInteger.Create(AipcService.Instance.Connect(function, name, protocol));
        }

        [AplusContextFunction("connectN", "i.connectN{any;any} returns scalar int")]
        public static AType ConnectN(AplusEnvironment environment, AType name, AType function)
        {
            return AInteger.Create(AipcService.Instance.Connect(function, name));
        }

        [AplusContextFunction("timeout", "i.timeout{any} returns any")]
        public static AType Timeout(AplusEnvironment environment, AType argument)
        {
            return AipcService.Instance.GetTimeout(argument);
        }

        [AplusContextFunction("attrs", "i.attrs{scalar int} returns any")]
        public static AType Attributes(AplusEnvironment environment, AType handle)
        {
            AType handleNumber;

            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.attrs");
            }

            if (!handle.TryFirstScalar(out handleNumber, true))
            {
                throw new Error.Length("i.attrs");
            }

            return AipcService.Instance.Attributes(handleNumber.asInteger);
        }

        [AplusContextFunction("getattr", "i.getattr{scalar int;any} returns any")]
        public static AType GetAttribute(AplusEnvironment environment, AType attributeName, AType handle)
        {
            AType handleNumber;

            if (!handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.getattr");
            }

            if (!handle.TryFirstScalar(out handleNumber, true))
            {
                throw new Error.Length("i.getattr");
            }

            return AipcService.Instance.GetAttribute(handleNumber.asInteger, attributeName);
        }

        [AplusContextFunction("setattr", "i.setattr{scalar int;any;any} returns scalar int")]
        public static AType SetAttribute(AplusEnvironment environment, AType value, AType attributeName, AType handle)
        {
            AType handleNumber;

            if (handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.setattr");
            }

            if (!handle.TryFirstScalar(out handleNumber, true))
            {
                throw new Error.Length("i.setattr");
            }

            return AipcService.Instance.SetAttribute(handleNumber.asInteger, attributeName, value);
        }

        [AplusContextFunction("whatis", "i.whatis{scalar int} returns any")]
        public static AType WhatIs(AplusEnvironment environment, AType handle)
        {
            AType handleNumber;

            if (handle.IsTolerablyWholeNumber)
            {
                throw new Error.Type("i.whatis");
            }

            if (!handle.TryFirstScalar(out handleNumber, true))
            {
                throw new Error.Length("i.whatis");
            }

            return AipcService.Instance.WhatIs(handleNumber.asInteger);
        }

        [AplusContextFunction("roster", "i.roster{} returns any")]
        public static AType WhatIs(AplusEnvironment environment)
        {
            return AipcService.Instance.Roster();
        }
    }
}
