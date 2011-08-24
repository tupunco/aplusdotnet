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
        #region Variables

        /// <summary>
        /// Internal types.
        /// </summary>
        private static readonly Type[] internalContextTypes = Assembly.GetExecutingAssembly().GetTypes();

        private Aplus environment;

        #endregion

        #region Constructors

        internal ContextLoader(Aplus environment)
        {
            this.environment = environment;
        }

        #endregion

        #region Context element discover

        /// <summary>
        /// Search for functions inside the given context.
        /// </summary>
        /// <param name="contextName">Specifies which context is needed.</param>
        /// <returns>Name and function key-value pairs for the functions inside the given context.</returns>
        internal IDictionary<string, AType> FindContextElements(string contextName)
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

                List<MethodInfo> contextInitMethods = new List<MethodInfo>();

                foreach (MethodInfo method in type.GetMethods(flags))
                {
                    AplusContextFunctionAttribute methodAttribute =
                        method.GetSingleAttribute<AplusContextFunctionAttribute>();

                    AplusContextInitAttribute initAttribute =
                        method.GetSingleAttribute<AplusContextInitAttribute>();

                    if (initAttribute != null)
                    {
                        // Initialize context
                        method.Invoke(null, new object[] { this.environment });
                    }

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

        #endregion
    }
}
