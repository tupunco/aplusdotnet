using System.Collections.Generic;

using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCore.Runtime.Context
{
    [AplusContext("sys")]
    public static class ContextSys
    {
        [AplusContextFunction("imp", "sys.imp{any} returns any")]
        public static AType Import(Aplus environment, AType argument)
        {
            if (argument.Type != ATypes.AChar)
            {
                throw new Error.Type("sys.imp");
            }

            if (argument.Rank > 1)
            {
                throw new Error.Rank("sys.imp");
            }

            List<byte> toConvert = new List<byte>();

            if (argument.Rank == 0)
            {
                throw new Error.Domain("sys.imp"); // One character can't be a valid message.
            }

            foreach (AType item in argument)
            {
                toConvert.Add((byte)item.asChar);
            }

            return SysImp.Instance.Import(toConvert.ToArray());
        }

        [AplusContextFunction("exp", "sys.exp{any} returns any")]
        public static AType Export(Aplus environment, AType argument)
        {
            byte[] exportedMessage = SysExp.Instance.Format(argument);
            AType result = AArray.Create(ATypes.AChar);

            foreach (byte charcter in exportedMessage)
            {
                result.AddWithNoUpdate(AChar.Create((char)charcter));
            }

            result.UpdateInfo();

            return result;
        }
    }
}
