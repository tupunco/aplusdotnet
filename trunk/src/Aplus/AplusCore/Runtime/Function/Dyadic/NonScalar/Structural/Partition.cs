using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Partition : AbstractDyadicFunction
    {
        #region Partition information

        class PartitionJobInfo
        {
            private int[] partitionVector;
            private int remainder;

            public PartitionJobInfo(int remainder, int[] partitionVector)
            {
                this.remainder = remainder;
                this.partitionVector = partitionVector;
            }

            /// <summary>
            /// Gets the number of remainder items.
            /// </summary>
            internal int Remainder
            {
                get { return this.remainder; }
            }

            /// <summary>
            /// Gets the partition counts
            /// </summary>
            internal int[] PartitionVector
            {
                get { return this.partitionVector; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            PartitionJobInfo arguments = CreatePartitionInfo(right, left);
            return Compute(right, arguments);
        }

        #endregion

        #region Preparation

        private PartitionJobInfo CreatePartitionInfo(AType right, AType left)
        {
            // left side type must be float, integer or null
            if (left.Type != ATypes.AFloat && left.Type != ATypes.AInteger && left.Type != ATypes.ANull || left.IsBox)
            {
                throw new Error.Type(TypeErrorText);
            }

            // if the right side is scalar then we throw Rank error exception
            if (right.Rank < 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            int element;
            List<int> partitionVector = new List<int>();
            int remainder = 0;
            int sum = 0;
            bool going = true;

            // TODO: check if this branch can be eliminated if the argument is raveled always
            if (left.IsArray)
            {
                // TODO: Check this:
                // if the left rank is higher than 1 then we ravel it to vector.
                AType raveled_y = left.Rank > 1 ? MonadicFunctionInstance.Ravel.Execute(left) : left;

                // get the integer list from the right side
                foreach (AType item in raveled_y)
                {
                    // if the actual item is float then we convert it to (if it's a restricted whole number)
                    if (!item.ConvertToRestrictedWholeNumber(out element))
                    {
                        throw new Error.Type(TypeErrorText);
                    }

                    if (element < 0)
                    {
                        // negative item raise domain error
                        throw new Error.Domain(DomainErrorText);
                    }

                    sum += element;

                    if (right.Length > 0 && going)
                    {
                        // compute how many item we can take from the list at last
                        if (sum > right.Length)
                        {
                            partitionVector.Add(right.Length - (sum - element));
                            going = false;
                        }
                        else
                        {
                            // collect the item to y list
                            partitionVector.Add(element);

                            if (sum == right.Length)
                            {
                                going = false;
                            }
                        }
                    }
                    else
                    {
                        // count the empty elements what we need
                        remainder++;
                    }
                }
            }
            else
            {
                // this case is when the left side is scalar
                if (!left.ConvertToRestrictedWholeNumber(out element))
                {
                    throw new Error.Type(TypeErrorText);
                }

                if (element < 0)
                {
                    throw new Error.Domain(DomainErrorText);
                }
                else if (element != 0)
                {
                    int round = right.Length / element;

                    for (int i = 0; i < round; i++)
                    {
                        partitionVector.Add(element);
                    }

                    int rem = right.Length % element;

                    if (rem != 0)
                    {
                        partitionVector.Add(rem);
                    }
                }
                else
                {
                    partitionVector.Add(0);
                }
            }

            PartitionJobInfo info = new PartitionJobInfo(remainder, partitionVector.ToArray());
            return info;
        }

        #endregion

        #region Computation

        private AType Compute(AType inputItems, PartitionJobInfo info)
        {
            // the left side is equal with 0, then the result is Enclosed null
            if (info.PartitionVector.Length == 1 && info.PartitionVector[0] == 0)
            {
                return CreateEnclosedNull(inputItems);
            }

            // the result is a nested vector
            AType result = AArray.Create(ATypes.AType);
            int counter = 0;

            // enclose y[i] items from x array
            //for (int i = 0; i < arguments.PartitionVector.Length; i++)
            foreach (int partitionNumber in info.PartitionVector)
            {
                AType item = AArray.Create(inputItems[counter].Type);

                for (int j = 0; j < partitionNumber; j++)
                {
                    item.AddWithNoUpdate(inputItems[counter]);
                    counter++;
                }

                item.Length = partitionNumber;
                item.Shape = new List<int>() { partitionNumber };
                if (inputItems.Rank > 1)
                {
                    item.Shape.AddRange(inputItems.Shape.GetRange(1, inputItems.Shape.Count - 1));
                }

                item.Rank = inputItems.Rank;

                result.AddWithNoUpdate(ABox.Create(item));
            }

            // add the remainder Enclosed null
            for (int i = 0; i < info.Remainder; i++)
            {
                result.AddWithNoUpdate(CreateEnclosedNull(inputItems));
            }

            result.Length = info.PartitionVector.Length + info.Remainder;
            result.Shape = new List<int>() { result.Length };
            result.Rank = 1;

            result.Type = result.Length > 0 ? ATypes.ABox : ATypes.ANull;

            return result;
        }

        private static AType CreateEnclosedNull(AType inputItem)
        {
            AType result = AArray.Create(inputItem.MixedType() ? ATypes.ANull : inputItem.Type);

            result.Length = 0;
            result.Shape = new List<int>() { result.Length };

            if (inputItem.Rank > 1)
            {
                result.Shape.AddRange(inputItem.Shape.GetRange(1, inputItem.Shape.Count - 1));
            }

            result.Rank = inputItem.Rank;

            return ABox.Create(result);
        }

        #endregion
    }
}
