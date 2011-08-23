using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        /// <summary>
        /// Convert the given argument to a slotfiller, if possible.
        /// </summary>
        /// <param nave="environment"></param>
        /// <param name="input">The thing that we convert to slotfiller.</param>
        /// <returns>A slotfiller.</returns>
        [SystemFunction("_alsf", "_alsf{x}: returns with a slotfiller")]
        internal static AType Alsf(Aplus environment, AType input)
        {
            if (input.Rank > 1)
            {
                throw new Error.Rank("_alsf");
            }

            if (input.IsSlotFiller())
            {
                return input.Clone();
            }

            return (input.IsArray) ? ArrayInput(input) : NotArrayInput(input);
        }

        private static AType NotArrayInput(AType input)
        {
            AType result;

            if (input.Type == ATypes.ABox)
            {
                result = AArray.Create(ATypes.ABox, ABox.Create(Utils.ANull()), ABox.Create(Utils.ANull()));
            }
            else if (input.Type == ATypes.ASymbol)
            {
                result = AArray.Create(ATypes.ABox, ABox.Create(input), ABox.Create(Utils.ANull()));
            }
            else
            {
                throw new Error.Type("_alsf");
            }

            return result;
        }

        private static AType ArrayInput(AType input)
        {
            AType result;
            AType titles = AArray.Create(ATypes.ANull);
            AType elements = AArray.Create(ATypes.ANull);
            int odd = input.Length % 2;

            for (int i = 0; i < input.Length - odd; i += 2)
            {
                AType key = MonadicFunctionInstance.Disclose.Execute(input[i]);
                AType value = input[i + 1];

                if (key.Type != ATypes.ASymbol)
                {
                    throw new Error.Domain("_alsf");
                }

                if (!value.IsBox)
                {
                    value = ABox.Create(value);
                }

                titles.Add(key);
                elements.Add(value);
            }

            AType lastItem;

            if (input.Length == 0)
            {
                lastItem = input;
            }
            else
            {
                lastItem = MonadicFunctionInstance.Disclose.Execute(input[input.Length - 1]);
            }

            if (odd != 0 && lastItem.Type != ATypes.ANull)
            {
                titles.Add(lastItem);
                elements.Add(ABox.Create(AArray.Create(ATypes.ANull)));
            }

            result = AArray.Create(ATypes.AType, ABox.Create(titles), ABox.Create(elements));

            return result;
        }
    }
}
