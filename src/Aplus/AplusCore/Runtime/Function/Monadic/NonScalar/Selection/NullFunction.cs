using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Selection
{
    class NullFunction : AbstractMonadicFunction
    {
        public override AType Execute(AType argument, Aplus environment = null)
        {
            return Utils.ANull();
        }
    }
}
