using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar.Miscellaneous
{
    [DefaultResult(ATypes.AInteger)]
    class Roll : MonadicScalar
    {

        public override AType ExecutePrimitive(AInteger argument, AplusEnvironment environment)
        {
            int seed = GetSeed(environment);
            return generateRandomNumber(seed, argument.asInteger);
        }

        public override AType ExecutePrimitive(AFloat argument, AplusEnvironment environment)
        {
            int seed = GetSeed(environment);
            int result;
            if (!argument.ConvertToRestrictedWholeNumber(out result))
            {
                throw new Error.Type(TypeErrorText);
            }

            return generateRandomNumber(seed, result);
            
        }

        private AType generateRandomNumber(int seed, int number)
        {
            if (number <= 0)
            {
                throw new Error.Domain(DomainErrorText);
            }

            Random random = (seed != -1) ? new Random(seed) : new Random();
            return AInteger.Create(random.Next(number));
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
    }
}
