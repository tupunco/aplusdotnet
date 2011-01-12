using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Runtime;
using AplusCore.Runtime.Function.Monadic;

namespace AplusCore.Types
{
    public static class Utils
    {
        
        #region Indexing
        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexers">List containing all of the indexes</param>
        /// <param name="indexpos"></param>
        /// <param name="currentIdx">Array containing the current indexes</param>
        /// <returns></returns>
        public static AType VectorIndexing(this AType input, List<AType> indexers, int indexpos, AType currentIdx)
        {
            // Create an array for the results
            //AArray result = new AArray(this.type);
            AType result = AArray.Create(input.Type);

            List<AType> items = currentIdx.Container;

            if (items.Count == 0)
            {
                // A Null item found!, select all of the current items
                for (int i = 0; i < input.Length; i++)
                {
                    items.Add(AInteger.Create(i));
                }
            }

            // Iterate over the indexes
            foreach (AType index in items)
            {
                AType item =
                    index.IsArray ?
                    input.VectorIndexing(indexers, indexpos, index) :
                    input.SimpleIndex(indexers, indexpos, index);

                result.AddWithNoUpdate(item);
            }

            result.UpdateInfo();
            return result;
        }

        public static AType Indexing(this AType input, List<AType> indexers, int indexpos)
        {
            // Select the current indexer element
            AType currentIdx = indexers[indexpos];

            if (currentIdx.IsArray)
            {
                // The indexer element is an Array handle that way
                return input.VectorIndexing(indexers, indexpos, currentIdx);
            }
            else
            {
                // The indexer element is a simple AType
                return input.SimpleIndex(indexers, indexpos, currentIdx);
            }

        }


        public static AType SimpleIndex(this AType input, List<AType> indexers, int indexpos, AType currentIdx)
        {
            // Get the current item, specified by the currentIdx
            AType item = input[currentIdx];

            if (item.IsArray && indexpos < indexers.Count - 1)
            {
                // If it is an array and we can index further, then do so
                return item.Indexing(indexers, indexpos + 1);
            }
            else
            {
                // Simply return the item
                return item;
            }
        }

        #endregion

        #region Indexed Assign
        
        internal static void PerformAssign(AType target, AType value)
        {
            if (!Utils.DifferentNumberType(target, value) && target.Type != value.Type)
            {
                // The target and value are not numbers and they are of a different type?
                throw new Error.Type("assign");
            }

            if (target.Rank < value.Rank)
            {
                throw new Error.Rank("assign");
            }

            if (value.Length == 1)
            {
                AType result;
                if (!value.TryFirstScalar(out result, true))
                {
                    throw new Error.Rank("assign");
                }

                if (target.Length == 1)
                {
                    PerformIndexAssign(target, result);
                }
                else
                {
                    foreach (AType item in target)
                    {
                        PerformIndexAssign(item, result);
                    }
                }

            }
            else if (target.Length == value.Length)
            {
                for (int i = 0; i < target.Length; i++)
                {
                    PerformIndexAssign(target[i], value[i]);
                }
            }
            else
            {
                throw new Error.Length("assign");
            }
        }

