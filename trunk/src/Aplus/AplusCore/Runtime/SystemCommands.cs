using System;
using System.IO;

using Microsoft.Scripting.Runtime;

using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime
{
    class SystemCommands
    {
        /// <summary>
        /// Prints the list of contexts
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static AType PrintContexts(DYN.IDynamicMetaObjectProvider scope)
        {
            DYN.DynamicMetaObject storage = scope.GetMetaObject(
                DLR.Expression.Property(
                    DLR.Expression.Convert(DLR.Expression.Constant(scope), typeof(Scope)),
                    "Storage"
                )
            );

            Console.WriteLine(String.Join(" ", storage.GetDynamicMemberNames()));

            return Utils.ANull();
        }

        /// <summary>
        /// Writes data to the specified file.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="variableName">
        /// The name of the variable which stores the data to write into the file.
        /// </param>
        /// <param name="filename">The filename to write the data to.</param>
        public static void WriteToFile(Aplus environment, string variableName, string filename)
        {
            AType data = null;

            try
            {
                data =
                    Function.Monadic.MonadicFunctionInstance.Value.Execute(ASymbol.Create(variableName), environment);
            }
            catch (Error)
            {
                // intentionally left blank
            }

            if (data != null)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                    {
                        foreach (char item in data.ToString())
                        {
                            fileStream.WriteByte((byte)item);
                        }
                    }
                }
                catch (ArgumentException)
                {
                    throw new Error.Invalid("$>");
                }
            }
            else
            {
                Console.WriteLine("bad varname");
            }
        }
    }
}
