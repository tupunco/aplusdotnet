using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Comprasion
{
    class PartitionCount : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            byte[] vector = PrepareVector(argument);
            return Compute(argument, vector);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Checks and make 0,1 vector.
        /// </summary>
        /// <param name="argument"></param>
        private byte[] PrepareVector(AType argument)
        {
            // Type check.
            if(!Util.TypeCorrect(argument.Type, 'F','I','N'))
            {
                throw new Error.Type(TypeErrorText);
            }

            // Rank check.
            if (argument.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            // If the argument is scalar then we wrap it to array.
            AType argArray = argument.IsArray ? argument : AArray.Create(argument.Type, argument);
            byte[] vector = new byte[argArray.Length];
            int i = 0;

            foreach (AType item in argArray)
            {
                int number;
                // Check the item if it is a resctricted whole number.
                if (!item.ConvertToRestrictedWholeNumber(out number))
                {
                    throw new Error.Type(TypeErrorText);
                }

                vector[i++] = ((byte)(number > 0 ? 1 : 0));
            }

            // the first argument is zero, raise Domain error.
            if (vector.Length > 0 && vector[0] == 0)
            {
                throw new Error.Domain(DomainErrorText);
            }

            return vector;
        }

        #endregion

        #region Computation

        /// <summary>
        /// Partition count.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Compute(AType argument, byte[] vector)
        {
            AType result = AArray.Create(ATypes.AArray);

            // If argument is () than result is ().
            result.Type = (argument.Type == ATypes.ANull) ? ATypes.ANull : ATypes.AInteger;

            if (vector.Length > 0)
            {
                int length = 1;
                int counter = 0;

                for (int i = 1; i < vector.Length; i++)
                {
                    counter++;

                    if (vector[i] == 1)
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
