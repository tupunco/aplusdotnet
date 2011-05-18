using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;
using AplusCore.Runtime.Function.Dyadic;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Disclose : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            if (argument.SimpleArray())
            {
                //The argument is simple array/scalar and not mapped we clone it! 
                return argument.IsMemoryMappedFile ?
                    argument :
                    argument.Clone();
            }
            else
            {
                if (!argument.NestedArray())
                {
                    throw new Error.Domain(DomainErrorText);
                }

                return DiscloseNestedArray(argument, environment);
            }
        }

        #endregion

        #region Computation

        private AType DiscloseNestedVector(AType argument, out List<int> shape)
        {
            //TODO: Is Clone method correct here?
            AType item = argument[0].NestedItem.Clone();

            ATypes type = item.Type;
            shape = item.Shape;
            int rank = item.Rank;

            if (CheckFloat(argument))
            {
                //Convert the first item to AFloat.
                if (item.Type == ATypes.AInteger)
                {
                    item = ConvertToFloat(item);
                    type = ATypes.AFloat;
                }

                if (item.Type != ATypes.AFloat)
                {
                    throw new Error.Type(TypeErrorText);
                }
            }

            AType result = AArray.Create(type, item);

            for (int i = 1; i < argument.Length; i++)
            {
                //TODO: Is Clone method correct here?
                item = argument[i].NestedItem.Clone();

                // Uniform rules!
                if (item.Type != type)
                {
                    if (type == ATypes.AFloat && item.Type == ATypes.AInteger)
                    {
                        item = ConvertToFloat(item);
                    }
                    else
                    {
                        if(!type.MixedType() || !item.MixedType())
                        {
                            throw new Error.Type(TypeErrorText);
                        }
                    }
                }
                
                if (item.Rank != rank)
                {
                    throw new Error.Rank(RankErrorText);
                }

                if (!item.Shape.SequenceEqual(shape))
                {
                    throw new Error.Mismatch(MismatchErrorText);
                }

                result.AddWithNoUpdate(item);
            }

            result.Length = argument.Length;
            result.Shape = new List<int>() { result.Length };
            result.Shape.AddRange(shape);
            result.Rank = 1 + rank;

            return result;
        }

        /// <summary>
        /// Check the argument contains type of float nested item.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private bool CheckFloat(AType argument)
        {
            foreach (AType item in argument)
            {
                if (item.NestedItem.Type == ATypes.AFloat)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Convert the argument to float array/scalar.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType ConvertToFloat(AType argument)
        {
            if (!argument.IsBox)
            {
                // TODO: remove is array logic from here => crate general to float converter
                if (argument.IsArray)
                {
                    argument.ConvertToFloat();
                }
                else
                {
                    argument = AFloat.Create(argument.asFloat);
                }

                return argument;
            }
            else
            {
                throw new Error.Type(TypeErrorText);
            }
        }

        /// <summary>
        /// Reconduct the argument to vectors.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        private AType DiscloseNestedArray(AType argument, AplusEnvironment environment)
        {
            AType result;

            if (argument.IsArray)
            {
                List<int> insideShape;

                if (argument.Rank > 1)
                {
                    result = MonadicFunctionInstance.Ravel.Execute(argument, environment);
                    result = DiscloseNestedVector(result, out insideShape);

                    List<int> newShape = new List<int>(argument.Shape);
                    newShape.AddRange(insideShape);

                    result = DyadicFunctionInstance.Reshape.Execute(result, newShape.ToAArray(), environment);
                }
                else
                {
                    result = DiscloseNestedVector(argument, out insideShape);
                }
            }
            else
            {
                // Get the nested scalar
                result = argument.NestedItem;
            }

            if (result.Rank > 9)
            {
                throw new Error.MaxRank(MaxRankErrorText);
            }

            return result;
        }

        #endregion
    }
}
