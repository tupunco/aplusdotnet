using System;
using System.Collections.Generic;
using System.Reflection;

using AplusCore.Types;

namespace AplusCore.Runtime.Context
{
    /// <summary>
    /// Internal class to perform context loading.
    /// </summary>
    internal class ContextLoader
    {
        /// <summary>
        /// Internal types.
        /// </summary>
        private static readonly Type[] internalContextTypes = Assembly.GetExecutingAssembly().GetTypes();

        /// <summary>
        /// Search for functions inside the given context.
        /// </summary>
        /// <param name="contextName">Specifies which context is needed.</param>
        /// <returns>Name and function key-value pairs for the functions inside the given context.</returns>
        internal static IDictionary<string, AType> FindContextElements(string contextName)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            Dictionary<string, AType> elements = new Dictionary<string, AType>();

            foreach (Type type in internalContextTypes)
            {
                AplusContextAttribute typeAttribute =
                    type.GetSingleAttribute<AplusContextAttribute>();

                if (typeAttribute == null || typeAttribute.ContextName != contextName)
                {
                    continue;
                }

                foreach (MethodInfo method in type.GetMethods(flags))
                {
                    AplusContextFunctionAttribute methodAttribute =
                        method.GetSingleAttribute<AplusContextFunctionAttribute>();

                    if (methodAttribute == null)
                    {
                        continue;
                    }

                    string methodName = String.Format("{0}.{1}", contextName, methodAttribute.ContextName);
                    AType afunction = method.BuildAFunction(methodName, methodAttribute.Description);

                    if (afunction == null)
                    {
                        continue;
                    }

                    elements[methodAttribute.ContextName] = afunction;
                }
            }

            return elements;
        }
    }
}
