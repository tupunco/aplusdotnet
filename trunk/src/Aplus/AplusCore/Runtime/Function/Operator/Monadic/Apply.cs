using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Monadic
{
    class Apply
    {
        public AType Execute(AType functionScalar, AType argument, AplusEnvironment environment = null)
        {
            //'Disclose' the function from functionscalar.
            AFunc function = (AFunc)functionScalar.NestedItem.Data;

            //Convert method to the correspond function format.
            if (function.IsBuiltin)
            {
                Func<AplusEnvironment, AType, AType, AType> primitiveFunction =
                    (Func<AplusEnvironment, AType, AType, AType>)function.Method;

                return primitiveFunction(environment, argument, null);
            }
            else
            {
                //If function is user defined, we check the valance.
                if (function.Valence - 1 != 1)
                {
                    throw new Error.Valence("Apply");
                }

                //Convert method to the correspond function format.
                Func<AplusEnvironment, AType, AType> userDefinedFunction =
                    (Func<AplusEnvironment, AType, AType>)function.Method;

                return userDefinedFunction(environment, argument);
            }
        }
    }
}
