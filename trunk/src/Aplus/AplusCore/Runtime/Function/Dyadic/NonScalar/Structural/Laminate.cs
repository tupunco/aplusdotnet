using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Laminate : AbstractDyadicFunction
    {
        #region Laminate job info

        class LaminateJobInfo
        {
            private AType left;
            private AType right;
            private ATypes resultType;

            public LaminateJobInfo(ATypes resultType)
            {
                this.resultType = resultType;
            }

            internal AType Left
            {
                get { return this.left; }
                set { this.left = value; }
            }

            internal AType Right
            {
                get { return this.right; }
                set { this.right = value; }
            }

            internal ATypes ResultType
            {
                get { return this.resultType; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            LaminateJobInfo arguments = CreateLaminateJob(right, left);

            return Compute(arguments);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        private LaminateJobInfo CreateLaminateJob(AType right, AType left)
        {
            LaminateJobInfo laminateInfo = 
                new LaminateJobInfo(left.Length != 0 ? left.Type : right.Type);
            
            if (left.IsArray && !right.IsArray)
            {
                laminateInfo.Left = left.Clone();
                laminateInfo.Right = DyadicFunctionInstance.Reshape.Execute(right, left.Shape.ToAArray());
            }
            else if (!left.IsArray && right.IsArray)
            {
                laminateInfo.Right = right.Clone();
                laminateInfo.Left = DyadicFunctionInstance.Reshape.Execute(left, right.Shape.ToAArray());
            }
            else
            {
                laminateInfo.Right = right.Clone();
                laminateInfo.Left = left.Clone();
            }

            // check if the types are same

            if (right.Type != left.Type)
            {
                if (left.Type == ATypes.AFloat && right.Type == ATypes.AInteger)
                {
                    right = right.ConvertToFloat();
                }
                else if (left.Type == ATypes.AInteger && right.Type == ATypes.AFloat)
                {
                    left = left.ConvertToFloat();
                }
                else if (!Utils.IsSameGeneralType(left, right) && !Util.TypeCorrect(right.Type, left.Type, "N?", "?N"))
                {
                    throw new Error.Type(TypeErrorText);
                }
            }

            // check the length, shape and rank
            if (laminateInfo.Left.Rank != laminateInfo.Right.Rank)
            {
                throw new Error.Rank(RankErrorText);
            }

            if (!laminateInfo.Left.Shape.SequenceEqual(laminateInfo.Right.Shape))
            {
                throw new Error.Length(LengthErrorText);
            }

            if (laminateInfo.Left.Rank >= 9 || laminateInfo.Right.Rank >= 9)
            {
                throw new Error.MaxRank(MaxRankErrorText);
            }

            return laminateInfo;
        }

        #endregion

        #region Computation

        /// <summary>
        /// Compose left and right side.
        /// </summary>
        /// <param name="laminateInfo">Instead of class variables.</param>
        /// <returns></returns>
        private AType Compute(LaminateJobInfo laminateInfo)
        {
            AType result = AArray.Create(laminateInfo.ResultType);

            if (laminateInfo.Left.Length == 0 || laminateInfo.Right.Length == 0)
            {
                result.Type = laminateInfo.Left.Type = laminateInfo.Right.Type = 
                    laminateInfo.ResultType.MixedType() ? ATypes.ANull : laminateInfo.ResultType;
            }

            result.AddWithNoUpdate(laminateInfo.Left);
            result.AddWithNoUpdate(laminateInfo.Right);

            result.Length = 2;
            result.Shape = new List<int>() { 2 };

            if (laminateInfo.Right.Rank >= 1)
            {
                result.Shape.AddRange(laminateInfo.Right.Shape);
            }

            result.Rank = 1 + laminateInfo.Right.Rank;

            return result;
        }

        #endregion
    }
}
