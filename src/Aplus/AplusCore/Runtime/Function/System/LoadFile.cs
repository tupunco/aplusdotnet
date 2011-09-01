using System.IO;

using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        [SystemFunction("_loadfile", "_loadfile{filename}: returns with the content of a file")]
        internal static AType Loadfile(Aplus environment, AType filename)
        {
            string fileName = filename.ToString();

            if (filename.Type != ATypes.AChar)
            {
                throw new Error.Type("_loadfile");
            }

            if (!File.Exists(fileName))
            {
                throw new Error.Invalid("_loadfile");
            }

            using (StreamReader reader = new StreamReader(fileName))
            {
                string fileContent = reader.ReadToEnd();
                return Helpers.BuildString(fileContent);
            }
        }
    }
}
