using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        /// <summary>
        /// Returns 1 if the input is a slotfiller, otherwise 0.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [SystemFunction("_issf", "_issf{x}:If x is a slotfiller, the result is 1. Otherwise the result is 0.")]
        internal static AType IsaSlotfiller(Aplus environment, AType input)
        {
            AType result;

            if (input.IsSlotFiller())
            {
                result = AInteger.Create(1);
            }
            else
            {
                result = AInteger.Create(0);
            }

            return result;
        }
    }
}
