using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;
using AplusCore.Runtime.Function.Monadic;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Selection
{
    class Pick : AbstractDyadicFunction
    {
        #region Variables

        private bool resultFromBox;

        #endregion

        #region Delegate helper

        private delegate AType ItemSelectDelegate(AType arg, AType items, AplusEnvironment environment);
        private static ItemSelectDelegate SymbolConstant2SlotFillerDelegate;
        private static ItemSelectDelegate NestedPathNumber2NestedArrayDelegate;

        private AType ItemSelectWalker(ItemSelectDelegate method, AType argument, AType items, AplusEnvironment environment)
        {
            AType result = items;

            if (argument.IsArray)
            {
                if (argument.Rank > 1)
                {
                    throw new Error.Rank(RankErrorText);
                }

                foreach (AType elem in argument)
                {
                    result = method(elem, result, environment);
                }
            }
            else
            {
                result = method(argument, items, environment);
            }

            return result;
        }

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, AplusEnvironment environment = null)
        {
            if (Pick.SymbolConstant2SlotFillerDelegate == null)
            {
                Pick.SymbolConstant2SlotFillerDelegate = new ItemSelectDelegate(SimpleSymbolConstant2SlotFiller);
                Pick.NestedPathNumber2NestedArrayDelegate = new ItemSelectDelegate(NestedPathNumber2NestedArray);
            }
            this.resultFromBox = false;

            AType result = Compute(right, left, environment);
            return result;
        }

        #endregion

        #region Assign Entry point

        public PickAssignmentTarget AssignExecute(AType right, AType left, AplusEnvironment environment = null)
        {
            AType result = Execute(right, left, environment);
            return new PickAssignmentTarget(result, this.resultFromBox);
        }

        #endregion

        #region Computation

        #region Selection

        public AType Compute(AType right, AType left, AplusEnvironment environment)
        {
            //Type Error!
            if (left.Type == ATypes.AChar || 
                (left.Type == ATypes.AFloat && left.asInteger != left.asFloat))
            {
                throw new Error.Type(TypeErrorText);
            }


            // Case 2: If the right argument is scalar than we wrap it to one-element vector.
            AType rightArray;

            if(right.IsArray)
            {
                rightArray = right;
            }
            else
            {
                rightArray = AArray.Create(
                    right.Type,
                    !String.IsNullOrEmpty(right.MemoryMappedFile) ?
                        right.Clone() :
                        right
                    );
            }

            if (left.IsBox)                         // Case 6
            {
                return ItemSelectWalker(NestedPathNumber2NestedArrayDelegate, left, rightArray, environment);
            }
            else if (left.Type == ATypes.ASymbol)   // Case 3
            {
                return ItemSelectWalker(SymbolConstant2SlotFillerDelegate, left, rightArray, environment);
            }
            else if (right.IsBox)                   // Case 5
            {
                return PathVector2NestedVector(left, rightArray);
            }
            else                                    // Case 1-2-4
            {
                return SimpleNumeric2SimpleRight(left, rightArray);
            }
        }

        #endregion

        #region Cases

        #region Case 3

        /// <summary>
        /// CASE 3: Slotfiller right argument and simple symbol left argument.
        /// </summary>
        /// <param name="symbol">Symbol to use for selecting from a slotfiller</param>
        /// <param name="items">Slotfiller</param>
        /// <returns></returns>
        private AType SimpleSymbolConstant2SlotFiller(AType symbol, AType items, AplusEnvironment environment)
        {
            if (!items.IsSlotFiller(true))
            {
                throw new Error.Domain(DomainErrorText);
            }

            AType keys = MonadicFunctionInstance.Disclose.Execute(items[0], environment);
            AType values = MonadicFunctionInstance.Disclose.Execute(items[1], environment);

            if (keys.IsArray)
            {
                if (keys.Rank > 1)
                {
                    keys = MonadicFunctionInstance.Ravel.Execute(keys);
                }

                if (values.Rank > 1)
                {
                    values = MonadicFunctionInstance.Ravel.Execute(values);
                }

                int index = -1;

                // TODO: refactor, remove 'i' use the 'index' variable
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].IsBox)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }

                    if (keys[i].CompareTo(symbol) == 0)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    //y is a symbol and is not a member of the vector of symbol s in x.
                    throw new Error.Index(IndexErrorText);
                }

                if (values[index].IsPrimitiveFunction() || !values[index].IsBox)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                this.resultFromBox = true;
                return MonadicFunctionInstance.Disclose.Execute(values[index], environment);
            }
            else
            {
                // We have only one item in the keys list
                if (symbol.CompareTo(keys) != 0)
                {
                    throw new Error.Index(IndexErrorText);
                }

                if (values.IsPrimitiveFunction() || !values.IsBox)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                this.resultFromBox = true;
                return MonadicFunctionInstance.Disclose.Execute(values, environment);
            }
        }

        #endregion

        #region Case 4

        /// <summary>
        /// CASE 4: Simple right argument and simple left argument.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public AType SimpleNumeric2SimpleRight(AType left, AType right)
        {
            AType item;

            if (!left.TryFirstScalar(out item, true))
            {
                if (left.Rank > 1)
                {
                    throw new Error.Rank(RankErrorText);
                }

                if (left.Length > 1)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                // Case 0
                if (left.Type == ATypes.ANull || left.Length == 0)
                {
                    return right;
                }

                throw new Error.Mismatch("Unreachable Code!");
            }

            int index = item.asInteger;

            if (right.Rank > 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            if (index < 0 || index >= right.Length)
            {
                throw new Error.Index(IndexErrorText);
            }

            this.resultFromBox = false;
            return right[index];
        }

        #endregion

        #region Case 5

        /// <summary>
        /// CASE 5: Nested vector right argument and simple path vector left argument.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private AType PathVector2NestedVector(AType path, AType items)
        {
            AType result;
            AType element;

            if (path.TryFirstScalar(out element, true))
            {
                result = PathNumber2NestedVector(element, items);
            }
            else
            {
                if (path.Rank > 1)
                {
                    throw new Error.Rank(RankErrorText);
                }

                //Case 0
                if (path.Type == ATypes.ANull || path.Length == 0)
                {
                    return items;
                }

                result = items;
                foreach (AType item in path)
                {
                    result = PathNumber2NestedVector(item, result);
                }
            }

            return result;
        }

        //Path must be AInteger!
        /// <summary>
        /// CASE 5: Nested vector right argument and simple left argument.
        /// </summary>
        /// <param name="path">Integer scalar</param>
        /// <param name="items"></param>
        /// <returns></returns>
        private AType PathNumber2NestedVector(AType path, AType items)
        {
            int index = path.asInteger;

            if (!items.IsBox)
            {
                throw new Error.Domain(DomainErrorText);
            }

            //Items must be a vector!
            if (!items.IsArray || items.Rank != 1)
            {
                throw new Error.Rank(RankErrorText);
            }

            if (index < 0 || index >= items.Length)
            {
                throw new Error.Index(IndexErrorText);
            }

            this.resultFromBox = true;
            return MonadicFunctionInstance.Disclose.Execute(items[index]);
        }

        #endregion

        #region Case 6

        /// <summary>
        /// CASE 6: Nested right argument and nested left argument.
        /// </summary>
        /// <param name="pathBox"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private AType NestedPathNumber2NestedArray(AType pathBox, AType items, AplusEnvironment environment)
        {
            AType path = MonadicFunctionInstance.Disclose.Execute(pathBox, environment);

            if (path.IsFunctionScalar)
            {
                throw new Error.Domain(DomainErrorText);
            }

            //Nested scalar or vector whose items are simple scalar or vector of integers.
            if (path.IsBox)
            {
                throw new Error.Type(TypeErrorText);
            }

            //Right argument is ().
            if (items.Type == ATypes.ANull)
            {
                throw new Error.Index(IndexErrorText);
            }

            if (path.Type == ATypes.ANull)
            {
                if (!items.IsBox)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                AType result;

                if (!items.TryFirstScalar(out result, true))
                {
                    throw new Error.Domain(DomainErrorText);
                }

                this.resultFromBox = true;
                return MonadicFunctionInstance.Disclose.Execute(result, environment);
            }
            else if (path.Type == ATypes.AInteger)
            {
                if (path.Rank > 1)
                {
                    throw new Error.Rank(RankErrorText);
                }

                if (path.Length == 1)
                {
                    //Case 5
                    return PathNumber2NestedVector(path.IsArray ? path[0] : path, items);
                }

                // The 'path' variable must be an AArray after this point.
                // so we treat it like that

                if (!items.IsBox)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                //Length: 0
                if (path.Length != items.Rank || path.Length == 0)
                {
                    throw new Error.Rank(RankErrorText);
                }

                List<int> shape = items.Shape;
                int index;

                for (int i = 0; i < path.Length; i++)
                {
                    index = path[i].asInteger;

                    if (index < 0 || index >= shape[i])
                    {
                        throw new Error.Index(IndexErrorText);
                    }

                    items = items[index];
                }

                this.resultFromBox = true;
                return MonadicFunctionInstance.Disclose.Execute(items, environment);
            }
            else if (path.Type == ATypes.ASymbol)
            {
                // Case 3
                return ItemSelectWalker(SymbolConstant2SlotFillerDelegate, path, items, environment);
            }
            else if (path.Type == ATypes.AChar)
            {
                throw new Error.Domain(DomainErrorText);
            }
            else
            {
                throw new Error.Type(TypeErrorText);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
