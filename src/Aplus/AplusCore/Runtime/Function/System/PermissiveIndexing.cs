using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function
{
    internal static partial class SystemFunction
    {
        [SystemFunction(
            "_index",
            "_index{index;array;default}: returns with the default if the index is invalid.")]
        internal static AType PermissiveIndexing(Aplus environment, AType defaultItem, AType array, AType index)
        {
            PermissiveIndexingErrorCheck(index, array, defaultItem);

            if (index.Type == ATypes.ANull)
            {
                return AArray.Create(ATypes.ANull);
            }

            return PermissiveIndexingSubIndex(index, array, defaultItem, environment);
        }

        private static AType PermissiveIndexingSubIndex(
            AType index, AType array, AType defaultItem, Aplus environment)
        {
            AType result = AArray.Create(array.Type);

            if (index.IsArray)
            {
                for (int i = 0; i < index.Length; i++)
                {
                    result.Add(PermissiveIndexingSubIndex(index[i], array, defaultItem, environment));
                }
            }
            else if (index.asInteger > array.Length - 1 || index.asInteger < 0)
            {
                if (defaultItem.Rank == 0 && array[0].Rank != 0)
                {
                    result = DyadicFunctionInstance.Reshape.Execute(defaultItem, array[0].Shape.ToAArray(), environment);
                }
                else
                {
                    result = defaultItem;
                }
            }
            else
            {
                result = array[index];
            }

            return result;
        }

        #region Error checking

        private static void PermissiveIndexingErrorCheck(AType index, AType array, AType defaultItem)
        {
            if (index.Type != ATypes.AInteger && index.Type != ATypes.AFloat && index.Type != ATypes.ANull)
            {
                throw new Error.Type("_index");
            }

            if (index.Type == ATypes.AFloat)
            {
                Stack<AType> stack = new Stack<AType>();
                stack.Push(index);

                while (stack.Count != 0)
                {
                    AType actual = stack.Pop();
                        
                    if (actual.IsArray)
                    {
                        for (int i = 0; i < actual.Length; i++)
                        {
                            stack.Push(actual[i]);
                        }
                    }
                    else if (!actual.IsTolerablyWholeNumber)
                    {
                        throw new Error.Type("_index");
                    }
                }
            }

            if (!Utils.IsSameGeneralType(array, defaultItem))
            {
                throw new Error.Type("_index");
            }

            if (array.Rank <= defaultItem.Rank)
            {
                throw new Error.Rank("_index");
            }

            if (defaultItem.Rank != 0 &&
                !array[0].Shape.SequenceEqual(defaultItem.Shape))
            {
                throw new Error.Rank("_index");
            }
        }

        #endregion
    }
}