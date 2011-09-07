using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Operator.Dyadic
{
    class Rank
    {
        #region Information for rank

        class RankJobInfo
        {
            private bool floatConvert;
            private ATypes check;
            private int[] rankSpecifier;
            private Func<Aplus, AType, AType, AType> function;

            internal RankJobInfo(int[] rankSpecifier, AFunc function)
            {
                this.rankSpecifier = rankSpecifier;
                this.function = (Func<Aplus, AType, AType, AType>)function.Method;
                this.check = ATypes.AType;
                this.floatConvert = false;
            }

            internal int[] RankSpecifier
            {
                get { return this.rankSpecifier; }
            }

            internal Func<Aplus, AType, AType, AType> Method
            {
                get { return this.function; }
            }

            internal ATypes Check
            {
                get { return this.check; }
                set { this.check = value; }
            }

            internal bool FloatConvert
            {
                get { return this.floatConvert; }
                set { this.floatConvert = value; }
            }
        }

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

            int[] rankSpecifier = GetRankSpecifier(n, left, right, environment);
            RankJobInfo rankInfo = new RankJobInfo(rankSpecifier, func);

            AType result = Walker(left, right, environment, rankInfo);

            if (rankInfo.FloatConvert && result.IsArray)
            {
                result.ConvertToFloat();
            }

            return result;
        }

        #endregion

        #region Rank parameters

        private int[] GetRankSpecifier(AType n, AType left, AType right, Aplus environment)
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
            int first = raveledArray[0].asInteger;
            int second = (length > 1) ? raveledArray[1].asInteger : first;
            int third = (length > 2) ? raveledArray[2].asInteger : 9;

            if (third < 0)
            {
                throw new Error.Domain("Rank");
            }

            int[] specifier = new int[] { GetSingleSpecifier(left, first), GetSingleSpecifier(right, second), third };
            return specifier;
        }

        private static int GetSingleSpecifier(AType item, int givenValue)
        {
            int result;

            if (givenValue < 0)
            {
                result = Math.Max(0, item.Rank - Math.Abs(givenValue));
            }
            else
            {
                result = Math.Min(givenValue, item.Rank);
            }

            return result;
        }

        #endregion

        #region Algorithm

        private AType Walker(AType left, AType right, Aplus environment, RankJobInfo rankInfo)
        {
            int tx = Math.Min(rankInfo.RankSpecifier[2], right.Rank - rankInfo.RankSpecifier[1]);
            int ty = Math.Min(rankInfo.RankSpecifier[2], left.Rank - rankInfo.RankSpecifier[0]);

            int lx = Math.Max(0, right.Rank - (rankInfo.RankSpecifier[1] + tx));
            int ly = Math.Max(0, left.Rank - (rankInfo.RankSpecifier[0] + ty));

            AType result;

            if (ly > 0)
            {
                result = LeftSideWalk(left, right, environment, rankInfo);
                result.UpdateInfo();
            }
            else if (lx > 0)
            {
                result = RightSideWalk(left, right, environment, rankInfo);
                result.UpdateInfo();
            }
            else if (ty != tx)
            {
                result = (ty > tx)
                    ? LeftSideWalk(left, right, environment, rankInfo)
                    : RightSideWalk(left, right, environment, rankInfo);

                result.UpdateInfo();
            }
            else
            {
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
                    result = AArray.Create(ATypes.ANull);
                    AType temp;

                    for (int i = 0; i < left.Length; i++)
                    {
                        temp = Walker(left[i], right[i], environment, rankInfo);
                        TypeCheck(temp.Type, rankInfo);
                        result.AddWithNoUpdate(temp);
                    }

                    result.UpdateInfo();
                }
                else
                {
                    result = rankInfo.Method(environment, right, left);
                }
            }

            return result;
        }

        private AType RightSideWalk(AType left, AType right, Aplus environment, RankJobInfo rankInfo)
        {
            AType result = AArray.Create(ATypes.ANull);
            AType temp;

            foreach (AType item in right)
            {
                temp = Walker(left, item, environment, rankInfo);
                TypeCheck(temp.Type, rankInfo);
                result.AddWithNoUpdate(temp);
            }

            return result;
        }

        private AType LeftSideWalk(AType left, AType right, Aplus environment, RankJobInfo rankInfo)
        {
            AType result = AArray.Create(ATypes.ANull);
            AType temp;

            foreach (AType item in left)
            {
                temp = Walker(item, right, environment, rankInfo);
                TypeCheck(temp.Type, rankInfo);
                result.AddWithNoUpdate(temp);
            }

            return result;
        }

        private void TypeCheck(ATypes type, RankJobInfo rankInfo)
        {
            if (!rankInfo.FloatConvert)
            {
                if (rankInfo.Check == ATypes.AType)
                {
                    rankInfo.Check = type;
                }
                else if (rankInfo.Check != type)
                {
                    if (rankInfo.Check == ATypes.AFloat && type == ATypes.AInteger ||
                        type == ATypes.AFloat && rankInfo.Check == ATypes.AInteger)
                    {
                        rankInfo.FloatConvert = true;
                    }
                    else if (!type.MixedType())
                    {
                        throw new Error.Type("Rank");
                    }
                }
            }
        }

        #endregion
    }
}