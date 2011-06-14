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
        internal static AType Alsf(AplusEnvironment environment, AType input)
        {
            AType result;

            if (input.Rank > 1)
            {
                throw new Error.Rank("_alsf");
            }

            if (input.IsSlotFiller())
            {
                result = input.Clone();
            }
            else
            {
                if (input.Type != ATypes.ABox && input.Type != ATypes.ASymbol && input.Type != ATypes.ANull)
                {
                    throw new Error.Type("_alsf");
                }

                AType titlesArray = AArray.Create(ATypes.ANull);
                AType elements = AArray.Create(ATypes.ANull);
                int odd = input.Length % 2;

                for (int i = 0; i < input.Length - odd; i += 2)
                {
                    if (input[i].NestedItem.Type != ATypes.ASymbol)
                    {
                        throw new Error.Domain("_alsf");
                    }

                    titlesArray.Add(input[i].NestedItem);
                    elements.Add(input[i + 1]);
                }

                if (odd != 0 && input[input.Length - 1].NestedItem.Type != ATypes.ANull)
                {
                    titlesArray.Add(input[input.Length - 1].NestedItem);
                    elements.Add(ABox.Create(AArray.Create(ATypes.ANull)));
                }

                result = AArray.Create(ATypes.AType, ABox.Create(titlesArray), ABox.Create(elements));
            }

            return result;
        }

    }
}
