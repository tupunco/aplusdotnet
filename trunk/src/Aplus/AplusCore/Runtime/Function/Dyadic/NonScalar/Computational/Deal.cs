using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Computational
{
    class Deal : AbstractDyadicFunction
    {
        #region Nested class for shuffling

        /// <summary>
        /// Class stores information for shuffling items.
        /// </summary>
        class ShuffleInfo
        {
            private int itemCount;
            private int maxValue;

            /// <summary>
            /// Initialise a instance of the <see cref="ShuffleInfo"/> class.
            /// </summary>
            /// <param name="itemCount">Number of items to shuffle.</param>
            /// <param name="maxValue">Max value for the shuffling.</param>
            internal ShuffleInfo(int itemCount, int maxValue)
            {
                this.itemCount = itemCount;
                this.maxValue = maxValue;
            }

            /// <summary>
            /// Gets the number of items to shuffle.
            /// </summary>
            internal int ItemCount
            {
                get { return this.itemCount; }
            }

            /// <summary>
            /// Gets the max value for the shuffling.
            /// </summary>
            internal int MaxValue
            {
                get { return this.maxValue; }
            }
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment)
        {
            ShuffleInfo info = ExtractShuffleInfo(left, right);

            return Shuffle(GetSeed(environment), info);
        }

        #endregion

        #region Preparation

        private ShuffleInfo ExtractShuffleInfo(AType left, AType right)
        {
            ShuffleInfo info = 
                new ShuffleInfo(ExtractInteger(left), ExtractInteger(right));

            if (info.ItemCount > info.MaxValue)
            {
                throw new Error.Domain(DomainErrorText);
            }

            return info;
        }

        /// <summary>
        /// Get integer from argument.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private int ExtractInteger(AType argument)
        {
            AType item;
            if (!argument.TryFirstScalar(out item, true))
            {
                throw new Error.Domain(DomainErrorText);
            }

            int number;
            if (!item.ConvertToRestrictedWholeNumber(out number))
            {
                throw new Error.Type(TypeErrorText);
            }

            if (number < 0)
            {
                throw new Error.Domain(DomainErrorText);
            }

            return number;
        }

        // TODO: move this to the SystemVariables
        private int GetSeed(Aplus environment)
        {
            // return and increment the Random Link System Variable
            int seed = environment.SystemVariables["rl"].asInteger + 1;
            environment.SystemVariables["rl"] = AInteger.Create(seed);
            return seed;
        }

        #endregion

        #region Computation

        /// <summary>
        /// Shuffle the items with a given seed.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="info">Information for shuffling.</param>
        /// <remarks>
        /// An implementation of "modified" Durstenfeld's algorithm.
        /// http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        /// </remarks>
        /// <returns></returns>
        private AType Shuffle(int seed, ShuffleInfo info)
        {
            AType result = AArray.Create(ATypes.AInteger);

            int[] randomList = Enumerable.Range(0, info.MaxValue).ToArray();
            Random random = (seed != -1) ? new Random(seed) : new Random();
            int currentIndex;

            for (int i = info.MaxValue - 1; i >= info.MaxValue - info.ItemCount; i--)
            {
                currentIndex = random.Next(i);
                result.AddWithNoUpdate(AInteger.Create(randomList[currentIndex]));
                randomList[currentIndex] = randomList[i];
            }

            result.Length = info.ItemCount;
            result.Shape = new List<int>() { info.ItemCount };
            result.Rank = 1;

            return result;
        }

        #endregion
    }
}
