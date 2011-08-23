using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Informational
{
    class Count : AbstractMonadicFunction
    {

        /// <summary>
        /// Returns the number of items of the input argument
        /// </summary>
        /// <remarks>
        /// If the argument is scalar the result is 1.
        /// If the argument is a nonscalar the result is the length of the leading axis
        /// </remarks>
        public override AType Execute(AType argument, Aplus environment = null)
        {
            return AInteger.Create(argument.Length);
        }

    }
}
