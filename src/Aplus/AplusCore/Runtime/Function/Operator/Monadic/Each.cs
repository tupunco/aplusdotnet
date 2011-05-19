using System;
using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Monadic
{
    /* Definition of Each in A+ :
         * 
            (f Each) x:
            {
                s := rho x;
	
                x := , x;

                z := (rho x) rho < ();

                (i := rho z) do z[i] := < f > x[i];

                s rho z
	
            }
         * 
         */

    class Each
    {

        public AType Execute(AType function, AType argument, AplusEnvironment environment = null)
        {
            //If function is not function, we throw an exception.
            if (!(function.Data is AFunc))
            {
                throw new Error.NonFunction("Each");
            }

            AFunc func = (AFunc)function.Data;

            //If function is user defined, we check function is monadic.
            if (!func.IsBuiltin)
            {
                if (func.Valence - 1 != 1)
                {
                    throw new Error.Valence("Each");
                }
            }

            return Walker(argument, environment, argument.Shape.Contains(0), func);
        }

        /// <summary>
        /// Execute 'monadic' Each algorithm.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="environment"></param>
        /// <param name="isNull"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        private AType Walker(AType argument, AplusEnvironment environment, bool isNull, AFunc function)
        {
            if (argument.IsArray)
            {
                AType result = AArray.Create(ATypes.AArray);

                foreach (AType item in argument)
                {
                    result.AddWithNoUpdate(Walker(item, environment, isNull, function));
                }

                result.Type = isNull ? ATypes.ANull : ATypes.ABox;
                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape);
                result.Rank = argument.Rank;

                return result;
            }
            else
            {
                //Disclose scalar argument.
                AType result = MonadicFunctionInstance.Disclose.Execute(argument);

                //Pick the holden function and apply it.
                if (function.IsBuiltin)
                {
                    var method = (Func<AplusEnvironment, AType, AType, AType>)function.Method;
                    result = method(environment, result, null);
                }
                else
                {
                    var method = (Func<AplusEnvironment, AType, AType>)function.Method;
                    result = method(environment, result);
                }

                //Enclose the result.
                result = MonadicFunctionInstance.Enclose.Execute(result);
                return result;
            }
        }

    }
}
