using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        /// <summary>
        /// Increase/truncate the size of a memory-mapped file, or get the size of the file.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="memoryMappedFileName"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [SystemFunction("_items", "_items{y;x}: Increase/truncate the size of a memory-mapped file, or get the size of the file.")]
        internal static AType Items(Aplus environment, AType memoryMappedFileName, AType number)
        {
            string resultPath = Util.GetPath(memoryMappedFileName, environment);

            if (resultPath == null)
            {
                throw new Error.Domain("Items");
            }

            int newLeadingAxesLength = GetLeadingAxesLength(number);

            if (newLeadingAxesLength == -1)
            {
                return environment.MemoryMappedFileManager.GetLeadingAxesLength(resultPath);
            }
            else
            {
                if (environment.MemoryMappedFileManager.ExistMemoryMappedFile(resultPath))
                {
                    throw new Error.Invalid("Items");
                }

                return environment.MemoryMappedFileManager.ExpandOrDecrease(resultPath, newLeadingAxesLength);
            }
        }

        private static int GetLeadingAxesLength(AType argument)
        {
            if (!argument.IsNumber)
            {
                throw new Error.Type("Items");
            }

            AType result;
            if (!argument.TryFirstScalar(out result, true))
            {
                throw new Error.Length("Items");
            }


            int number;
            if (!result.ConvertToRestrictedWholeNumber(out number))
            {
                throw new Error.Type("Items");
            }

            if (number != -1 && number < 0)
            {
                throw new Error.Domain("Items");
            }

            return number;
        }
    }
}
