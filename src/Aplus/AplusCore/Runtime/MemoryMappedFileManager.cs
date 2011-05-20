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
            return 1024 * 1024;
        }

        public void CreateMemmoryMappedFile(string path, AType argument)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string memoryMappedFileName = EncodeName(path);

            var memoryMappedFile = MemoryMappedFile.CreateFromFile(
                new FileStream(path, FileMode.Create),
                memoryMappedFileName,
                ComputeSize(argument),
                MemoryMappedFileAccess.ReadWrite,
                new MemoryMappedFileSecurity(),
                HandleInheritability.Inheritable,
                false
            );

            MappedFile mappedFile = new MappedFile(memoryMappedFile);

            mappedFile.Create(argument);
            mappedFile.Dispose();
        }

        #endregion

        #region Read

        public AType Read(string memoryMappadFilePath, byte mode)
        {
            string memoryMappedFileName = EncodeName(memoryMappadFilePath);
            MemoryMappedFile memoryMappedFile;

            try
            {
                memoryMappedFile = MemoryMappedFile.OpenExisting(memoryMappedFileName);
            }
            catch (Exception)
            {
                memoryMappedFile = MemoryMappedFile.CreateFromFile(memoryMappadFilePath, FileMode.Open, memoryMappedFileName);
            }

            return MappedFile.Read(memoryMappedFile);
        }

        #endregion
    }
}
