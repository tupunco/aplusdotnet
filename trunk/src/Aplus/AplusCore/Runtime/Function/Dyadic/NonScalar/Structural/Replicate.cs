using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Replicate : AbstractDyadicFunction
    {
        #region Repliacte information

        class ReplicateJobInfo
        {
            private AType items;
            private int[] replicateVector;

            public ReplicateJobInfo(int[] replicateVector, AType items)
            {
                this.replicateVector = replicateVector;
                this.items = items;
            }

            internal AType Items
            {
                get { return items; }
            }

            internal int[] ReplicateVector
            {
                get { return this.replicateVector; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            ReplicateJobInfo replicateInfo = CreateReplicateJobInfo(right, left);
            return Compute(replicateInfo);
        }

        #endregion

        #region Preparation

        private ReplicateJobInfo CreateReplicateJobInfo(AType right, AType left)
        {
            if (!(left.Type == ATypes.AFloat || left.Type == ATypes.AInteger || left.Type == ATypes.ANull))
            {
                throw new Error.Type(TypeErrorText);
            }

            if (left.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            int[] replicateVector;
            AType scalar;

            if (left.TryFirstScalar(out scalar, true))
            {
                replicateVector = new int[] { ExtractInteger(scalar) };
            }
            else
            {
                if (left.Length > 0)
                {
                    replicateVector = left.Select(item => ExtractInteger(item)).ToArray();
                }
                else
                {
                    replicateVector = new int[] { 0 };
                }

                // lenght check should be the first than parse the left side,
                // but the A+ follow that order.
                if (right.Length != 1 && left.Length != right.Length)
                {
                    throw new Error.Length(LengthErrorText);
                }
            }

            ReplicateJobInfo info = new ReplicateJobInfo(
                replicateVector,
                right.IsArray ? right : AArray.Create(right.Type, right)
            );

            return info;
        }

        private int ExtractInteger(AType item)
        {
            int element;

            if (!item.ConvertToRestrictedWholeNumber(out element))
            {
                throw new Error.Type(TypeErrorText);
            }

            if (element < 0)
            {
                throw new Error.Domain(DomainErrorText);
            }

            return element;
        }

        #endregion

        #region Computation

        private static AType Compute(ReplicateJobInfo replicateInfo)
        {
            AType result = AArray.Create(ATypes.AArray);
            int length = 0;

            if (replicateInfo.ReplicateVector.Length == 1)
            {
                for (int i = 0; i < replicateInfo.Items.Length; i++)
                {
                    for (int j = 0; j < replicateInfo.ReplicateVector[0]; j++)
                    {
                        result.AddWithNoUpdate(replicateInfo.Items[i].Clone());
                    }
                }

                length = replicateInfo.ReplicateVector[0] * replicateInfo.Items.Length;
            }
            else //replicateCounter.Count > 1
            {
                for (int i = 0; i < replicateInfo.ReplicateVector.Length; i++)
                {
                    for (int j = 0; j < replicateInfo.ReplicateVector[i]; j++)
                    {
                        result.AddWithNoUpdate(replicateInfo.Items[replicateInfo.Items.Length > 1 ? i : 0].Clone());
                    }

                    length += replicateInfo.ReplicateVector[i];
                }
            }

            result.Length = length;
            result.Shape = new List<int>() { length };

            if (replicateInfo.Items.Rank > 1)
            {
                result.Shape.AddRange(replicateInfo.Items.Shape.GetRange(1, replicateInfo.Items.Shape.Count - 1));
            }

            result.Rank = replicateInfo.Items.Rank;
            result.Type = 
                length > 0 ? result[0].Type : (replicateInfo.Items.MixedType() ? ATypes.ANull : replicateInfo.Items.Type);

            return result;
        }

        #endregion
    }
}
