using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Raze : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, Aplus environment)
        {
            AType result;

            if (argument.SimpleArray())
            {
                result = argument.IsMemoryMappedFile ? argument : argument.Clone();
            }
            else
            {
                if (!argument.NestedArray())
                {
                    throw new Error.Domain(DomainErrorText);
                }

                switch (argument.Rank)
                {
                    case 0:
                        result = MonadicFunctionInstance.Disclose.Execute(argument);
                        break;
                    case 1:
                        result = NestedVector(argument);
                        break;
                    default:
                        throw new Error.Rank(RankErrorText);
                }
            }

            return result;
        }

        #endregion

        #region Computation

        /// <summary>
        /// Argument is a nested vector than we: >(0#x), >(1#x),..., >(-1+#x)#x
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType NestedVector(AType argument)
        {
            AType result = AArray.Create(ATypes.AArray);

            // disclose the first item of the argument, get type, rank and shape
            AType disclosedItem = MonadicFunctionInstance.Disclose.Execute(argument[0]);
            AType disclosedArray = disclosedItem.IsArray ? disclosedItem : AArray.Create(disclosedItem.Type, disclosedItem);

            ATypes type = disclosedArray.Type;
            int rank = disclosedArray.Rank;
            List<int> shape = disclosedArray.Shape;

            bool convertToFloat = false;

            // first Float null item
            bool FloatNull = (disclosedArray.Length == 0 && type == ATypes.AFloat);

            // add first item to the result
            int length = Catenate(disclosedArray, result);

            for (int i = 1; i < argument.Length; i++)
            {
                //Disclose ith item.
                disclosedItem = MonadicFunctionInstance.Disclose.Execute(argument[i]);
                // TODO: do we need these checks?
                disclosedArray = disclosedItem.IsArray ? disclosedItem : AArray.Create(disclosedItem.Type, disclosedItem);

                if (disclosedArray.Length == 0)
                {
                    // skip empty items
                    continue;
                }


                if (type != disclosedArray.Type)
                {

                    // if one item of the argument is Float then all item will be Float
                    // if and only if other items is Integers or Floats.
                    if (type == ATypes.AFloat && disclosedArray.Type == ATypes.AInteger)
                    {
                        if (FloatNull)
                        {
                            type = ATypes.AInteger;
                        }
                        else
                        {
                            convertToFloat = true;
                        }
                    }
                    else if (type == ATypes.AInteger && disclosedArray.Type == ATypes.AFloat)
                    {
                        convertToFloat = true;
                    }
                    else if (type == ATypes.ANull)
                    {
                        type = disclosedArray.Type;
                    }
                    else if (!type.MixedType() || !disclosedArray.MixedType())
                    {
                        // type is differnce so we throw Type error
                        throw new Error.Type(TypeErrorText);
                    }
                }

                FloatNull = false;

                if (rank != disclosedArray.Rank)
                {
                    throw new Error.Rank(RankErrorText);
                }

                // mismatch error arise if actual item has bigger rank than 1 and has a different shape
                if (!shape.SequenceEqual(disclosedArray.Shape))
                {
                    if (shape.Count > 1 || disclosedArray.Shape.Count > 1)
                    {
                        throw new Error.Mismatch(MismatchErrorText);
                    }
                }

                length += Catenate(disclosedArray, result);
            }

            // set result properties
            result.Length = length;
            result.Rank = rank;
            result.Shape = new List<int>() { length };
            if (rank > 1)
            {
                result.Shape.AddRange(shape.GetRange(1, shape.Count - 1));
            }

            // convert items to Float, if this flag is true.
            if (convertToFloat)
            {
                result.ConvertToFloat();
            }
            else
            {
                result.Type = type;
            }

            return result;
        }

        /// <summary>
        /// Catenate items to the result.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="result"></param>
        /// <returns>Number of items added to the result.</returns>
        private static int Catenate(AType items, AType result)
        {
            for (int i = 0; i < items.Length; i++)
            {
                result.AddWithNoUpdate(items[i].Clone());
            }

            return items.Length;
        }

        #endregion
    }
}
