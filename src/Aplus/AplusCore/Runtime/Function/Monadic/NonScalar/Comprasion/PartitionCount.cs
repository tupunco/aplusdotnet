using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Comprasion
{
    class PartitionCount : AbstractMonadicFunction
    {
        #region Variables

        private List<byte> vector;

        #endregion

        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            PrepareVector(argument);
            return Compute(argument);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Checks and make 0,1 vector.
        /// </summary>
        /// <param name="argument"></param>
        private void PrepareVector(AType argument)
        {
            //Type check.
            if(!Util.TypeCorrect(argument.Type, 'F','I','N'))
            {
                throw new Error.Type(TypeErrorText);
            }

            //Rank check.
            if (argument.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            this.vector = new List<byte>();

            //If the argument is scalar then we wrap it to array.
            AType argArray = argument.IsArray ? argument : AArray.Create(argument.Type, argument);

            foreach (AType item in argArray)
            {
                int number;
                //Check the item is resctricted whole number.
                if (item.ConvertToRestrictedWholeNumber(out number))
                {
                    this.vector.Add((byte)(number > 0 ? 1 : 0));
                }
                else
                {
                    throw new Error.Type(TypeErrorText);
                }
            }

            //the first argument is zero, raise Domain error.
            if (this.vector.Count > 0 && this.vector[0] == 0)
            {
                throw new Error.Domain(DomainErrorText);
            }
        }

        #endregion

        #region Computation

        /// <summary>
        /// Partition count.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Compute(AType argument)
        {
            AType result = AArray.Create(ATypes.AArray);

            //If argument is () than result is ().
            result.Type = argument.Type == ATypes.ANull ? ATypes.ANull : ATypes.AInteger;

            if (this.vector.Count > 0)
            {
                int length = 1, counter = 0;

                for (int i = 1; i < this.vector.Count; i++)
                {
                    counter++;

                    if (this.vector[i] == 1)
                    {
                        length++;
                        result.AddWithNoUpdate(AInteger.Create(counter));
                        counter = 0;
                    }
                }

                counter++;
                result.AddWithNoUpdate(AInteger.Create(counter));

                result.Length = length;
                result.Shape = new List<int>() { length };
                result.Rank = 1;
            }

            return result;
        }

        #endregion
    }
}
