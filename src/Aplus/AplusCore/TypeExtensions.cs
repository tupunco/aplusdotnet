using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Returns a single attribute instance attached to the <see cref="System.Reflection.MemberInfo"/>.
        /// </summary>
        /// <typeparam name="T">Type of the custom attribute to search for.</typeparam>
        /// <param name="info">
        /// <see cref="System.Reflection.MemberInfo"/> where the attributes will be searched.
        /// </param>
        /// <returns>If there is only one such attribute, than it'll be returned. Otherwise null.</returns>
        internal static T GetSingleAttribute<T>(this MemberInfo info) where T : Attribute
        {
            T firstAttribute = null;
            object[] attributes = info.GetCustomAttributes(typeof(T), false);

            if (attributes.Length == 1)
            {
                firstAttribute = (T)attributes[0];
            }

            return firstAttribute;
        }

        /// <summary>
        /// Build an AFunction for the given method.
        /// </summary>
        /// <param name="method">The method to wrap inside an AFunction.</param>
        /// <param name="name">The name of AFunction.</param>
        /// <param name="description">Description for the AFunction.</param>
        /// <returns>Returns an AFunction for the given method.</returns>
        internal static AType BuildAFunction(this MethodInfo method, string name, string description)
        {
            ParameterInfo[] parameters = method.GetParameters();
            List<Type> types = new List<Type>(parameters.Select(parameter => parameter.ParameterType));
            types.Add(method.ReturnType);

            Delegate methodDelegate = Delegate.CreateDelegate(DLR.Expression.GetFuncType(types.ToArray()), method);

            return AFunc.Create(name, methodDelegate, parameters.Length, description);
        }
    }
}
