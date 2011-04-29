using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;
using AplusCore.Runtime.Function.Monadic;

namespace AplusCore.Runtime.Function.Operator.Monadic
{
    class Rank
    {
        private ATypes check;
        private bool convert;

        public AType Execute(AType function, AType n, AType argument, AplusEnvironment environment = null)
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

            int number = GetNumber(argument, n, environment);

            if (argument.Shape.Contains(0))
            {
                return NullWalker(argument, func, number, environment);
            }

            this.check = ATypes.AType;
            this.convert = false;

            AType result = Walker(argument, func, number, environment);

            if (this.convert && result.IsArray)
            {
                result.ConvertToFloat();
            }

            return result;
        }

        private int GetNumber(AType argument, AType n, AplusEnvironment environment)
        {
            if (n.Type != ATypes.AInteger)
            {
                throw new Error.Type("Rank");
            }

            int length = 1;
            for (int i = 0; i < n.Shape.Count; i++)
            {
                length *= n.Shape[i];
            }

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

        private AType NullWalker(AType argument, AFunc function, int number, AplusEnvironment environment)
        {
            if (argument.Type == ATypes.ANull)
            {
                if (number == 0)
                {
                    throw new Error.Type("Rank");
                }
                else
                {
                    AType result = ExecuteFunction(function, Utils.ANull(), environment);

                    if (result.Type == ATypes.ABox || result.Type == ATypes.ASymbol)
                    {
                        result.Type = ATypes.ANull;
                    }

                    return result;
                }
            }
            else
            {
                AType result = AArray.Create(ATypes.AArray);

                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape.GetRange(0, argument.Rank));
                result.Rank = argument.Rank;
                result.Type = ExecuteFunction(function, Utils.ANull(argument.Type), environment).Type;

                return result;
            }
        }


        private AType Walker(AType argument, AFunc function, int number, AplusEnvironment environment)
        {
            if (argument.Rank != number)
            {
                AType result = AArray.Create(ATypes.AArray);
                AType temp;

                foreach (AType item in argument)
                {
                    temp = Walker(item, function, number, environment);

                    if (this.check == ATypes.AType)
                    {
                        this.check = temp.Type;
                    }

                    if (this.check != temp.Type)
                    {
                        if (this.check == ATypes.AFloat && temp.Type == ATypes.AInteger ||
                           temp.Type == ATypes.AFloat && this.check == ATypes.AInteger)
                        {
                            this.convert = true;
                        }
                        else
                        {
                            throw new Error.Type("Rank");
                        }
                    }

                    result.AddWithNoUpdate(temp);
                }

                result.UpdateInfo();

                return result;
            }
            else
            {
                return ExecuteFunction(function, argument, environment);
            }
        }

        private AType ExecuteFunction(AFunc function, AType argument, AplusEnvironment environment)
        {
            AType result;

            if (function.IsBuiltin)
            {
                var method = (Func<AplusEnvironment, AType, AType, AType>)function.Method;
                result = method(environment, argument, null);
            }
            else
            {
                var method = (Func<AplusEnvironment, AType, AType>)function.Method;
                result = method(environment, argument);
            }

            return result;
        }
    }
}
