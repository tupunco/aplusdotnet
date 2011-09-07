using System;
using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Monadic
{
    class Rank
    {
        #region DLR entry point

        public AType Execute(AType function, AType n, AType argument, Aplus environment = null)
        {

            if (!(function.Data is AFunc))
            {
                throw new Error.NonFunction("Rank");
            }

            AFunc func = (AFunc)function.Data;

            if (!func.IsBuiltin)
            {
                if (func.Valence - 1 != 1)
                {
                    throw new Error.Valence("Rank");
                }
            }

            AType result;
            int number = GetNumber(argument, n, environment);

            if (argument.Shape.Contains(0))
            {
                result = NullWalker(argument, func, number, environment);
            }
            else
            {
                ATypes typeChecker = ATypes.AType;
                bool floatConvert = false;

                result = Walker(argument, func, number, environment, ref typeChecker, ref floatConvert);

                if (floatConvert && result.IsArray)
                {
                    result.ConvertToFloat();
                }
            }

            return result;
        }

        #endregion

        #region Utility

        private int GetNumber(AType argument, AType n, Aplus environment)
        {
            if (n.Type != ATypes.AInteger)
            {
                throw new Error.Type("Rank");
            }

            int length = n.Shape.Product();

            if (length > 3)
            {
                throw new Error.Length("Rank");
            }

            AType raveledArray = MonadicFunctionInstance.Ravel.Execute(n, environment);
            int result = raveledArray[0].asInteger;

            if (result < 0)
            {
                result = Math.Max(0, argument.Rank - Math.Abs(result));
            }
            else
            {
                result = Math.Min(result, argument.Rank);
            }

            return result;
        }

        private AType NullWalker(AType argument, AFunc function, int number, Aplus environment)
        {
            AType result;

            if (argument.Type == ATypes.ANull)
            {
                if (number == 0)
                {
                    throw new Error.Type("Rank");
                }

                result = ExecuteFunction(function, Utils.ANull(), environment);

                if (result.Type == ATypes.ABox || result.Type == ATypes.ASymbol)
                {
                    result.Type = ATypes.ANull;
                }
            }
            else
            {
                result = AArray.Create(ATypes.AArray);

                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape.GetRange(0, argument.Rank));
                result.Rank = argument.Rank;
                result.Type = ExecuteFunction(function, Utils.ANull(argument.Type), environment).Type;
            }

            return result;
        }

        private AType Walker(
            AType argument, AFunc function, int number, Aplus environment, ref ATypes typeChecker, ref bool floatConvert)
        {
            AType result;

            if (argument.Rank != number)
            {
                result = AArray.Create(ATypes.AArray);

                foreach (AType item in argument)
                {
                    AType resultItem = Walker(item, function, number, environment, ref typeChecker, ref floatConvert);

                    if (typeChecker == ATypes.AType)
                    {
                        typeChecker = resultItem.Type;
                    }
                    else if (typeChecker == ATypes.AFloat && resultItem.Type == ATypes.AInteger ||
                             resultItem.Type == ATypes.AFloat && typeChecker == ATypes.AInteger)
                    {
                        floatConvert = true;
                    }
                    else if (typeChecker != resultItem.Type)
                    {
                        throw new Error.Type("Rank");
                    }

                    result.AddWithNoUpdate(resultItem);
                }

                result.UpdateInfo();
            }
            else
            {
                result = ExecuteFunction(function, argument, environment);
            }

            return result;
        }

        private static AType ExecuteFunction(AFunc function, AType argument, Aplus environment)
        {
            AType result;

            if (function.IsBuiltin)
            {
                var method = (Func<Aplus, AType, AType, AType>)function.Method;
                result = method(environment, argument, null);
            }
            else
            {
                var method = (Func<Aplus, AType, AType>)function.Method;
                result = method(environment, argument);
            }

            return result;
        }

        #endregion
    }
}
