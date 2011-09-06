using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Comparison
{
    class Member : AbstractDyadicFunction
    {
        #region Nested class for search information handling

        /// <summary>
        /// Class to store information about the search.
        /// </summary>
        class SearchInfo
        {
            private AType searchWhere;
            private AType result;
            private string errorText;

            internal AType SearchWhere
            {
                get { return this.searchWhere; }
                set { this.searchWhere = value; }
            }

            internal AType Result
            {
                get { return this.result; }
                set { this.result = value; }
            }

            internal string ErrorText
            {
                get { return this.errorText; }
                set { this.errorText = value; }
            }
        }

        #endregion

        #region DLR entry point

        /// <summary>
        /// Search the <see cref="left"/> argument in the <see cref="right"/> argument.
        /// </summary>
        /// <param name="right">The data where the search should be performed.</param>
        /// <param name="left">Element to search in the right argument.</param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            if (right.Length == 0)
            {
                switch (left.Rank)
                {
                    case 0:
                        return AInteger.Create(0);
                    default:
                        return DyadicFunctionInstance.Reshape.Execute(AInteger.Create(0), left.Shape.ToAArray());
                }
            }
            else if (!(left.IsNumber && right.IsNumber) && (right.Type != left.Type))
            {
                throw new Error.Type(this.TypeErrorText);
            }

            SearchInfo searchInfo = new SearchInfo()
            {
                // convert the right argument to an array (make life easier..)
                SearchWhere = right.IsArray ? right : AArray.Create(right.Type, right),
                ErrorText = this.LengthErrorText
            };

            if (left.Rank < searchInfo.SearchWhere[0].Rank)
            {
                // The item we are looking for have a lesser rank than the items of the right argument
                throw new Error.Rank(this.RankErrorText);
            }

            AType result;

            if (!left.IsArray)
            {
                // Process a simple scalar 
                result = ProcessScalar(left, searchInfo);
            }
            else
            {
                searchInfo.Result = AArray.Create(ATypes.AInteger);

                ProcessMatrix(left, searchInfo);

                searchInfo.Result.UpdateInfo();

                // Check how many elements we need to return
                int elementCount = left.Shape.Count - Math.Max(right.Rank - 1, 0);

                if ((elementCount == 0) && searchInfo.Result.TryFirstScalar(out result))
                {
                    // need to return a simple scalar
                    // 'result' contains the desired value
                }
                else
                {
                    // reshape the result to the required shape
                    List<int> desiredShape = left.Shape.GetRange(0, elementCount);
                    result = DyadicFunctionInstance.Reshape.Execute(searchInfo.Result, desiredShape.ToAArray());
                }
            }

            return result;
        }

        #endregion

        #region Utility

        private static void ProcessMatrix(AType searchWhat, SearchInfo searchInfo)
        {
            if (searchWhat.Shape.Count > searchInfo.SearchWhere[0].Shape.Count)
            {
                // Travers down to the correct cells
                foreach (AType item in searchWhat)
                {
                    if (item.IsArray)
                    {
                        // Found an array, lets see if we can process it further
                        ProcessMatrix(item, searchInfo);
                    }
                    else
                    {
                        // Simple Scalar found
                        searchInfo.Result.AddWithNoUpdate(ProcessScalar(item, searchInfo));
                    }
                }
            }
            else if (searchWhat.Shape.SequenceEqual<int>(searchInfo.SearchWhere[0].Shape))
            {
                // search for the cell
                int found = 
                    searchInfo.SearchWhere.Any(item => searchWhat.EqualsWithTolerance(item)) ? 1 : 0;

                searchInfo.Result.AddWithNoUpdate(AInteger.Create(found));
            }
            else
            {
                throw new Error.Length(searchInfo.ErrorText);
            }
        }

        private static AType ProcessScalar(AType searchWhat, SearchInfo searchInfo)
        {
            int found = 
                searchInfo.SearchWhere.Any(item =>
                    (item.IsNumber && Utils.ComparisonTolerance(item.asFloat, searchWhat.asFloat)) ||
                    item.Equals(searchWhat)) ? 1 : 0;

            return AInteger.Create(found);
        }

        #endregion
    }
}