        private static void PerformIndexAssign(AType target, AType value)
        {
            if (target.Shape.Count > 0)
            {
                for (int i = 0; i < target.Length; i++)
                {
                    PerformIndexAssign(target[i], value.IsArray ? value[i] : value);
                }
            }
            else
            {
                if (target.Type == value.Type)
                {
                    target.Data = value.Clone().Data;
                }
                else if (target.Type == ATypes.AInteger && value.Type == ATypes.AFloat)
                {
                    int result;
                    if (!value.ConvertToRestrictedWholeNumber(out result))
                    {
                        throw new Error.Type("assign");
                    }

                    target.Data = AInteger.Create(result).Data;
                }
                else if (target.Type == ATypes.AFloat && value.Type == ATypes.AInteger)
                {
                    target.Data = AFloat.Create(value.asInteger).Data;
                }
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Checks if the supplied ATypes are of a different number type.
        /// </summary>
        /// <returns>
        /// True if the arguments have different number type. 
        /// Every other cases False
        /// </returns>
        internal static bool DifferentNumberType(AType left, AType right)
        {
            return (left.Type == ATypes.AFloat && right.Type == ATypes.AInteger) ||
                   (right.Type == ATypes.AFloat && left.Type == ATypes.AInteger);

        }


        private static Dictionary<Type, ATypes> typeconverter =
            new Dictionary<Type, ATypes>()
            {
                { typeof(AInteger), ATypes.AInteger },
                { typeof(AFloat), ATypes.AFloat },
                { typeof(ASymbol), ATypes.ASymbol },
                { typeof(AChar), ATypes.AChar },
                { typeof(ABox), ATypes.ABox },
                { typeof(AArray), ATypes.AArray },
                { typeof(AType), ATypes.AType },
                { typeof(AFunc), ATypes.AFunc }
            };

        /// <summary>
        /// Get the corresponding ATypes enum value based on the input Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static ATypes GetATypesFromType(Type type)
        {
            ATypes typeInfo;
            if (!typeconverter.TryGetValue(type, out typeInfo))
            {
                throw new Error.Type("Should NOT reach this point");
            }

            return typeInfo;
        }

        /// <summary>
        /// AArray generated by type and shape with Reshape dyadic nonscalar function.
        /// If shape list is null, we give back the filler elemenet.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="shape"></param>
        internal static AType FillElement(ATypes type, AType shape = null)
        {
            AType filler = null;

            switch (type)
            {
                case ATypes.ABox:
                case ATypes.AFunc:
                    filler = ABox.Create(AArray.ANull());
                    break;
                case ATypes.AChar:
                    filler = AChar.Create(' ');
                    break;
                case ATypes.AFloat:
                    filler = AFloat.Create(0);
                    break;
                case ATypes.AInteger:
                    filler = AInteger.Create(0);
                    break;
                case ATypes.ASymbol:
                    filler = ASymbol.Create("");
                    break;
                default:
                    throw new NotImplementedException("Invalid use-case");
            }

            if (shape != null)
            {
                return AplusCore.Runtime.Function.Dyadic.DyadicFunctionInstance.Reshape.Execute(filler, shape);
            }
            else
            {
                return filler;
            }
        }

        ///<summary>
        ///If the input paramter range of integer repressentation, we give back AInteger.
        ///</summary>
        public static bool ConvertDoubleToInteger(double number, out int result)
        {
            if (Int32.MinValue <= number && number <= Int32.MaxValue && number % 1 == 0)
            {
                result = (int)number;
                return true;
            }
            result = -1;
            return false;
        }

        /// <summary>
        /// Returns AInteger or AFloat based on the input number.
        /// If the number can be represented as an integer return AInteger
        /// otherwise return AFloat.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static AType CreateATypeResult(double number)
        {
            int result;
            if (ConvertDoubleToInteger(number, out result))
            {
                return AInteger.Create(result);
            }
            return AFloat.Create(number);
        }

        public static bool ComparisonTolerance(double x, double y)
        {
            double minvalue = Math.Min(Math.Abs(x), Math.Abs(y));
            return Math.Abs(x - y) < (1e-13 * ((minvalue == 0) ? 1 : minvalue));
        }

        public static bool TryComprasionTolarence(double number, out double result)
        {
            result = Math.Round(number);
            return ComparisonTolerance(number, result);
        }

        public static bool TryComprasionTolarence(double number, out int result)
        {
            result = (int)Math.Round(number);
            return ComparisonTolerance(number, result);
        }

        #endregion

        #region AArray methods


        /// <summary>
        /// Add AType to AArray.
        /// Also updates length, shape, rank and type informations
        /// </summary>
        /// <param name="item"></param>
        public static void Add(this AType array, AType item)
        {
            array.Container.Add(item);
            array.UpdateInfo();
        }

        /// <summary>
        /// Add AType to AArray without updating length, shape, rank and type informations
        /// </summary>
        /// <param name="item"></param>
        public static void AddWithNoUpdate(this AType array, AType item)
        {
            array.Container.Add(item);
        }

        /// <summary>
        /// Add array of AType to AArray.
        /// Also updates length, shape, rank and type informations
        /// </summary>
        /// <param name="items"></param>
        public static void AddRange(this AType array, IEnumerable<AType> items)
        {
            array.Container.AddRange(items);
            array.UpdateInfo();
        }

        public static void AddRangeWithNoUpdate(this AType array, IEnumerable<AType> items)
        {
            array.Container.AddRange(items);
        }

        /// <summary>
        /// Updates the array's length, shape, rank and type informations
        /// </summary>
        public static void UpdateInfo(this AType array)
        {
            array.Length = array.Container.Count;

            array.Shape.Clear();
            array.Shape.Add(array.Container.Count);

            if (array.Length > 0)
            {
                // Update Type info from children
                array.Type = array.Container[0].Type;

                // Update Shape info
                if (array.Container[0].IsArray)
                {
                    array.Shape.AddRange(array.Container[0].Shape);
                }
            }

            array.Rank = array.Shape.Count;
        }

        /// <summary>
        /// Update AArray's infos from an other AArray
        /// </summary>
        /// <param name="argument">Input AArray</param>
        public static void UpdateInfo(this AType array, AType argument)
        {
            array.Length = argument.Length;
            array.Shape.Clear();
            array.Shape.AddRange(argument.Shape);
            array.Type = argument.Type;
            array.Rank = argument.Rank;
        }

        public static bool IsSlotFiller(this AType vector, bool extended = false)
        {
            //Vector must be box array.
            if (!vector.IsArray)
            {
                return false;
            }

            AType ravelVector = vector.Rank > 1 ? MonadicFunctionInstance.Ravel.Execute(vector) : vector;

            //Form (symbol;value)
            if (vector.Length != 2)
            {
                return false;
            }

            //Symbol and value must be box.
            if (!ravelVector[0].IsBox || !ravelVector[0].IsBox)
            {
                return false;
            }

            AType symbol = MonadicFunctionInstance.Disclose.Execute(ravelVector[0]);
            AType value =  MonadicFunctionInstance.Disclose.Execute(ravelVector[1]); 

            // calculate symbols' size
            int symbolLength = CalculateLength(symbol, extended);

            // calculate values' size
            int valueLength = CalculateLength(value, extended);

            //symbol and value has same length!
            if (symbolLength != valueLength)
            {
                return false;
            }

            if (symbol.Type != ATypes.ASymbol)
            {
                return false;
            }

            if (!extended)
            {
                if (!symbol.SimpleSymbolArray())
                {
                    return false;
                }

                //If symbol is array, it must consist of distinct symbol.
                if (symbol.IsArray)
                {
                    foreach (AType item in symbol)
                    {
                        if (symbol.Container.FindAll(a => { return a.CompareTo(item) == 0; }).Count != 1)
                        {
                            return false;
                        }
                    }
                }
            }

            if (extended)
            {
                return value.Type == ATypes.ABox || value.Type == ATypes.AFunc;
            }
            else
            {
                if (!value.IsBox)
                {
                    return false;
                }

                if (value.IsArray)
                {
                    //Value is a vector.
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (value[i].IsPrimitiveFunction())
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    return !value.IsPrimitiveFunction();
                }
            }
        }

        private static int CalculateLength(AType argument, bool extended)
        {
            int argumentLength = 1;

            if (argument.Rank > 1)
            {
                if (extended)
                {
                    foreach (int item in argument.Shape)
                    {
                        argumentLength *= item;
                    }
                }
                else
                {
                    argumentLength = -1;
                }
            }
            else
            {
                argumentLength = argument.Length;
            }

            return argumentLength;
        }

        // TODO: make it to handle all ATypes not just array
        /// <summary>
        /// Converts array's elements to float
        /// </summary>
        public static AType ConvertToFloat(this AType array)
        {
            // TODO: no array type check
            array.Type = ATypes.AFloat;

            for (int i = 0; i < array.Length; i++)
            {
                if (array.Container[i].Type != ATypes.AInteger)
                {
                    continue;
                }

                if (array.Container[i].IsArray)
                {
                    array.Container[i] = Utils.ConvertToFloat(array.Container[i]);
                }
                else
                {
                    array.Container[i] = AFloat.Create(array.Container[i].asFloat);
                }
            }

            return array;
        }

        public static bool EqualsWithTolerance(this AType lhs, AType rhs)
        {
            if (!rhs.IsArray)
            {
                return false;
            }
            AType left = lhs;
            AType right = rhs;

            bool numbers = left.IsNumber && right.IsNumber;
            // Check if the inner elem count matches and the type is the same
            if ((left.Container.Count != right.Container.Count) ||
                // We treat AInteger and AFloat as equal
                (!numbers && (left.Type != right.Type)))
            {
                return false;
            }

            // Check items one-by-one
            for (int i = 0; i < left.Container.Count; i++)
            {

                if (numbers)
                {
                    // both are arrays check them with tolerance
                    if (left.Container[i].IsArray && right.Container[i].IsArray
                        && !(Utils.EqualsWithTolerance(left.Container[i], right[i]))
                        )
                    {
                        return false;
                    }
                    // do a tolerance check if it is a number
                    else if (!Utils.ComparisonTolerance(left.Container[i].asFloat, right[i].asFloat))
                    {
                        return false;
                    }
                }
                // Otherwise a simple equality check
                else if (left.Container[i].Equals(right.Container[i]))
                {
                    return false;
                }

            }

            return true;
        }

        #region Type

        /// <summary>
        /// General SimpleArray type checker method.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static bool SimpleArray(this AType argument)
        {
            return argument.IsBox ? argument.MixedSimpleArray() : true;

        }

        /// <summary>
        /// If argument made up by function scalar and symbol the result is true.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static bool MixedSimpleArray(this AType argument)
        {
            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    if (!MixedSimpleArray(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return argument.IsFunctionScalar || argument.SimpleSymbolArray();
            }
        }

        public static bool SimpleSymbolArray(this AType argument)
        {
            return !argument.IsBox && argument.Type == ATypes.ASymbol;
        }

        /// <summary>
        /// Return true, if all elements are nested!
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static bool NestedArray(this AType argument)
        {
           if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    if (!NestedArray(item))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return argument.IsBox && !argument.IsFunctionScalar;
            }
        }

        public static bool MixedType(this AType argument)
        {
            return MixedType(argument.Type);
        }

        public static bool MixedType(this ATypes argument)
        {
            return argument == ATypes.ABox || argument == ATypes.AFunc || argument == ATypes.ASymbol;
        }

        public static bool IsPrimitiveFunction(this AType argument)
        {
            if (argument.IsFunctionScalar)
            {
                AFunc function = (AFunc)argument.NestedItem.Data;
                return function.IsBuiltin;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region Type tests

        /// <summary>
        /// Check if the arguments are of the same general type
        /// </summary>
        /// <remarks>
        /// 3 checks preformed, if any of these are true then the result is also true.
        /// a) both arguments are of number types
        /// b) both arguments are of character types
        /// c) both arguments are of any of the mixed types
        /// 
        /// Mixed types: Function, Box, Symbol
        /// </remarks>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsSameGeneralType(AType left, AType right)
        {
            if (left.IsNumber && right.IsNumber)
            {
                return true;
            }
            else if (left.Type == ATypes.AChar && right.Type == ATypes.AChar)
            {
                return true;
            }
            else if (MixedType(left) && MixedType(right))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
