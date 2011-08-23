using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Dyadic
{
    /* Skeleton of Each definition in A+ :
         * 
            y (f Each) x:
            {
                s := rho x;
	
                x := , x;

		        y := , y;

                z := (rho x) rho < ();

                (i := rho z) do z[i] := < (>i#y) f >i#x;

                s rho z
            }
         * 
         */
    class Each
    {
        public AType Execute(AType function, AType right, AType left, Aplus environment = null)
        {
            //If function is not function, we throw an exception.
            if (!(function.Data is AFunc))
            {
                throw new Error.NonFunction("Each");
            }

            AFunc func = (AFunc)function.Data;

            //If left and right side is array, we chech rank and shape. 
            if (right.IsArray && left.IsArray)
            {
                if (right.Rank != left.Rank)
                {
                    throw new Error.Rank("Each");
                }

                if (!right.Shape.SequenceEqual(left.Shape))
                {
                    throw new Error.Length("Each");
                }
            }

            //If function is user defined, we check function is dyadic.
            if (!func.IsBuiltin)
            {
                if (func.Valence - 1 != 2)
                {
                    throw new Error.Valence("Each");
                }
            }

            //Null array flag.
            bool isNull = right.Shape.Contains(0) || left.Shape.Contains(0);

            return Walker(left, right, environment, isNull, func);
        }

        /// <summary>
        /// Execute 'dyadic' Each algorithm.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="environment"></param>
        /// <param name="isNull"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        private AType Walker(AType left, AType right, Aplus environment, bool isNull, AFunc function)
        {
            if (left.IsArray)
            {
                AType result = AArray.Create(ATypes.AArray);

                AType rightArray = right.IsArray ? right : AArray.Create(right.Type, right);

                for (int i = 0; i < left.Length; i++)
                {
                    result.AddWithNoUpdate(Walker(left[i], rightArray[right.IsArray ? i : 0], environment, isNull, function));
                }

                result.Type = isNull ? ATypes.ANull : ATypes.ABox;
                result.Length = left.Length;
                result.Shape = new List<int>(left.Shape);
                result.Rank = left.Rank;

                return result;
            }
            else
            {
                if (right.IsArray)
                {
                    AType result = AArray.Create(ATypes.AArray);

                    for (int i = 0; i < right.Length; i++)
                    {
                        result.AddWithNoUpdate(Walker(left, right[i], environment, isNull, function));
                    }

                    result.Type = isNull ? ATypes.ANull : ATypes.ABox;
                    result.Length = right.Length;
                    result.Shape = new List<int>(right.Shape);
                    result.Rank = right.Rank;

                    return result;
                }
                else
                {
                    //Disclose left and right scalar argument.
                    AType disclosedRight = MonadicFunctionInstance.Disclose.Execute(right, environment);
                    AType disclosedLeft = MonadicFunctionInstance.Disclose.Execute(left, environment);

                    var method = (Func<Aplus, AType, AType, AType>)function.Method;

                    //Execute the holden function and enclose the result.
                    AType result = method(environment, disclosedRight, disclosedLeft);
                    result = MonadicFunctionInstance.Enclose.Execute(result);

                    return result;
                }
            }
        }
    }
}
