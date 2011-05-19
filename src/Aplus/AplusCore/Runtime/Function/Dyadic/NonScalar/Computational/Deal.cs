using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Computational
{
    class Deal : AbstractDyadicFunction
    {
        #region Variables

        private int y;
        private int x;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            PrepareVariables(left, right);

            return Schuffle(GetSeed(environment));
        }

        #endregion

        #region Preparation

        private void PrepareVariables(AType left, AType right)
        {
            this.y = GetNumber(left);
            this.x = GetNumber(right);

            if (this.y > this.x)
            {
                throw new Error.Domain(DomainErrorText);
            }
        }

        /// <summary>
        /// Get integer from argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private int GetNumber(AType argument)
        {
            AType item;
            if (argument.TryFirstScalar(out item, true))
            {
                int number;
                if (item.ConvertToRestrictedWholeNumber(out number))
                {
                    if (number < 0)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                    return number;
                }
                else
                {
                    throw new Error.Type(TypeErrorText);
                }
            }
            else
            {
                throw new Error.Domain(DomainErrorText);
            }
        }


        private int GetSeed(AplusEnvironment environment)
        {
            if (environment == null)
            {
                return -1;
            }

            // Return and increment the Random Link System Variable
            int seed = environment.Runtime.SystemVariables["rl"].asInteger + 1;
            environment.Runtime.SystemVariables["rl"] = AInteger.Create(seed);
            return seed;
        }

        #endregion

        #region Computation

        /// <summary>
        /// An implementation of "modified" Durstenfeld's algorithm.
        /// http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        private AType Schuffle(int seed)
        {
            AType result = AArray.Create(ATypes.AInteger);
            
            int[] randomList = Enumerable.Range(0, this.x).ToArray();
            Random rnd = (seed != -1) ? new Random(seed) : new Random();
            int j;

            for (int i = x - 1; i >= this.x - this.y; i--)
            {
                j = rnd.Next(i);
                result.AddWithNoUpdate(AInteger.Create(randomList[j]));
                randomList[j] = randomList[i];
            }

            result.Length = this.y;
            result.Shape = new List<int>() { this.y };
            result.Rank = 1;

            return result;
        }

        #endregion
    }
}
