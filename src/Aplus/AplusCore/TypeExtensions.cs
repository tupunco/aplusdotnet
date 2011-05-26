using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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
    }
}
