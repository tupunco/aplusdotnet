using System;
using System.IO;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class Map : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            return left.IsNumber ?
                ReadMemoryMappedFile(right, left, environment) :
                CreateMemoryMappedFile(right, left, environment);
        }

        #endregion

        #region Computation

        private AType ReadMemoryMappedFile(AType right, AType left, AplusEnvironment environment)
        {
            string path = GetPath(right, environment);
            string resultPath = null;

            if (!Path.IsPathRooted(path))
            {
                string apath = Environment.GetEnvironmentVariable("APATH", EnvironmentVariableTarget.User);

                string absolutePath;

                foreach (string item in apath.Split(';'))
                {
                    absolutePath = Path.Combine(item, path);

                    if (!Path.IsPathRooted(absolutePath))
                    {
                        absolutePath = Path.GetFullPath(absolutePath);
                    }

                    if (FileSearch(absolutePath, out resultPath))
                    {
                        break;
                    }
                }
            }
            else
            {
                FileSearch(path, out resultPath);
            }

            if (String.IsNullOrEmpty(resultPath))
            {
                throw new Error.Domain(DomainErrorText);
            }

            int mode = left.asInteger;

            // 0: read, 1: read and write, 2: read and local write
            if (mode != 0 && mode != 1 && mode != 2)
            {
                throw new Error.Domain(DomainErrorText);
            }

            return environment.Runtime.MemoryMappedFileManager.Read(resultPath, (MemoryMappedFileMode)mode);
        }

        private bool FileSearch(string path, out string result)
        {
            bool found = false;
            result = null;

            if (Path.GetExtension(path) == ".m" && File.Exists(path))
            {
                result = path;
                found = true;
            }
            else
            {
                string appendedExtension = path + ".m";

                if (File.Exists(appendedExtension))
                {
                    result = appendedExtension;
                    found = true;
                }
                else if (File.Exists(path))
                {
                    result = path;
                    found = true;
                }
            }

            return found;
        }

        private string GetPath(AType pathArgument, AplusEnvironment environment)
        {
            if (pathArgument.Type != ATypes.AChar && pathArgument.Type != ATypes.ASymbol)
            {
                throw new Error.Domain("Map");
            }

            AType raveled = pathArgument.Rank > 1 ?
                MonadicFunctionInstance.Ravel.Execute(pathArgument, environment) :
                pathArgument;

            string path;

            if (raveled.Type == ATypes.AChar)
            {
                path = raveled.ToString();
            }
            else
            {
                if (raveled.Rank > 0)
                {
                    path = raveled.Length > 0 ? raveled[0].asString : "";
                }
                else
                {
                    path = raveled.asString;
                }
            }

            if (!String.IsNullOrEmpty(path) && !Path.IsPathRooted(path) && !String.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                path = Path.GetFullPath(path);
            }

            return path;
        }

        private AType CreateMemoryMappedFile(AType right, AType left, AplusEnvironment environment)
        {
            string path = GetPath(left, environment);

            if (!path.Contains("."))
            {
                path += ".m";
            }

            if (!right.IsNumber && right.Type != ATypes.AChar)
            {
                throw new Error.Type(TypeErrorText);
            }

            environment.Runtime.MemoryMappedFileManager.CreateMemmoryMappedFile(path, right);

            return Utils.ANull();
        }

        #endregion
    }
}
