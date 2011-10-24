using System.Reflection;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler
{
    /// <summary>
    /// Expression Tree extension methods.
    /// </summary>
    internal static class ExpressionTreeExtensions
    {
        /// <see cref="Expression.Property"/>
        internal static DLR.MemberExpression Property(this DLR.Expression target, string propertyName)
        {
            return DLR.Expression.Property(target, propertyName);
        }

        /// <summary>
        /// Build <see cref="DLR.MethodCallExperssion"/> from the given arguments.
        /// </summary>
        /// <remarks>
        /// This is a shorthand for a simple <see cref="DLR.Experssion.Call"/> invokation.
        /// </remarks>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance on which the method will be called.</param>
        /// <param name="methodName">The name of the method in the given type.</param>
        /// <param name="arguments">The additional arguments for the method call.</param>
        /// <returns><see cref="DLR.MethodCallExperssion"/> built using the parameters.</returns>
        internal static DLR.MethodCallExpression Call<T>(
            this DLR.Expression instance, string methodName, params DLR.Expression[] arguments)
        {
            return DLR.Expression.Call(instance, typeof(T).GetMethod(methodName), arguments);
        }

        /// <summary>
        /// Wraps the expression inside a Dynamic cast to AType
        /// </summary>
        /// <param name="expression">Expression to cast.</param>
        /// <param name="runtime"></param>
        /// <returns></returns>
        internal static DLR.DynamicExpression ToAType(this DLR.Expression expression, Aplus runtime)
        {
            return DLR.Expression.Dynamic(runtime.ConvertBinder(typeof(AType)), typeof(AType), expression);
        }

        /// <summary>
        /// Creates an Expression Tree representing a cast to the given <see cref="T"/> type.
        /// </summary>
        /// <typeparam name="T">Specifies the type to cast to.</typeparam>
        /// <param name="target">Expression Tree to cast.</param>
        /// <returns>Convert Expression Tree</returns>
        internal static DLR.UnaryExpression To<T>(this DLR.Expression target)
        {
            return DLR.Expression.Convert(target, typeof(T));
        }
    }
}
