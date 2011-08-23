using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Monadic
{
    class Apply
    {
        public AType Execute(AType functionScalar, AType argument, Aplus environment = null)
        {
            //'Disclose' the function from functionscalar.
            AFunc function = (AFunc)functionScalar.NestedItem.Data;

            //Convert method to the correspond function format.
            if (function.IsBuiltin)
            {
                Func<Aplus, AType, AType, AType> primitiveFunction =
                    (Func<Aplus, AType, AType, AType>)function.Method;

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
                Func<Aplus, AType, AType> userDefinedFunction =
                    (Func<Aplus, AType, AType>)function.Method;

                return userDefinedFunction(environment, argument);
            }
        }
    }
}
