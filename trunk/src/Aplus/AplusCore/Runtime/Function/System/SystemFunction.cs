using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Runtime.Function
{
    /// <summary>
    /// Class to contain all base system functions.
    /// </summary>
    internal static partial class SystemFunction
    {
        /// <summary>
        /// Searches for System functions inside the <see cref="SystemFunction"/> class.
        /// </summary>
        /// <returns>Returns a dictionary of system function names and the <see cref="AFunc"/> for each.</returns>
        internal static Dictionary<string, AType> DiscoverSystemFunctions()
        {
            Dictionary<string, AType> functions = new Dictionary<string, AType>();

            Type systemFunctionType = typeof(SystemFunction);
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (MethodInfo method in systemFunctionType.GetMethods(flags))
            {
                SystemFunctionAttribute attibute = method.GetSingleAttribute<SystemFunctionAttribute>();

                if (attibute == null)
                {
                    continue;
                }

                if (functions.ContainsKey(attibute.Name))
                {
                    // TODO: what should we do here? Exception? Overwrite?
                }

                ParameterInfo[] parameters = method.GetParameters();
                List<Type> types = new List<Type>(parameters.Select(parameter => parameter.ParameterType));
                types.Add(method.ReturnType);

                Delegate methodDelegate = Delegate.CreateDelegate(DLR.Expression.GetFuncType(types.ToArray()), method);

                functions[attibute.Name] = AFunc.Create(attibute.Name, methodDelegate, parameters.Length, attibute.Description);
            }

            return functions;
        }

        /// <summary>
        /// Tries to convert the given <see cref="AType"/> to a qualified name.
        /// </summary>
        /// <param name="environment"><see cref="AplusEnvironment"/></param>
        /// <param name="symbol"><see cref="AType"/> to convert to qualified name</param>
        /// <param name="qualifiedName">Qualified name from the given <see cref="AType"/>.</param>
        /// <returns>True if ther is a valid qualified name otherwise false.</returns>
        internal static bool TryQualifiedName(AplusEnvironment environment, AType symbol, out string qualifiedName)
        {
            string currentContext = environment.Runtime.CurrentContext;
            bool result = false;

            if (symbol.Type != ATypes.ASymbol)
            {
                qualifiedName = null;
                return result;
            }

            switch (symbol.Length)
            {
                case 1:
                    qualifiedName = symbol.asString;
                    if (!qualifiedName.Contains('.'))
                    {
                        qualifiedName = string.Join(".",
                            (currentContext == "." ? "" : currentContext),
                            qualifiedName
                        );
                    }

                    result = true;
                    break;
                case 2:
                    qualifiedName = string.Join(".", symbol[0].asString, symbol[1].asString);
                    result = true;
                    break;
                default:
                    qualifiedName = null;
                    break;
            }

            return result;
        }
    }
}
