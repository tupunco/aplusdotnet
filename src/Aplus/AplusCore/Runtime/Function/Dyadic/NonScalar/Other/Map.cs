using System;
using System.IO;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class Map : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            return left.IsNumber ?
                ReadMemoryMappedFile(right, left, environment) :
                CreateMemoryMappedFile(right, left, environment);
        }

        #endregion

        #region Computation

        private AType ReadMemoryMappedFile(AType right, AType left, Aplus environment)
        {
            string resultPath = Util.GetPath(right, environment);

            if (resultPath == null)
            {
                throw new Error.Domain(DomainErrorText);
            }

            int mode = left.asInteger;

            // 0: read, 1: read and write, 2: read and local write
            if (mode != 0 && mode != 1 && mode != 2)
            {
                throw new Error.Domain(DomainErrorText);
            }

            return environment.MemoryMappedFileManager.Read(resultPath, (MemoryMappedFileMode)mode);
        }

        private AType CreateMemoryMappedFile(AType right, AType left, Aplus environment)
        {
            string path = Util.GetFullPathOrValue(left, environment);

            if (path == null)
            {
                throw new Error.Domain(DomainErrorText);
            }

            if (!path.Contains("."))
            {
                path += ".m";
            }

            if (!right.IsNumber && right.Type != ATypes.AChar)
            {
                throw new Error.Type(TypeErrorText);
            }

            environment.MemoryMappedFileManager.CreateMemmoryMappedFile(path, right);

            return Utils.ANull();
        }

        #endregion
    }
}
