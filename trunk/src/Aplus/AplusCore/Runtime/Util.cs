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

        /// <summary>
        /// Get Atypes from Character.
        /// Abbreviations:
        /// B : Box, C: Char, F: Float, U: Function, I: Integer, N: Null, S : Symbol
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ATypes getType(char type)
        {
            switch (type)
            {
                case 'B':
                    return ATypes.ABox;
                case 'C':
                    return ATypes.AChar;
                case 'F':
                    return ATypes.AFloat;
                case 'U':
                    return ATypes.AFunc;
                case 'I':
                    return ATypes.AInteger;
                case 'N':
                    return ATypes.ANull;
                case 'S':
                    return ATypes.ASymbol;
            }
            return ATypes.AType;
        }

        /// <summary>
        /// Determine right and left types are in accept types. 
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <param name="acceptTypes"></param>
        /// <returns></returns>
         internal static bool TypeCorrect(ATypes right, ATypes left, params string[] acceptTypes)
 	        {
 	            foreach (string item in acceptTypes)
	            {
 	                if (item[0] == '?')
 	                {
 	                    if(right == getType(item[1]))
 	                    {
 	                        return true;
 	                    }
 	                }
	                else if(item[1] == '?')
 	                {
 	                    if (left == getType(item[0]))
 	                    {
 	                        return true;
	                    }
 	                }
 	                else
	                {
 	                    if (left == getType(item[0]) && right == getType(item[1]))
	                    {
	                        return true;
	                    }
	                }
 	            }
 	
	            return false;
 	        }

         internal static bool TypeCorrect(ATypes argument, params char[] acceptTypes)
         {
             foreach (char item in acceptTypes)
             {
                 if (argument == getType(item))
                 {
                     return true;
                 }
             }
             return false;
         }

         #region FileSearch

        /// <summary>
         /// pathArgument type must be char or symbol. Result is the value of pathArgument with full path.
         /// If the result is null, we didn't found the file. When we search, we use the APATH environment.
        /// </summary>
        /// <param name="pathArgument"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
         internal static string GetPath(AType pathArgument, AplusEnvironment environment)
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

             return resultPath;
         }

         /// <summary>
         /// pathArgument type must be char or symbol. The result is the value of the pathArgument,
         /// if it doesn't have path. Else it gives back the value with the full path.
        /// </summary>
        /// <param name="pathArgument"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
         internal static string GetFullPathOrValue(AType pathArgument, AplusEnvironment environment)
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

         private static bool FileSearch(string path, out string result)
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

        #endregion
    }
}
