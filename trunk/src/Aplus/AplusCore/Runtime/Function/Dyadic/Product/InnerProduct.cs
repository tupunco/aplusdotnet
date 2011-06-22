using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    /// <summary>
    /// Skeleton for Inner Product calculation.
    /// </summary>
    /// <remarks>
    /// Uses the definition written in language reference:
    /// y f.g x  ==  y (pdt @ 1 1 0) (-1 rot iota rho rho x) flip x
    /// where pdt:
    /// pdt{a;b} : f/ a g b
    /// </remarks>
    abstract class InnerProduct : AbstractDyadicFunction
    {
        #region Entry Point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            if (left.Rank < 1 || right.Rank < 1)
            {
                throw new Error.Rank(this.RankErrorText);
            }

            if (left.Shape[left.Shape.Count - 1] != right.Shape[0])
            {
                throw new Error.Length(this.LengthErrorText);
            }

            // Calculate the axes for the right argument: (-1 rot iota rho rho right)
            AType targetAxes = DyadicFunctionInstance.Rotate.Execute(
                Enumerable.Range(0, right.Rank).ToAArray(), AInteger.Create(-1), environment);

            AType transposedRight = DyadicFunctionInstance.TransposeAxis.Execute(right, targetAxes, environment);

            AType result = WalkItems(left, transposedRight, environment);

            // by observation it seems that the reference implementation always returns float
            // we behave the same
            result.ConvertToFloat();

            if (result.Length == 0)
            {
                result.Shape = new List<int>(left.Shape.GetRange(0, left.Shape.Count - 1));
                if (right.Shape.Count > 1)
                {
                    result.Shape.AddRange(right.Shape.GetRange(1, right.Shape.Count - 1));
                }
            }
            
            return result;
        }

        #endregion

        #region calculation method

        protected abstract AType Calculate(AType left, AType right, AplusEnvironment env);

        #endregion

        #region Internal methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="env"></param>
        private AType WalkItems(AType left, AType right, AplusEnvironment env)
        {
            AType result;
            if (left.Rank > 1)
            {
                result = AArray.Create(ATypes.AArray);
                foreach (AType a in left)
                {
                    result.Add(WalkItems(a, right, env));
                }
            }
            else if (right.Rank > 1)
            {
                result = AArray.Create(ATypes.AArray);
                foreach (AType b in right)
                {
                    result.Add(WalkItems(left, b, env));
                }
            }
            else
            {
                result = Calculate(left, right, env);
            }

            return result;
        }
        
        #endregion
    }
}
