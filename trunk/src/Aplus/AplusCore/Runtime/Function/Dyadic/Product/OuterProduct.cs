using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.Product
{
    /// <summary>
    /// Skeleton for Outer Product calculation.
    /// </summary>
    /// <remarks>
    /// Uses the definition written in language reference:
    /// y f. g  ==  y (f@ 0 0 0) x
    /// </remarks>
    abstract class OuterProduct : AbstractDyadicFunction
    {
        #region Null Type information

        protected virtual ATypes NullType { get { return ATypes.AInteger; } }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            AType result = Walk(left, right, environment);

            if (result.Length == 0)
            {
                result.Type = this.NullType;
                result.Shape = new List<int>(left.Shape);
                result.Shape.AddRange(right.Shape);
            }

            return result;
        }

        #endregion

        #region Item selector

        private AType Walk(AType left, AType right, AplusEnvironment env)
        {
            AType result;
            if (left.Rank > 0)
            {
                result = AArray.Create(ATypes.AArray);
                foreach (AType a in left)
                {
                    result.Add(Walk(a, right, env));
                }
            }
            else if (right.Rank > 0)
            {
                result = AArray.Create(ATypes.AArray);
                foreach (AType b in right)
                {
                    result.Add(Walk(left, b, env));
                }
            }
            else
            {
                result = Calculate(left, right, env);
            }

            return result;
        }

        #endregion

        #region Perform calculation

        protected abstract AType Calculate(AType left, AType right, AplusEnvironment env);

        #endregion
    }
}
