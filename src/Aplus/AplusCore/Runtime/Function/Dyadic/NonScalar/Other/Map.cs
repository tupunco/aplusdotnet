using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.InteropServices;


using System.Runtime.Serialization.Formatters.Binary;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class Map : AbstractDyadicFunction
    {
        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {

            if (left.IsNumber)
            {
                return ReadMemoryMappedFile(right, left, environment);
            }

            CreateMemoryMappedFile(right, left, environment);

            return Utils.ANull();
        }

        private AType ReadMemoryMappedFile(AType right, AType left, AplusEnvironment environment)
        {
            return environment.Runtime.MemoryMappedFileManager.Read(getPath(right), (byte)left.asInteger);
        }

        private void CreateMemoryMappedFile(AType right, AType left, AplusEnvironment environment)
        {
            environment.Runtime.MemoryMappedFileManager.CreateMemmoryMappedFile(getPath(left), right);
        }

        private string getPath(AType argument)
        {
            if (argument.Type == ATypes.AChar)
            {
                return argument.ToString();
            }

            return null;
        }

    }
}
