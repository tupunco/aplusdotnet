using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Informational
{
    class TypeFunction : AbstractMonadicFunction
    {
        /// <summary>
        /// returns the type of the argument in a ASymbol
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            return ASymbol.Create(argument.Type.ToTypeString());
        }

    }
}
