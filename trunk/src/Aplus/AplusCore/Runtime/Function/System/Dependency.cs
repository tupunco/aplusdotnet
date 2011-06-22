using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        /// <summary>
        /// System function to get the dependency definition for the given symbol.
        /// </summary>
        /// <param name="environment"><see cref="AplusEnvironment"/></param>
        /// <param name="symbol"><see cref="AType"/> symbol containing the name of the dependency.</param>
        /// <returns>Dependency defintion as a character array or <see cref="ANull"/>.</returns>
        [SystemFunction("_def", "_def{x}: returns the dependency defintion of x, if there is any.")]
        internal static AType DependencyDefinition(AplusEnvironment environment, AType symbol)
        {
            AType result;
            string variableName;
            DependencyItem dependency;
            DependencyManager manager = environment.Runtime.DependencyManager;

            if (!TryQualifiedName(environment, symbol, out variableName))
            {
                throw new Error.Domain("_def");
            }

            if (manager.TryGetDependency(variableName, out dependency))
            {
                result = Helpers.BuildString(dependency.Function.ToString());
            }
            else
            {
                result = Utils.ANull();
            }

            return result;
        }
    }
}
