using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Structural
{

    class Reshape : AbstractDyadicFunction
    {

        #region Entry point

        public override AType Execute(AType rightArgument, AType leftArgument, Aplus environment = null)
        {
            if (leftArgument.Type != ATypes.AInteger)
            {
                throw new Error.Type(this.TypeErrorText);
            }

            AType left = Function.Monadic.MonadicFunctionInstance.Ravel.Execute(leftArgument);
            AType right = Function.Monadic.MonadicFunctionInstance.Ravel.Execute(rightArgument);

            AType result;

            switch (CheckVector(left))
            {
                case State.NullFound:
                    // Found a zero in the list, create an emtpy list with correct shape
                    result = CreateResult(left, right);
                    result.Shape = new List<int>(left.Select(item => { return item.asInteger; }));
                    result.Rank = result.Shape.Count;
                    result.Length = result.Shape[0];
                    break;
                case State.DomainError:
                    throw new Error.Domain(this.DomainErrorText);
                case State.MaxRankError:
                    throw new Error.MaxRank(this.MaxRankErrorText);

                default:
                case State.OK:
                    result = CreateResult(left, right);
                    break;
            }

            return result;
        }

        #endregion

        #region Methods

        private AType Process(AType shape, AType input, ref int pos, int shapePos)
        {
            if (!shape.IsArray)
            {
                return ProcessToVector(shape, input, ref pos);
            }

            AType result = AArray.Create(input.Type);
            int itemCount = shape[shapePos].asInteger;
            for (int i = 0; i < itemCount; i++)
            {
                if (shapePos + 1 >= shape.Length)
                {
                    return ProcessToVector(shape[shapePos], input, ref pos);
                }
                else
                {
                    result.AddWithNoUpdate(Process(shape, input, ref pos, shapePos + 1));
                }
            }

            result.UpdateInfo();
            return result;
        }

        /// <summary>
        /// Generate a vector with the length of the specified shape
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="input">Vector of input elements</param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private AType ProcessToVector(AType shape, AType input, ref int pos)
        {
            AType result = AArray.Create(input.Type);
            int itemCount = shape.asInteger;
            for (int i = 0; i < itemCount; i++)
            {
                result.AddWithNoUpdate(input[pos++].Clone());
                if (pos >= input.Length)
                {
                    pos = 0;
                }
            }

            result.UpdateInfo();
            return result;
        }


        private AType CreateResult(AType left, AType right)
        {
            int pos = 0;
            if (right.Length == 0)
            {
                AType filler = Utils.FillElement(right.Type == ATypes.ANull ? ATypes.ABox : right.Type);

                return Process(left, AArray.Create(filler.Type, filler), ref pos, 0);
            }

            if (right.IsArray)
            {
                return Process(left, right, ref pos, 0);
            }

            return Process(left, AArray.Create(right.Type, right), ref pos, 0);
        }

        /// <summary>
        /// Checks if the left argument can be safely used
        /// </summary>
        /// <param name="left">Vector of numbers</param>
        /// <returns>
        ///  State.NullFound: if a 0 found in the vector
        ///  State.DomainError: if a negative number found or 
        ///                     multiplication of the numbers in the list results a value grater than INT.MAX
        ///  State.MaxRankError: the vector has more than MAXRANK(9) items
        ///  State.Ok: if everything is ok
        /// </returns>
        private State CheckVector(AType left)
        {
            float counter = 1;
            foreach (AType item in left)
            {
                if (item.asInteger == 0)
                {
                    return State.NullFound;
                }

                counter *= item.asInteger;

                if (counter >= Int32.MaxValue || item.asInteger < 0)
                {
                    return State.DomainError;
                }
            }

            if (left.Length > 9)
            {
                return State.MaxRankError;
            } 

            return State.OK;
        }
        #endregion

        #region Helper enum

        enum State
        {
            OK,
            NullFound,
            DomainError,
            MaxRankError
        }

        #endregion

    }

}
