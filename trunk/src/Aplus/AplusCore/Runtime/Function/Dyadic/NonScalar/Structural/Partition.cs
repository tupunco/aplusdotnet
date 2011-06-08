using System.Collections.Generic;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class Partition : AbstractDyadicFunction
    {
        #region Variables

        private List<int> y;
        private AType x;
        private int remainder;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            PrepareVariables(right, left);
            return Compute();
        }

        #endregion

        #region Preparation

        private void PrepareVariables(AType right, AType left)
        {
            int sum = 0;
            bool going = true;

            this.remainder = 0;

            // Left side type must be float, integer or null.
            if (left.Type != ATypes.AFloat && left.Type != ATypes.AInteger && left.Type != ATypes.ANull || left.IsBox)
            {
                throw new Error.Type(TypeErrorText);
            }

            //If the right side is scalar then we throw Rank error exception.
            if (right.Rank < 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            this.x = right;

            this.y = new List<int>();
            int element;

            if (left.IsArray)
            {
                //If the left rank is higher than 1 then we ravel it to vector.
                AType raveled_y = left.Rank > 1 ? MonadicFunctionInstance.Ravel.Execute(left) : left;

                //Get the integer list from the right side.
                foreach (AType item in raveled_y)
                {
                    //If the actual item is float then we convert it to (if it's a restricted whole number).
                    if (item.ConvertToRestrictedWholeNumber(out element))
                    {
                        //Negative item raise domain error.
                        if (element < 0)
                        {
                            throw new Error.Domain(DomainErrorText);
                        }

                        sum += element;

                        if (right.Length > 0 && going)
                        {
                            //Compute how many item we can take from the list at last.
                            if (sum > right.Length)
                            {
                                this.y.Add(right.Length - (sum - element));
                                going = false;
                            }
                            else
                            {
                                //Collect the item to y list.
                                this.y.Add(element);

                                if (sum == right.Length)
                                {
                                    going = false;
                                }
                            }
                        }
                        else
                        {
                            //Count the empty elements what we need.
                            this.remainder++;
                        }
                    }
                    else
                    {
                        throw new Error.Type(TypeErrorText);
                    }
                }
            }
            else
            {
                //This case is when the left side is scalar.
                if (left.ConvertToRestrictedWholeNumber(out element))
                {
                    if (element < 0)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                }
                else
                {
                    throw new Error.Type(TypeErrorText);
                }

                if (element != 0)
                {
                    int round = right.Length / element;

                    for (int i = 0; i < round; i++)
                    {
                        this.y.Add(element);
                    }

                    int rem = right.Length % element;

                    if (rem != 0)
                    {
                        this.y.Add(rem);
                    }
                }
                else
                {
                    this.y.Add(0);
                }
            }
        }

        #endregion

        #region Computation

        private AType Compute()
        {
            //The left side is equal with 0, then the result is Enclosed null.
            if (this.y.Count == 1 && this.y[0] == 0)
            {  
                return CreateEnclosedNull();
            }

            //The result is a nested vector.
            AType result = AArray.Create(ATypes.AType);
            AType item;
            int counter = 0;

            //Enclose y[i] items from x array.
            for (int i = 0; i < this.y.Count; i++)
            {
                item = AArray.Create(this.x[counter].Type);

                for (int j = 0; j < this.y[i]; j++)
                {
                    item.AddWithNoUpdate(this.x[counter]);
                    counter++;
                }

                item.Length = this.y[i];
                item.Shape = new List<int>() { this.y[i] };
                if (this.x.Rank > 1)
                {
                    item.Shape.AddRange(this.x.Shape.GetRange(1, this.x.Shape.Count - 1));
                }

                item.Rank = this.x.Rank;

                result.AddWithNoUpdate(ABox.Create(item));
            }

            //Add the remainder Enclosed null.
            if (this.remainder != 0)
            {
                for (int i = 0; i < this.remainder; i++)
                {
                    result.AddWithNoUpdate(CreateEnclosedNull());
                }
            }

            result.Length = this.y.Count + this.remainder;
            result.Shape = new List<int>() { result.Length };
            result.Rank = 1;

            result.Type = result.Length > 0 ? ATypes.ABox : ATypes.ANull;

            return result;
        }

        private AType CreateEnclosedNull()
        {
            AType result = AArray.Create(this.x.MixedType() ? ATypes.ANull : this.x.Type);

            result.Length = 0;
            result.Shape = new List<int>() { result.Length };

            if (this.x.Rank > 1)
            {
                result.Shape.AddRange(this.x.Shape.GetRange(1, this.x.Shape.Count - 1));
            }

            result.Rank = this.x.Rank;

            return ABox.Create(result);
        }

        #endregion
    }
}
