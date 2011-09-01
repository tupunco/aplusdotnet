using System.Collections.Generic;
using System.Linq;
using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        [SystemFunction("_ssr", "_ssr{char;any;any} returns any")]
        internal static AType StringSearchandReplace(Aplus environment, AType replaceWith, AType replaceWhat, AType replaceIn)
        {
            if (replaceIn.Type != ATypes.AChar || replaceWhat.Type != ATypes.AChar || replaceWith.Type != ATypes.AChar)
            {
                throw new Error.Type("_ssr");
            }

            string withString = Monadic.MonadicFunctionInstance.Ravel.Execute(replaceWith, environment).ToString();
            string whatString = Monadic.MonadicFunctionInstance.Ravel.Execute(replaceWhat, environment).ToString();
            AType argument = (withString.Length == whatString.Length)
                ? replaceIn.Clone() : Monadic.MonadicFunctionInstance.Ravel.Execute(replaceIn, environment);

            Queue<string> rows = new Queue<string>();
            ExtractRows(argument, rows);

            Queue<string> replacedRows = new Queue<string>();

            foreach (string item in rows)
            {
                string replaced = item.Replace(whatString, withString);
                replacedRows.Enqueue(replaced);
            }

            AType result = BuildAType(replacedRows, argument.Shape);
            return result;

        }

        private static void ExtractRows(AType argument, Queue<string> rows)
        {
            if (argument.Rank > 1)
            {
                foreach (AType item in argument)
                {
                    ExtractRows(item, rows);
                }
            }
            else
            {
                rows.Enqueue(argument.ToString());
            }
        }

        private static AType BuildAType(Queue<string> rows, List<int> shape)
        {
            AType result;

            if (shape.Count == 1)
            {
                result = Helpers.BuildString(rows.Dequeue());
            }
            else
            {
                result = AArray.Create(ATypes.AChar);
                List<int> subshape = shape.GetRange(1, shape.Count - 1);

                for (int i = 0; i < shape[0]; i++)
                {
                    result.Add(BuildAType(rows, subshape));
                }
            }

            return result;
        }

    }
}
