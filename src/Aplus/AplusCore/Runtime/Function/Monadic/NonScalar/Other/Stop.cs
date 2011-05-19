using Microsoft.Scripting.Utils;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Other
{
    class Stop : AbstractMonadicFunction
    {
        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            Assert.NotNull(environment);

            switch (environment.Runtime.SystemVariables["stop"].asInteger)
            {
                case 1:
                    throw new StopException("stop");
                case 2:
                    MonadicFunctionInstance.Print.Execute(argument, environment);
                    break;

                default:
                    break;
            }

            return argument;
        }
    }
}
