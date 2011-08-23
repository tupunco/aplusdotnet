using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Rake : AbstractMonadicFunction
    {
        #region Variables

        private AType result;
        private int length;

        #endregion

        #region Entry point

        public override AType Execute(AType argument, Aplus environment)
        {
            return argument.SimpleArray() ? SimpleCase(argument) : NestedCase(argument);
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
            if (argument.Type == ATypes.ANull)
            {
                return Utils.ANull();
            }
            else
            {
                AType result = MonadicFunctionInstance.Enclose.Execute(argument);
                return AArray.Create(result.Type, result);
            }
        }

        private AType NestedCase(AType argument)
        {
            this.result = AArray.Create(ATypes.AArray);
            this.length = 0;

            DiscloseNestedElement(argument);

            this.result.Type = this.length > 0 ? argument.Type : ATypes.ANull;
            this.result.Length = this.length;
            this.result.Shape = new List<int>() { this.length };
            this.result.Rank = 1;

            return result;
        }

        /// <summary>
        /// The result is a nested vector whose depth is one.
        /// </summary>
        /// <param name="argument"></param>
        private void DiscloseNestedElement(AType argument)
        {
            if (argument.IsArray)
            {
                //If rank of the argument is higher than one, so not vector, we ravel it!
                AType raveled = (argument.Rank > 1 ? MonadicFunctionInstance.Ravel.Execute(argument) : argument);

                //Call this function recursively for each element in the array.
                foreach (AType item in raveled)
                {
                    DiscloseNestedElement(item);
                }
            }
            else
            {
                //Determine the depth of the argument.
                int depth = (MonadicFunctionInstance.Depth.Execute(argument)).asInteger;

                //If depth is bigger than 1, we disclose it.
                if (depth > 1)
                {
                    DiscloseNestedElement(MonadicFunctionInstance.Disclose.Execute(argument));
                }
                else
                {
                    //Discard Null!
                    if (argument.IsBox ? !IsNull(argument) : true)
                    {
                        this.result.AddWithNoUpdate(argument);
                        this.length++;
                    }
                }
            }
        }

        // TODO: remove this method: only used in one place
        //Argument must be Box.
        /// <summary>
        /// Check the argument is a nested Null.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private bool IsNull(AType argument)
        {
            return argument.NestedItem.Type == ATypes.ANull;
        }

        #endregion
    }
}
