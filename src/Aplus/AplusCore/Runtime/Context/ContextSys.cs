using System.Collections.Generic;
using System.IO;

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

        [AplusContextFunction("filesize", "sys.filesize{string} returns any")]
        public static AType FileSize(Aplus environment, AType argument)
        {
            if (argument.Type != ATypes.AChar)
            {
                throw new Error.Type("sys.filesize");
            }
            
            AType result;
            string fileName = argument.ToString();

            if (File.Exists(fileName))
            {
                result = Utils.CreateATypeResult((new FileInfo(fileName)).Length);
            }
            else
            {
                result = AInteger.Create(-1);
            }

            return result;
        }
    }
}
