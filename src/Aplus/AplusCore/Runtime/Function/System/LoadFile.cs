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

            AType result = AArray.Create(ATypes.AChar);
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                int blockReadSize = 5 * 1024; // 5 KiB
                long totalSize = fileStream.Length;
                int readLength = blockReadSize < totalSize ? blockReadSize : (int)totalSize;
                byte[] filePartialContent = new byte[readLength];

                int sum = 0;    // total number of bytes read
                int count;  // current number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(filePartialContent, 0, readLength)) > 0)
                {
                    sum += count;  // sum is a buffer offset for next reading

                    // build the array from the bytes that was read
                    for (int i = 0; i < count; i++)
                    {
                        result.Add(AChar.Create((char)filePartialContent[i]));
                    }

                    // calculate the next size of the read block
                    long leftover = totalSize - sum;
                    readLength = blockReadSize < leftover ? blockReadSize : (int)leftover;
                }

                return result;
            }
        }
    }
}
