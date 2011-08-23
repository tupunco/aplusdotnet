using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Interval : AbstractMonadicFunction
    {

        private AType GenerateVector(int itemCount)
        {
            AType vector = AArray.Create(ATypes.AInteger);

            for (int i = 0; i < itemCount; i++)
            {
                vector.AddWithNoUpdate(AInteger.Create(i));
            }
            vector.UpdateInfo();
            return vector;
        }

        private AType GenerateMultiDimension(AType arguments)
        {
            int itemCount = 1;
            int number;
            for (int i = 0; i < arguments.Length; i++)
            {
                // Try to convert number to integer
                if (!Utils.TryComprasionTolarence(arguments[i].asFloat, out number))
                {
                    // Failed to convert to integer
                    throw new Error.Type(this.TypeErrorText);
                }

                if (number < 0)
                {
                    // Negative numbers are invalid
                    throw new Error.Domain(this.DomainErrorText);
                }

                // Count how many numbers we need to generate
                itemCount *= number;
            }

            // Generate the numbers
            AType range = GenerateVector(itemCount);

            // Restructure to correct shape
            return Function.Dyadic.DyadicFunctionInstance.Reshape.Execute(range, arguments);
        }

        public override AType Execute(AType argument, Aplus environment = null)
        {
            if (argument.Rank > 1)
            {
                throw new Error.Rank(this.RankErrorText);
            }

            // Only numbers can be an argument for interval
            if (!argument.IsNumber)
            {
                throw new Error.Type(this.TypeErrorText);
            }

            if (argument.Length > 9)
            {
                throw new Error.MaxRank(this.RankErrorText);
            }

            if (argument.IsArray)
            {
                return GenerateMultiDimension(argument);
            }

            int number;
            if (Utils.TryComprasionTolarence(argument.asFloat, out number))
            {
                if (number < 0)
                {
                    throw new Error.Domain(this.DomainErrorText);
                }

                return GenerateVector(number);
            }

            throw new Error.Type(this.TypeErrorText);
        }
    }
}
