using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class TransposeAxes : AbstractDyadicFunction
    {
        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment)
        {
            int[] transposeVector;
            int[] duplicateItems;
            ExtractData(left, right, out transposeVector, out duplicateItems);
            
            LinkedList<int> newShape = ComputeNewShape(right, transposeVector, duplicateItems);

            return CreateArray(right, newShape, new List<int>(), transposeVector);
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Extract data required for further processing.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="rvDuplicateItems"></param>
        /// <param name="rvTransposeVector"></param>
        private void ExtractData(
            AType left, AType right, out int[] rvTransposeVector, out int[] rvDuplicateItems)
        {
            // left side type must be Float, Integer or Null, else Type error
            if (left.Type != ATypes.AFloat && left.Type != ATypes.AInteger && left.Type != ATypes.ANull)
            {
                throw new Error.Type(TypeErrorText);
            }

            // if it's scalar then we wrap it to one-element vector
            AType vector = left.IsArray ? left : AArray.Create(left.Type, left);

            // if the rank > 1 than we ravel it
            if (vector.Rank > 1)
            {
                vector = MonadicFunctionInstance.Ravel.Execute(vector);
            }

            // the left side length must equal the right rank
            if (vector.Length != right.Rank)
            {
                throw new Error.Rank(RankErrorText);
            }

            int number;
            // make a statistics from the left side to discover the if the vector contains duplicate items
            int[] stat = new int[vector.Length];
            List<int> transposeVector = new List<int>();

            //Extract items from the left side to integer list.
            foreach (AType item in vector)
            {
                if (!item.ConvertToRestrictedWholeNumber(out number))
                {
                    throw new Error.Type(TypeErrorText);
                }

                // bad index, raise Domain error
                if (number < 0 || number >= vector.Length)
                {
                    throw new Error.Domain(DomainErrorText);
                }

                stat[number]++;
                transposeVector.Add(number);
            }

            // check the list contains duplicate item
            bool firstIsNull = false;
            List<int> duplicateItems = new List<int>();

            for (int i = 0; i < stat.Length; i++)
            {
                if (stat[i] == 0)
                {
                    if (duplicateItems.Count == 0)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }

                    if (!firstIsNull)
                    {
                        firstIsNull = true;
                    }
                }
                else
                {
                    if (firstIsNull)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }

                    if (stat[i] > 1)
                    {
                        duplicateItems.Add(i);
                    }
                }
            }

            if (!right.IsArray && transposeVector.Count == 0)
            {
                // the left side is null and right is scalar, throw Domain error!
                throw new Error.Domain(DomainErrorText);

            }

            rvTransposeVector = transposeVector.ToArray();
            rvDuplicateItems = duplicateItems.ToArray();
        }

        #endregion

        #region Computation

        /// <summary>
        /// Determine the new shape of the result.
        /// </summary>
        /// <param name="arguments">Instead of class variables.</param>
        /// <returns></returns>
        private LinkedList<int> ComputeNewShape(AType items, int[] transposeVector, int[] duplicateItems)
        {
            int[] shape;
            LinkedList<int> result = new LinkedList<int>();

            if (duplicateItems.Length != 0)
            {
                shape = Enumerable.Repeat(int.MaxValue, transposeVector.Length).ToArray();

                for (int i = 0; i < transposeVector.Length; i++)
                {
                    if (duplicateItems.Contains(transposeVector[i]))
                    {
                        shape[transposeVector[i]] = Math.Min(shape[transposeVector[i]], items.Shape[i]);
                    }
                    else
                    {
                        shape[transposeVector[i]] = items.Shape[i];
                    }
                }

                int index = 0;
                while (shape[index] != int.MaxValue)
                {
                    result.AddLast(shape[index++]);
                }
            }
            else
            {
                shape = new int[transposeVector.Length];

                for (int i = 0; i < transposeVector.Length; i++)
                {
                    shape[transposeVector[i]] = items.Shape[i];
                }

                for (int i = 0; i < shape.Length; i++)
                {
                    result.AddLast(shape[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Create the result array with corrpesond shape.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="shape"></param>
        /// <param name="pathVector"></param>
        /// <param name="transposeVector"></param>
        /// <returns></returns>
        private AType CreateArray(AType items, LinkedList<int> shape, List<int> pathVector, int[] transposeVector)
        {
            List<int> newPathVector;
            LinkedList<int> cutShape = null;
            AType result = AArray.Create(ATypes.AArray);

            for (int i = 0; i < shape.First(); i++)
            {
                newPathVector = new List<int>();
                newPathVector.AddRange(pathVector);
                newPathVector.Add(i);

                if (shape.Count > 1)
                {
                    cutShape = new LinkedList<int>(shape);
                    cutShape.RemoveFirst();
                    result.AddWithNoUpdate(CreateArray(items, cutShape, newPathVector, transposeVector));
                }
                else
                {
                    result.AddWithNoUpdate(GetItem(items, newPathVector, transposeVector));
                }
            }

            result.Length = shape.First();
            result.Shape = new List<int>() { result.Length };

            if (shape.Count > 1)
            {
                result.Shape.AddRange(cutShape.ToList());
            }

            result.Rank = result.Shape.Count;
            result.Type = 
                result.Length > 0 ? result[0].Type : (items.MixedType() ? ATypes.ANull : items.Type);

            return result;
        }

        /// <summary>
        /// Determine the original index from the new index.
        /// </summary>
        /// <param name="newPathVector"></param>
        /// <param name="transposeVector"></param>
        /// <returns></returns>
        private static int[] GetOriginalIndex(List<int> newPathVector, int[] transposeVector)
        {
            int[] originalPathVector = new int[transposeVector.Length];

            for (int i = 0; i < transposeVector.Length; i++)
            {
                originalPathVector[i] = newPathVector[transposeVector[i]];
            }

            return originalPathVector;
        }

        /// <summary>
        /// Select item from items array by pathVector.
        /// </summary>
        /// <param name="pathVector"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private static AType GetItem(AType items, List<int> pathVector, int[] transposeVector)
        {
            int[] originalPathVector = GetOriginalIndex(pathVector, transposeVector);

            AType result = items;

            foreach (int pathElement in originalPathVector)
            {
                result = result[pathElement];
            }

            return result.Clone();
        }

        #endregion
    }
}
