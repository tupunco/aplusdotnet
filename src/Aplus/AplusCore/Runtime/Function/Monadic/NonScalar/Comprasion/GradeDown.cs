using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Comprasion
{
    class GradeDown : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            //If the is scalar, it can't be sort.
            if (argument.Rank >= 1)
            {
                //Likewise box array...
                if(argument.IsBox)
                {
                    throw new Error.Type(TypeErrorText);
                }

                return argument.InsertionSortIndex((a, b) => { return -a.CompareTo(b); });
            }
            else
            {
                throw new Error.Rank(RankErrorText);
            }
        }

        #endregion

        #region Comparison

        private static int GradeDownComparison(AType x, AType y)
        {
            return - x.CompareTo(y);
        }

        #endregion
    }
}
