using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        [SystemFunction("_gsv", "_gsv{any}: returns any")]
        internal static AType GetSystemVariable(Aplus environment, AType input)
        {
            ATypes type = input.Type;

            if (type != ATypes.ASymbol && type != ATypes.AChar)
            {
                throw new Error.Type("_gsv");
            }

            string name = (type == ATypes.ASymbol) ? input.asString : input.ToString();

            if(!environment.SystemVariables.Contains(name))
            {
                throw new Error.Domain("_gsv");
            }

            return environment.SystemVariables[name];
        }
    }
}
