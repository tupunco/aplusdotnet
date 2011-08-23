using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Comprasion
{
    class GradeUp : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, Aplus environment = null)
        {
            // Scalar can not be sorted
            if (argument.Rank < 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            // Likewise box array
            if (argument.IsBox)
            {
                throw new Error.Type(TypeErrorText);
            }

            return argument.InsertionSortIndex((a, b) => { return a.CompareTo(b); });
        }

        #endregion
    }
}
