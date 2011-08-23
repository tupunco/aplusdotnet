using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        /// <summary>
        /// Exits with the exitcode given as parameter.
        /// </summary>
        /// <param nave="environment"></param>
        /// <param name="input">exitcode</param>
        [SystemFunction("_exit", "_exit{x}: exits with exitcode x")]
        internal static AType Exit(Aplus environment, AType argument)
        {
            AType result;
            bool isFirstScalar = argument.TryFirstScalar(out result, true);

            if (isFirstScalar && !result.IsTolerablyWholeNumber || 
                !(argument.Type == ATypes.AInteger || argument.Type == ATypes.AFloat))
            {
                throw new Error.Type("_exit");
            }

            if (!isFirstScalar)
            {
                throw new Error.Length("_exit");
            }

            Environment.Exit(result.asInteger);

            // unreachable code
            return argument;
        }
    }
}
