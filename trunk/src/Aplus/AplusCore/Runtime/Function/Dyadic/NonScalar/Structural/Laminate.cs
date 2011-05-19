using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Laminate : AbstractDyadicFunction
    {
        #region Variables

        private AType y;
        private AType x;
        private ATypes resultType;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            TypeCheckAndConvert(right, left);
            return Compute();
        }

        #endregion

        #region Preparation

        private void TypeCheckAndConvert(AType right, AType left)
        {
            AType temp_x = right;
            AType temp_y = left;

            if (temp_x.Type != temp_y.Type)
            {
                if (temp_y.Type == ATypes.AFloat && temp_x.Type == ATypes.AInteger)
                {
                    if (temp_x.IsArray)
                    {
                        temp_x.ConvertToFloat();
                    }
                    else
                    {
                        temp_x = AFloat.Create(temp_x.asFloat);
                    }
                }
                else if (temp_y.Type == ATypes.AInteger && temp_x.Type == ATypes.AFloat)
                {
                    if (temp_y.IsArray)
                    {
                        temp_y.ConvertToFloat();
                    }
                    else
                    {
                        temp_y = AFloat.Create(temp_y.asFloat);
                    }
                }
                else
                {
                    if (!Utils.IsSameGeneralType(left, right) && !Util.TypeCorrect(right.Type, left.Type, "N?", "?N"))
                    {
                        throw new Error.Type(TypeErrorText);
                    }
                }
            }

            resultType = temp_y.Length != 0 ? temp_y.Type : temp_x.Type;

            PrepareVariables(temp_x, temp_y);
        }

        /// <summary>
        /// Set x,y attributes, if x or y is scalar, we reshape it to the other one`s shape.
        /// At the end Rank, and Length error check.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        private void PrepareVariables(AType right, AType left)
        {
            if(left.IsArray && !right.IsArray)
            {
                this.y = left.Clone();
                this.x = DyadicFunctionInstance.Reshape.Execute(right, this.y.Shape.ToAArray());
            }
            else if (!left.IsArray && right.IsArray)
            {
                this.x = right.Clone();
                this.y = DyadicFunctionInstance.Reshape.Execute(left, this.x.Shape.ToAArray());
            }
            else
            {
                this.x = right.Clone();
                this.y = left.Clone();
            }

            if (this.y.Rank != this.x.Rank)
            {
                throw new Error.Rank(RankErrorText);
            }

            if (!this.y.Shape.SequenceEqual(this.x.Shape))
            {
                throw new Error.Length(LengthErrorText);
            }

            if (this.y.Rank >= 9 || this.x.Rank >= 9)
            {
                throw new Error.MaxRank(MaxRankErrorText);
            }
        }

        #endregion

        #region Computation

        /// <summary>
        /// Compose left and right side.
        /// </summary>
        /// <returns></returns>
        private AType Compute()
        {
            AType result = AArray.Create(ATypes.AArray);

            if (this.y.Length == 0 || this.x.Length == 0)
            {
                result.Type = this.y.Type = this.x.Type = resultType.MixedType() ? ATypes.ANull : this.resultType;
            }
            else
            {
                result.Type = this.resultType;
            }

            result.AddWithNoUpdate(this.y);
            result.AddWithNoUpdate(this.x);

            result.Length = 2;
            result.Shape = new List<int>() { 2 };

            if (this.x.Rank >= 1)
            {
                result.Shape.AddRange(this.x.Shape);
            }

            result.Rank = 1 + this.x.Rank;

            return result;
        }

        #endregion
    }
}
