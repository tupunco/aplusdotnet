using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Selection
{
    class Right : AbstractMonadicFunction
    {
        public override AType Execute(AType argument, Aplus environment = null)
        {
            return argument.IsMemoryMappedFile ? argument.Clone() : argument;
        }
    }
}
