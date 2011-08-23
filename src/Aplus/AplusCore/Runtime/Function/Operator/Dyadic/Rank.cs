using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Dyadic
{
    class Rank
    {
        #region Variables

        private bool convert;
        private ATypes check;

        #endregion

        #region Entry point

        public AType Execute(AType function, AType n, AType right, AType left, Aplus environment = null)
        {
            if (!(function.Data is AFunc))
            {
                throw new Error.NonFunction("Rank");
            }

            AFunc func = (AFunc)function.Data;

            if (!func.IsBuiltin)
            {
                if (func.Valence - 1 != 2)
                {
                    throw new Error.Valence("Rank");
                }
            }

            List<int> numberList = GetNumberList(n, left, right, environment);

            this.check = ATypes.AType;
            this.convert = false;

            AType result = Walker(left, right, numberList, func, environment);

            if (this.convert && result.IsArray)
            {
                result.ConvertToFloat();
            }
            return result;
        }

        #endregion

        #region Rank params

        private List<int> GetNumberList(AType n, AType left, AType right, Aplus environment)
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

            List<int> number = new List<int>();

            if (length == 1)
            {
                int first = raveledArray[0].asInteger;

                if (first < 0)
                {
                    number.Add(Math.Max(0, left.Rank - Math.Abs(first)));
                    number.Add(Math.Max(0, right.Rank - Math.Abs(first)));
                }
                else
                {
                    number.Add(Math.Min(first, left.Rank));
                    number.Add(Math.Min(first, right.Rank));
                }

                number.Add(9);
            }
            else //length: 2,3
            {
                int first = raveledArray[0].asInteger;
                int second = raveledArray[1].asInteger;

                if (first < 0)
                {
                    number.Add(Math.Max(0, left.Rank - Math.Abs(first)));
                }
                else
                {
                    number.Add(Math.Min(first, left.Rank));
                }

                if (second < 0)
                {
                    number.Add(Math.Max(0, right.Rank - Math.Abs(second)));
                }
                else
                {
                    number.Add(Math.Min(second, right.Rank));
                }

                if (length == 3)
                {
                    int third = raveledArray[2].asInteger;

                    if (third < 0)
                    {
                        throw new Error.Domain("Rank");
                    }

                    number.Add(third);
                }
                else
                {
                    number.Add(9);
                }
            }

            return number;
        }

        #endregion

        #region Algorithm

        private AType Walker(AType left, AType right, List<int> numberList, AFunc function, Aplus environment)
        {
            int tx = Math.Min(numberList[2], right.Rank - numberList[1]);
            int ty = Math.Min(numberList[2], left.Rank - numberList[0]);

            int lx = Math.Max(0, right.Rank - (numberList[1] + tx));
            int ly = Math.Max(0, left.Rank - (numberList[0] + ty));

            AType temp;

            if (ly > 0)
            {
                AType result = AArray.Create(ATypes.ANull);

                foreach (AType item in left)
                {
                    temp = Walker(item, right, numberList, function, environment);
                    TypeCheck(temp.Type);
                    result.AddWithNoUpdate(temp);
                }

                result.UpdateInfo();
                return result;
            }

            if (lx > 0)
            {
                AType result = AArray.Create(ATypes.ANull);

                foreach (AType item in right)
                {
                    temp = Walker(left, item, numberList, function, environment);
                    TypeCheck(temp.Type);
                    result.AddWithNoUpdate(temp);
                }

                result.UpdateInfo();
                return result;
            }

            if (ty != tx)
            {
                AType result = AArray.Create(ATypes.ANull);

                if (ty > tx)
                {
                    //Left array
                    foreach (AType item in left)
                    {
                        temp = Walker(item, right, numberList, function, environment);
                        TypeCheck(temp.Type);
                        result.AddWithNoUpdate(temp);
                    }
                }
                else
                {
                    //Right array
                    foreach (AType item in right)
                    {
                        temp = Walker(left, item, numberList, function, environment);
                        TypeCheck(temp.Type);
                        result.AddWithNoUpdate(temp);
                    }
                }

                result.UpdateInfo();
                return result;
            }

            if (ty > 0)
            {
                List<int> tyShape = left.Shape.GetRange(0, ty);
                List<int> txShape = right.Shape.GetRange(0, tx);

                if (!tyShape.SequenceEqual(txShape))
                {
                    throw new Error.Mismatch("Rank");
                }
            }

            if (ty != 0)
            {
                AType result = AArray.Create(ATypes.ANull);

                for (int i = 0; i < left.Length; i++)
                {
                    temp = Walker(left[i], right[i], numberList, function, environment);
                    TypeCheck(temp.Type);
                    result.AddWithNoUpdate(temp);
                }

                result.UpdateInfo();
                return result;
            }
            else
            {
                var method = (Func<Aplus, AType, AType, AType>)function.Method;
                return method(environment, right, left);
            }
        }

        private void TypeCheck(ATypes type)
        {
            if (!this.convert)
            {
                if (this.check == ATypes.AType)
                {
                    this.check = type;
                }

                if (this.check != type)
                {
                    if (this.check == ATypes.AFloat && type == ATypes.AInteger ||
                        type == ATypes.AFloat && this.check == ATypes.AInteger)
                    {
                        this.convert = true;
                    }
                    else
                    {
                        if (!type.MixedType())
                        {
                            throw new Error.Type("Rank");
                        }
                    }
                }
            }
        }

        #endregion
    }
}