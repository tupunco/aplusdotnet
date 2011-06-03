using System;
using System.IO;
using System.IO.MemoryMappedFiles;

using AplusCore.Types;

namespace AplusCore.Runtime
{

    public enum MemoryMappedFileMode
    {
        Read,
        ReadAndWrite,
        LocalWrite
    }


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
                    MappedFile.ComputeSize(argument),
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

        public AType Read(string memoryMappadFilePath, MemoryMappedFileMode memoryMappedFileMode)
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

            return MappedFile.Read(memoryMappedFile, memoryMappedFileMode);
        }

        #endregion
    }
}
