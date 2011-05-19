using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Comparison
{
    class Member : AbstractDyadicFunction
    {
        /// <summary>
        /// Stores the resulting AArray
        /// </summary>
        private AType result;

        /// <summary>
        /// Stores the AArray where we need to perform the search
        /// </summary>
        private AType searchWhere;

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            if (right.Type == ATypes.ANull)
            {
                return DyadicFunctionInstance.Reshape.Execute(AInteger.Create(0), left.Shape.ToAArray());
            }
            else if (!(left.IsNumber && right.IsNumber) && (right.Type != left.Type))
            {
                throw new Error.Type(this.TypeErrorText);
            }
            else if (left.Type == ATypes.ANull)
            {
                return Utils.ANull();
            }

            // Convert the right argument to an array (make life easier..)
            this.searchWhere = (right.IsArray) ? right : AArray.Create(right.Type, right);

            if (left.Rank < this.searchWhere[0].Rank)
            {
                // The item we are looking for have a lesser rank than the items of the right argument
                throw new Error.Rank(this.RankErrorText);
            }

            if (!left.IsArray)
            {
                // Process a simple scalar 
                return ProcessScalar(left);
            }

            this.result = AArray.Create(ATypes.AInteger);
            AType searchWhat = left;
            
            ProcessMatrix(searchWhat);

            this.result.UpdateInfo();

            // Check how many elements we need to return
            int elementCount = left.Shape.Count - Math.Max(right.Rank - 1, 0);
            AType scalar;
            if ((elementCount == 0) && this.result.TryFirstScalar(out scalar))
            {
                // need to return a simple scalar
                return scalar;
            }

            // Reshape the result to the required shape
            List<int> desiredShape = left.Shape.GetRange(0, elementCount);
            return DyadicFunctionInstance.Reshape.Execute(this.result, desiredShape.ToAArray());
        }


        private void ProcessMatrix(AType searchWhat)
        {
            if (searchWhat.Shape.Count > this.searchWhere[0].Shape.Count)
            {
                // Travers down to the correct cells
                foreach (AType item in searchWhat)
                {
                    if (item.IsArray)
                    {
                        // Found an array, lets see if we can process it further
                        ProcessMatrix(item);
                    }
                    else
                    {
                        // Simple Scalar found
                        this.result.AddWithNoUpdate(ProcessScalar(item));
                    }
                }
            }
            else if (searchWhat.Shape.SequenceEqual<int>(this.searchWhere[0].Shape))
            {
                // Found the cell we were looking for
                int value = 0;

                foreach (AType item in this.searchWhere)
                {
                    if (searchWhat.EqualsWithTolerance(item))
                    {
                        value = 1;
                        // If we found one true result, no need to check further
                        break;
                    }
                }
                this.result.AddWithNoUpdate(AInteger.Create(value));
            }
            else
            {
                throw new Error.Length(this.LengthErrorText);
            }
        }

        private AType ProcessScalar(AType searchWhat)
        {
            int value = 0;
            foreach (AType item in this.searchWhere)
            {
                // Check if the item is equals to the search item
                //  in case of number use comparisonTolerance
                //  otherwise a simple ==
                if ((item.IsNumber && Utils.ComparisonTolerance(item.asFloat, searchWhat.asFloat)) ||
                    (item.Equals(searchWhat)) )
                {
                    value = 1;
                    // If we found one true result, no need to check further
                    break;
                }
            }

            return AInteger.Create(value);
        }

    }
}
