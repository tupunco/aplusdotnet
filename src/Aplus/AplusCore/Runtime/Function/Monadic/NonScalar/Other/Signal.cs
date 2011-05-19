using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class Signal : AbstractMonadicFunction
    {

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            string message;
            if (ExtractString(argument, out message))
            {
                throw new Error.Signal(message);
            }

            throw new Error.Domain(this.DomainErrorText);
        }

        private static bool ExtractString(AType argument, out string message)
        {
            AType symbol;
            if (argument.Type == ATypes.ASymbol && argument.TryFirstScalar(out symbol))
            {
                message = symbol.asString;
                return true;
            }
            else if (argument.Type == ATypes.AChar)
            {
                message = MonadicFunctionInstance.Ravel.Execute(argument).ToString();
                return true;
            }

            message = "";
            return false;
        }
    }
}
