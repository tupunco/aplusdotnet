using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Rake : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, Aplus environment)
        {
            AType result;

            if (argument.SimpleArray())
            {
                result = SimpleCase(argument);
            }
            else
            {
                result = NestedCase(argument);
            }

            return result;
        }

        #endregion

        #region Computation

        /// <summary>
        /// If argument is simple and not the Null, the result is a one-element nested vector.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType SimpleCase(AType argument)
        {
            AType result;

            if (argument.Type == ATypes.ANull)
            {
                result = Utils.ANull();
            }
            else
            {
                AType enclosedItem = MonadicFunctionInstance.Enclose.Execute(argument);
                result = AArray.Create(enclosedItem.Type, enclosedItem);
            }

            return result;
        }

        private AType NestedCase(AType argument)
        {
            AType result = AArray.Create(ATypes.AArray);

            int itemCount = DiscloseNestedElement(argument, result);

            result.Type = itemCount > 0 ? argument.Type : ATypes.ANull;
            result.Length = itemCount;
            result.Shape = new List<int>() { itemCount };
            result.Rank = 1;

            return result;
        }

        /// <summary>
        /// The result is a nested vector whose depth is one.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="result"></param>
        /// <returns>Number of items disclosed.</returns>
        private int DiscloseNestedElement(AType argument, AType result)
        {
            int count = 0;

            if (argument.IsArray)
            {
                // if rank of the argument is higher than one, so not vector, we ravel it
                AType raveled = (argument.Rank > 1 ? MonadicFunctionInstance.Ravel.Execute(argument) : argument);

                // call this function recursively for each element in the array.
                foreach (AType item in raveled)
                {
                    count += DiscloseNestedElement(item, result);
                }
            }
            else
            {
                // get the depth of the argument
                int depth = MonadicFunctionInstance.Depth.Execute(argument).asInteger;

                // if depth is bigger than 1, we disclose it
                if (depth > 1)
                {
                    count += DiscloseNestedElement(MonadicFunctionInstance.Disclose.Execute(argument), result);
                }
                else
                {
                    bool isBox = argument.IsBox;
                    // leave out Null item
                    if((isBox && argument.NestedItem.Type != ATypes.ANull) || !isBox)
                    {
                        
                        result.AddWithNoUpdate(argument);
                        count++;
                    }
                }
            }

            return count;
        }

        #endregion
    }
}
