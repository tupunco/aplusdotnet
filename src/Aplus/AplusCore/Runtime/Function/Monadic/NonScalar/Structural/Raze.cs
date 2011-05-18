using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Structural
{
    class Raze : AbstractMonadicFunction
    {
        #region Variables

        private AType result;
        private int length;

        #endregion

        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment)
        {
            return Compute(argument);
        }

        #endregion

        #region Computation

        private AType Compute(AType argument)
        {
            if (argument.SimpleArray())
            {
                return String.IsNullOrEmpty(argument.MemoryMappedFile) ?
                    argument.Clone() :
                    argument;
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
                        return MonadicFunctionInstance.Disclose.Execute(argument);
                    case 1:
                        return NestedVector(argument);
                    default:
                        throw new Error.Rank(RankErrorText);
                }
            }
        }

        /// <summary>
        /// Argument is a nested vector than we: >(0#x), >(1#x),..., >(-1+#x)#x
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType NestedVector(AType argument)
        {
            this.result = AArray.Create(ATypes.AArray);

            //Disclose the first item of the argument, set type, rank and shape.
            AType disclosedItem = MonadicFunctionInstance.Disclose.Execute(argument[0]);
            AType disclosedArray = disclosedItem.IsArray ? disclosedItem : AArray.Create(disclosedItem.Type, disclosedItem);
            ATypes type = disclosedArray.Type;
            int rank = disclosedArray.Rank;
            List<int> shape = disclosedArray.Shape;

            bool convertToFloat = false;

            //First Float null item.
            bool FloatNull = disclosedArray.Length == 0 && type == ATypes.AFloat;
            this.length = 0;

            //Add first item to the result.
            Catenate(disclosedArray);

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

                    //If one item of the argument is Float then all item will be Float
                    //If and only if other items is Integers or Floats.
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
                    else
                    {
                        if (!type.MixedType() || !disclosedArray.MixedType())
                        {
                            //Type is differnce so we throw Type error.
                            throw new Error.Type(TypeErrorText);
                        }
                    }
                }

                FloatNull = false;

                if (rank != disclosedArray.Rank)
                {
                    throw new Error.Rank(RankErrorText);
                }

                //Mismatch error arise if actual item has bigger rank than 1 and different from the shape.
                if (!shape.SequenceEqual(disclosedArray.Shape))
                {
                    if (shape.Count > 1 || disclosedArray.Shape.Count > 1)
                    {
                        throw new Error.Mismatch(MismatchErrorText);
                    }
                }

                Catenate(disclosedArray);
            }

            //Set result properties.
            this.result.Length = length;
            this.result.Rank = rank;
            this.result.Shape = new List<int>() { length };
            if (rank > 1)
            {
                this.result.Shape.AddRange(shape.GetRange(1, shape.Count - 1));
            }

            //Convert items to Float, if this flag is true.
            if (convertToFloat)
            {
                this.result.ConvertToFloat();
            }
            else
            {
                this.result.Type = type;
            }

            return result;
        }

        /// <summary>
        /// Add/Catenate argument to the result.
        /// </summary>
        /// <param name="argument"></param>
        private void Catenate(AType argument)
        {
            for (int i = 0; i < argument.Length; i++)
            {
                this.result.AddWithNoUpdate(argument[i].Clone());
            }

            this.length += argument.Length;
        }

        #endregion
    }
}
