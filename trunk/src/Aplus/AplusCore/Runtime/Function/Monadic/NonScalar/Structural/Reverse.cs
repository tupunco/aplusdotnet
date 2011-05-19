using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Reverse : AbstractMonadicFunction
    {

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            if (argument.IsArray)
            {
                AType result = AArray.Create(ATypes.AArray);

                for (int i = argument.Length - 1;  i >= 0;  i--)
                {
                    result.AddWithNoUpdate(argument[i].Clone());
                }

                result.Length = argument.Length;
                result.Shape = new List<int>(argument.Shape);
                result.Rank = argument.Rank;
                result.Type = result.Length > 0 ? result[0].Type : ATypes.ANull;

                return result;
            }
            else
            {
                return argument.Clone();
            }
        }
    }
}
