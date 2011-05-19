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
    }
}
