using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Dyadic
{
    class Apply
    {
        public AType Execute(AType functionScalar, AType right, AType left, Aplus environment = null)
        {
            //'Disclose' the function from functionscalar.
            AFunc func = (AFunc)functionScalar.NestedItem.Data;

            //If function is user defined, we check the valance.
            if (!func.IsBuiltin)
            {
                if (func.Valence - 1 != 2)
                {
                    throw new Error.Valence("Apply");
                }
            }

            //Convert method to the correspond function format.
            var function = (Func<Aplus, AType, AType, AType>)func.Method;

            return function(environment, right, left);
        }
    }
}
