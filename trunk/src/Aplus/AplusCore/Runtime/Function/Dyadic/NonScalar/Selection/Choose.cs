using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Selection
{
    class Choose : AbstractDyadicFunction
    {
        #region Entry point

        /// <summary>
        /// Reconduct Choose to Bracket indexing.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            //If the left side is Null then we clone the right side.
            if (left.Type == ATypes.ANull)
            {
                return right;
            }

            return right[GetIndexes(left, right)];
        }

        #endregion

        #region Indexing

        private List<AType> GetIndexes(AType left, AType right)
        {
            List<AType> indexes = new List<AType>();

            if (left.IsBox)
            {
                //Ravel left side box array, if rank > 1.
                AType raveled = left.Rank > 1 ? MonadicFunctionInstance.Ravel.Execute(left) : left;

                //length of the left argument greater than the rank of the right argument, raise Rank error.
                if (raveled.Length > right.Rank)
                {
                    throw new Error.Rank(RankErrorText);
                }

                //Nested case: x[0 pick y; 1 pick y; ...; (-1 + #y) pick y]
                for (int i = 0; i < raveled.Length; i++)
                {
                    indexes.Add(DyadicFunctionInstance.Pick.Execute(raveled, AInteger.Create(i)));
                }
            }
            else
            {
                //Simple case: x[y;...;]
                indexes.Add(left);
            }

            return indexes;
        }

        #endregion

        #region Assign

        public AType Assign(AType right, AType left, AplusEnvironment environment)
        {
            //If the left side is Null then we clone the right side.
            if (left.Type == ATypes.ANull)
            {
                return right;
            }

            return right.Indexing(GetIndexes(left,right),0, true, right.IsMemoryMappedFile);
        }

        #endregion
    }
}
