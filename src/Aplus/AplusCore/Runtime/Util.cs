using System;
using System.IO;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime
{
    internal struct PickAssignmentTarget
    {
        #region Variables

        private AType target;
        private bool fromBox;

        #endregion

        #region Properties

        public AType Target { get { return this.target; } }
        public bool FromBox { get { return this.fromBox; } }

        #endregion

        public PickAssignmentTarget(AType target, bool fromBox)
        {
            this.target = target;
            this.fromBox = fromBox;
        }
    }


    class Util
    {
        /// <summary>
        /// Splits a User Name into Context Name and Variable name parts
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        internal static string[] SplitUserName(string variableName)
        {
            int lastDotPos = variableName.LastIndexOf('.');

            return new string[] { 
                    (lastDotPos <= 0) ? "." :  variableName.Substring(0, lastDotPos), 
                    variableName.Substring(lastDotPos + 1)
                };
        }

        #region File search

        /// <summary>
        /// Gets the absolute path for the supplied <see cref="pathArgument"/>.
        /// </summary>
        /// <remarks>
        /// The search for the file relies on the APATH environment variable.
        /// </remarks>
        /// <param name="environment"></param>
        /// <param name="pathArgument">Must be a Char or Symbol type.</param>
        /// <param name="expectedExtension"></param>
        /// <returns>The absolute path for the file or if not found null.</returns>
        internal static string GetPath(Aplus environment, AType pathArgument, string expectedExtension)
        {
            string path = GetFullPathOrValue(pathArgument, environment);
            string resultPath = null;

            if (path != null && !Path.IsPathRooted(path))
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

                    if (FileSearch(absolutePath, expectedExtension, out resultPath))
                    {
                        break;
                    }
                }
            }
            else
            {
                FileSearch(path, expectedExtension, out resultPath);
            }

            return resultPath;
        }

        /// <summary>
        /// Extracts the path from the supplied <see cref="pathArgument"/>.
        /// </summary>
        /// <param name="pathArgument">Must be a Char or Symbol type.</param>
        /// <param name="environment"></param>
        /// <returns>
        /// Absolute path if the <see cref="pathArgument"/> is a relative/absoulte path.
        /// Name of the file extracted from the <see cref="pathArgument"/> if it is not a relative/absolute path.
        /// Null if the supplied <see cref="AType"/> has an incorrect type.
        /// </returns>
        internal static string GetFullPathOrValue(AType pathArgument, Aplus environment)
        {
            if (pathArgument.Type != ATypes.AChar && pathArgument.Type != ATypes.ASymbol)
            {
                return null;
            }

            AType raveled = pathArgument.Rank > 1 ?
                MonadicFunctionInstance.Ravel.Execute(pathArgument, environment) :
                pathArgument;

            string path;

            if (raveled.Type == ATypes.AChar)
            {
                path = raveled.ToString();
            }
            else if (raveled.Rank > 0)
            {
                path = raveled.Length > 0 ? raveled[0].asString : "";
            }
            else
            {
                path = raveled.asString;
            }

            if (!String.IsNullOrEmpty(path) && !Path.IsPathRooted(path) && !String.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                path = Path.GetFullPath(path);
            }

            return path;
        }

        private static bool FileSearch(string path, string expectedExtension, out string result)
        {
            string appendedExtension = string.Concat(path, expectedExtension);

            if (Path.GetExtension(path) == expectedExtension && File.Exists(path))
            {
                result = path;
            }
            else if (File.Exists(appendedExtension))
            {
                result = appendedExtension;
            }
            else if (File.Exists(path))
            {
                result = path;
            }
            else
            {
                result = null;
            }

            return result != null;
        }

        #endregion
    }
}
