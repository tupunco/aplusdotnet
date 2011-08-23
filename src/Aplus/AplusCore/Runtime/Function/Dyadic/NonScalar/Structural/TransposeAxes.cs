using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime.Function.Monadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{
    class TransposeAxes : AbstractDyadicFunction
    {
        #region Variables

        private List<int> transposeVector;
        private AType items;
        private List<int> duplicateItem;

        #endregion

        #region Entry point

        public override AType Execute(AType right, AType left, Aplus environment)
        {
            PrepareAttributes(left, right);
            return CreateArray(ComputeNewShape(), new List<int>());
        }

        #endregion

        #region Preparation

        /// <summary>
        /// Set left and right side of the variables.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void PrepareAttributes(AType left, AType right)
        {
            //Left side type must be Float, Integer or Null, else Type error.
            if (left.Type != ATypes.AFloat && left.Type != ATypes.AInteger && left.Type != ATypes.ANull)
            {
                throw new Error.Type(TypeErrorText);
            }

            //If it's scalar then we wrap it to one-element vector.
            AType vector = left.IsArray ? left : AArray.Create(left.Type, left);

            //If the rank > 1 than we ravel it.
            if (vector.Rank > 1)
            {
                vector = MonadicFunctionInstance.Ravel.Execute(vector);
            }

            //The left side length must equal the right rank.
            if (vector.Length != right.Rank)
            {
                throw new Error.Rank(RankErrorText);
            }

            int number;
            //Make a statistics from the left side to discover the vector contain duplicate items.
            int[] stat = new int[vector.Length];
            this.transposeVector = new List<int>();

            //Extract items from the left side to integer list.
            foreach (AType item in vector)
            {
                if (item.ConvertToRestrictedWholeNumber(out number))
                {
                    //Bad index, raise Domain error!
                    if (number < 0 || number >= vector.Length)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }

                    stat[number]++;
                    this.transposeVector.Add(number);
                }
                else
                {
                    throw new Error.Type(TypeErrorText);
                }
            }

            //Check the list contains duplicate item.
            bool firstNull = false;
            this.duplicateItem = new List<int>();

            for (int i = 0; i < stat.Length; i++)
            {
                if (stat[i] == 0)
                {
                    if (this.duplicateItem.Count != 0)
                    {
                        if (!firstNull)
                        {
                            firstNull = true;
                        }
                    }
                    else
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                }
                else
                {
                    if (firstNull)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }

                    if(stat[i] > 1)
                    {
                        duplicateItem.Add(i);
                    }
                }
            }

            //Set the right side.
            if (right.IsArray)
            {
                this.items = right;
            }
            else
            {
                //The left side is null and right is scalar, throw Domain error!
                if (this.transposeVector.Count == 0)
                {
                    throw new Error.Domain(DomainErrorText);
                }
            }
        }

        #endregion

        #region Computation

        /// <summary>
        /// Determine the new shape of the result.
        /// </summary>
        /// <returns></returns>
        private LinkedList<int> ComputeNewShape()
        {
            int[] shape;
            LinkedList<int> result = new LinkedList<int>();

            if (this.duplicateItem.Count != 0)
            {
                shape = Enumerable.Repeat(int.MaxValue, this.transposeVector.Count).ToArray();

                for (int i = 0; i < this.transposeVector.Count; i++)
                {
                    if (this.duplicateItem.Contains(this.transposeVector[i]))
                    {
                        shape[this.transposeVector[i]] = Math.Min(shape[this.transposeVector[i]], this.items.Shape[i]);
                    }
                    else
                    {
                        shape[this.transposeVector[i]] = this.items.Shape[i];
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
                shape = new int[this.transposeVector.Count];

                for (int i = 0; i < this.transposeVector.Count; i++)
                {
                    shape[this.transposeVector[i]] = this.items.Shape[i];
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
        /// <param name="shape"></param>
        /// <param name="pathVector"></param>
        /// <returns></returns>
        private AType CreateArray(LinkedList<int> shape, List<int> pathVector)
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
                    result.AddWithNoUpdate(CreateArray(cutShape, newPathVector));
                }
                else
                {
                    result.AddWithNoUpdate(GetItem(newPathVector));
                }
            }

            result.Length = shape.First();
            result.Shape = new List<int>() { result.Length };

            if (shape.Count > 1)
            {
                result.Shape.AddRange(cutShape.ToList());
            }

            result.Rank = result.Shape.Count;
            result.Type = result.Length > 0 ? result[0].Type : (this.items.MixedType() ? ATypes.ANull : this.items.Type);

            return result;
        }
        
        /// <summary>
        /// Determine the original index from the new index.
        /// </summary>
        /// <param name="newPathVector"></param>
        /// <returns></returns>
        private List<int> GetOriginalIndex(List<int> newPathVector)
        {
            List<int> originalPathVector = new List<int>();

            for (int i = 0; i < this.transposeVector.Count; i++)
            {
                originalPathVector.Add(newPathVector[this.transposeVector[i]]);
            }

            return originalPathVector;
        }

        /// <summary>
        /// Select item from items array by pathVector.
        /// </summary>
        /// <param name="pathVector"></param>
        /// <returns></returns>
        private AType GetItem(List<int> pathVector)
        {
            List<int> originalPathVector = GetOriginalIndex(pathVector);

            AType result = this.items;

            for (int i = 0; i < originalPathVector.Count; i++)
            {
                result = result[originalPathVector[i]];
            }

            return result.Clone();
        }

        #endregion
    }
}
