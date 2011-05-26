using System;
using System.IO;
using System.IO.MemoryMappedFiles;

using AplusCore.Types;

namespace AplusCore.Runtime
{
    public class MemoryMappedFileManager
    {
        #region Encode

        private string EncodeName(string name)
        {
            return System.Convert.ToBase64String(
                System.Text.ASCIIEncoding.ASCII.GetBytes(name)
            );
        }

        #endregion

        #region Create

        public static long ComputeSize(AType argument)
        {
            return 20 * 1024 * 1024;
        }

        public void CreateMemmoryMappedFile(string path, AType argument)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string memoryMappedFileName = EncodeName(path);
            MemoryMappedFile memoryMappedFile;

            try
            {
                memoryMappedFile = MemoryMappedFile.CreateFromFile(
                    new FileStream(path, FileMode.Create),
                    memoryMappedFileName,
                    ComputeSize(argument),
                    MemoryMappedFileAccess.ReadWrite,
                    new MemoryMappedFileSecurity(),
                    HandleInheritability.Inheritable,
                    false
                );

            }
            catch (Exception)
            {
                throw new Error.Invalid("MemoryMappedFile");
            }

            MappedFile mappedFile = new MappedFile(memoryMappedFile);

            mappedFile.Create(argument);
            mappedFile.Dispose();
        }

        #endregion

        #region Read

        public AType Read(string memoryMappadFilePath, bool localWrite)
        {
            string memoryMappedFileName = EncodeName(memoryMappadFilePath);
            MemoryMappedFile memoryMappedFile;

            try
            {
                memoryMappedFile = MemoryMappedFile.OpenExisting(memoryMappedFileName);
            }
            catch (FileNotFoundException)
            {
                memoryMappedFile = MemoryMappedFile.CreateFromFile(memoryMappadFilePath, FileMode.Open, memoryMappedFileName);
            }

            return MappedFile.Read(memoryMappedFile, localWrite);
        }

        #endregion
    }
}
