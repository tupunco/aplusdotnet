using System;

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
    }
}
