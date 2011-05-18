using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.InteropServices;
using AplusCore.Types.MemoryMapped;

namespace AplusCore.Runtime
{
    public class MemoryMappedFileManager
    {
        #region Variables

        private const byte typePosition         = 4;
        private const byte rankPosition         = 8;
        private const byte lengthPosition       = 12;
        private const byte shapePosition        = 16;
        private const byte leadingAxesPosition  = 52;
        private const byte itemPosition         = 56;

        private const int intSize       = 4;
        private const int doubleSize    = 8;

        //private Dictionary<string, MemoryMappedViewAccessor> accessors;
        
        #endregion

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

            MappedFile mappedFile = new MappedFile(memoryMappedFile, memoryMappedFileName);

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

            return MappedFile.Read(memoryMappedFile, memoryMappedFileName);
        }

        #endregion
    }
}
