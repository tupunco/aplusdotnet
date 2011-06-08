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
            MemoryMappedFile memoryMappedFile;

            GetMemoryMappedFile(memoryMappadFilePath, out memoryMappedFile);
              
            return MappedFile.Read(memoryMappedFile, memoryMappedFileMode);
        }

        private bool GetMemoryMappedFile(string memoryMappadFilePath, out MemoryMappedFile memoryMappedFile)
        {
            string memoryMappedFileName = EncodeName(memoryMappadFilePath);
            bool exist = false;

            try
            {
                memoryMappedFile = MemoryMappedFile.OpenExisting(memoryMappedFileName);
                exist = true;
            }
            catch (FileNotFoundException)
            {
                memoryMappedFile = MemoryMappedFile.CreateFromFile(memoryMappadFilePath, FileMode.Open, memoryMappedFileName);
            }

            return exist;
        }

        public bool ExistMemoryMappedFile(string memoryMappedFilePath)
        {
            MemoryMappedFile memoryMappedFile;

            bool result = GetMemoryMappedFile(memoryMappedFilePath, out memoryMappedFile);

            memoryMappedFile.Dispose();

            return result;
        }

        #endregion

        #region Expand/Decrease

        public AType GetLeadingAxesLength(string memoryMappedFilePath)
        {
            MemoryMappedFile memoryMappedFile;

            GetMemoryMappedFile(memoryMappedFilePath, out memoryMappedFile);

            MappedFile mappedFile = new MappedFile(memoryMappedFile);

            AType result = AInteger.Create(mappedFile.LeadingAxes);

            mappedFile.Dispose();

            return result;
        }

        public AType ExpandOrDecrease(string memoryMappedFilePath, int newLeadingAxesLength)
        {
            string memoryMappedFileName = EncodeName(memoryMappedFilePath);

            var memoryMappedFile = MemoryMappedFile.CreateFromFile(memoryMappedFilePath, FileMode.Open, memoryMappedFileName);
            
            MappedFile mappedFile = new MappedFile(memoryMappedFile);

            int newSize = mappedFile.ComputeNewSize(newLeadingAxesLength);
            int oldLeadingAxesLength = mappedFile.LeadingAxes;

            if (mappedFile.LeadingAxes < newLeadingAxesLength)
            {
                mappedFile.LeadingAxes = newLeadingAxesLength;
                mappedFile.Dispose();

                memoryMappedFile = MemoryMappedFile.CreateFromFile(memoryMappedFilePath, FileMode.Open, memoryMappedFileName, newSize);
                memoryMappedFile.Dispose();
            }
            else if (newLeadingAxesLength < mappedFile.LeadingAxes)
            {
                mappedFile.LeadingAxes = mappedFile.Length = newLeadingAxesLength;
                mappedFile.Dispose();

                FileStream fileStream = new FileStream(memoryMappedFilePath, FileMode.Open);
                fileStream.SetLength(newSize);
                fileStream.Close();
            }

            return AInteger.Create(oldLeadingAxesLength);
        }

        #endregion
    }
}
