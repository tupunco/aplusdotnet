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

        /// <summary>
        /// Encode the name (= file name with path), result is the name of a memory-mapped file.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string EncodeName(string name)
        {
            return System.Convert.ToBase64String(
                System.Text.ASCIIEncoding.ASCII.GetBytes(name)
            );
        }

        #endregion

        #region Create

        /// <summary>
        /// Create a memory-mapped file with the given path and argument.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="argument"></param>
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

        /// <summary>
        /// Read memory-mapped file from a file with the mode, and result is an AType.
        /// </summary>
        /// <param name="memoryMappadFilePath"></param>
        /// <param name="memoryMappedFileMode"></param>
        /// <returns></returns>
        public AType Read(string memoryMappadFilePath, MemoryMappedFileMode memoryMappedFileMode)
        {
            MemoryMappedFile memoryMappedFile;

            GetMemoryMappedFile(memoryMappadFilePath, out memoryMappedFile);
              
            return MappedFile.Read(memoryMappedFile, memoryMappedFileMode);
        }

        /// <summary>
        /// Create a Memory-mapped file from file in out parameter, and the result is true, if exist.
        /// </summary>
        /// <param name="memoryMappadFilePath"></param>
        /// <param name="memoryMappedFile"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return true, if the memory-mapped file is exist.
        /// </summary>
        /// <param name="memoryMappedFilePath"></param>
        /// <returns></returns>
        public bool ExistMemoryMappedFile(string memoryMappedFilePath)
        {
            MemoryMappedFile memoryMappedFile;

            bool result = GetMemoryMappedFile(memoryMappedFilePath, out memoryMappedFile);

            memoryMappedFile.Dispose();

            return result;
        }

        #endregion

        #region Expand/Decrease

        /// <summary>
        /// It gives back the leading axes (_Items function modify this value) of a memory-mapped file.
        /// </summary>
        /// <param name="memoryMappedFilePath"></param>
        /// <returns></returns>
        public AType GetLeadingAxesLength(string memoryMappedFilePath)
        {
            MemoryMappedFile memoryMappedFile;

            GetMemoryMappedFile(memoryMappedFilePath, out memoryMappedFile);

            MappedFile mappedFile = new MappedFile(memoryMappedFile);

            AType result = AInteger.Create(mappedFile.LeadingAxes);

            mappedFile.Dispose();

            return result;
        }

        /// <summary>
        /// Increase or decreade the size of a memory-mapped file.
        /// </summary>
        /// <param name="memoryMappedFilePath"></param>
        /// <param name="newLeadingAxesLength"></param>
        /// <returns></returns>
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
