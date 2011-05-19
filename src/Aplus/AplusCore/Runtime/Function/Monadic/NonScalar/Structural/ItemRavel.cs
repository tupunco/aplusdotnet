using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class ItemRavel : AbstractMonadicFunction
    {
        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            if(argument.Rank < 2)
            {
                throw new Error.Rank(RankErrorText);
            }

            AType result = AArray.Create(argument.Type);

            foreach (AType item in argument)
            {
                foreach (AType element in item)
                {
                    result.AddWithNoUpdate(element.Clone());
                }
            }

            result.Shape = new List<int>() { argument.Shape[0] * argument.Shape[1] };
            result.Shape.AddRange(argument.Shape.GetRange(2, argument.Shape.Count - 2));
            result.Length = result.Shape[0];
            result.Rank = result.Shape.Count;

            return result;
        }
    }
}
