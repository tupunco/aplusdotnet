using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Other
{
    class BitwiseCast : AbstractDyadicFunction
    {
        #region Delegates

        private delegate AType ConverterFunction(AType argument);

        #endregion

        #region Static members

        private static ASCIIEncoding encoding = new ASCIIEncoding();

        #endregion

        #region Constants

        private static readonly int symbolSize = 4;
        private static readonly int charSize = 1;

        #endregion

        #region Computation

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            AType typeToCast;

            if (!left.TryFirstScalar(out typeToCast, true))
            {
                throw new Error.Domain(this.DomainErrorText);
            }

            int sourceTypeSize = GetTypeSize(right.Type);

            ATypes typeToConvert;
            ConverterFunction converterFunction;
            int destinationTypeSize;

            switch (typeToCast.asString)
            {
                case "int":
                    converterFunction = ConvertToInt;
                    typeToConvert = ATypes.AInteger;
                    break;
                case "float":
                    converterFunction = ConvertToFloat;
                    typeToConvert = ATypes.AFloat;
                    break;
                case "char":
                    converterFunction = ConvertToChar;
                    typeToConvert = ATypes.AChar;
                    break;
                default:
                    throw new Error.Domain(this.DomainErrorText);
            }
            
            destinationTypeSize = GetTypeSize(typeToConvert);
            int convertSizeRatio;
            double shapeModifier;

            if (sourceTypeSize >= destinationTypeSize)
            {
                convertSizeRatio = sourceTypeSize / destinationTypeSize;
                shapeModifier = sourceTypeSize / destinationTypeSize;
            }
            else
            {
                convertSizeRatio = destinationTypeSize / sourceTypeSize;

                // check if the last dimension of the right argument is convertable to the type specified in the left argument
                if (right.Shape.Count == 0 || right.Shape[right.Shape.Count - 1] % convertSizeRatio != 0)
                {
                    throw new Error.Length(this.LengthErrorText);
                }

                shapeModifier = destinationTypeSize / sourceTypeSize;
            }

            AType result;

            if (right.Shape.Contains(0))
            {
                List<int> desiredShape = new List<int>(right.Shape);
                int length = desiredShape.Count - 1;
                desiredShape[length] = (int)(desiredShape[length] * shapeModifier);
                AType reshapeShape = desiredShape.ToAArray();

                result = Utils.FillElement(typeToConvert, reshapeShape);
            }
            else
            {
                result = converterFunction(right);
            }

            return result;
        }

        private AType ConvertToChar(AType argument)
        {
            AType result;

            switch (argument.Type)
            {
                case ATypes.AInteger:
                    result = ConvertNumberToChar(argument);
                    break;
                case ATypes.AFloat:
                    result = ConvertNumberToChar(argument);
                    break;
                case ATypes.AChar:
                    result = argument.Clone();
                    break;
                case ATypes.ASymbol:
                    result = ConvertToChar(ConvertToInt(argument));
                    break;
                default:
                    throw new Error.Domain(this.DomainErrorText);
            }

            return result;
        }

        private AType ConvertNumberToChar(AType argument)
        {
            AType result = AArray.Create(ATypes.AChar);

            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    AType convertedItem = ConvertNumberToChar(item);

                    if (item.IsArray)
                    {
                        result.Add(convertedItem);
                    }
                    else
                    {
                        result.AddRange(convertedItem);
                    }
                }
            }
            else
            {
                byte[] bytes = (argument.Type == ATypes.AFloat)
                    ? BitConverter.GetBytes(argument.asFloat)
                    : BitConverter.GetBytes(argument.asInteger);

                result.AddRange(bytes.Select<byte, AType>(item => AChar.Create((char)item)));
            }

            return result;
        }

        private AType ConvertToInt(AType argument)
        {
            AType result;

            switch (argument.Type)
            {
                case ATypes.AInteger:
                    result = argument.Clone();
                    break;
                case ATypes.AFloat:
                    result = ConvertFloatToInt(argument);
                    break;
                case ATypes.AChar:
                    result = ConvertCharToInt(argument, false);
                    break;
                case ATypes.ASymbol:
                    result = ConvertSymToInt(argument);
                    break;
                default:
                    throw new Error.Domain(this.DomainErrorText);
            }

            return result;
        }

        private AType ConvertFloatToInt(AType argument)
        {
            AType result = AArray.Create(ATypes.AInteger);

            if (argument.IsArray)
            {
                foreach (AType item in argument)
                {
                    AType convertedItem = ConvertFloatToInt(item);

                    if (item.IsArray)
                    {
                        result.Add(convertedItem);
                    }
                    else
                    {
                        result.AddRange(convertedItem);
                    }
                }
            }
            else
            {
                int destinationTypeSize = GetTypeSize(ATypes.AInteger);
                byte[] bytes = BitConverter.GetBytes(argument.asFloat);

                for (int i = 0; i < bytes.Length; i += destinationTypeSize)
                {
                    result.Add(AInteger.Create(BitConverter.ToInt32(bytes, i)));
                }
            }

            return result;
        }

        private AType ConvertCharToInt(AType argument, bool multipleDimensions)
        {
            AType result;

            if (argument[0].IsArray)
            {
                result = AArray.Create(ATypes.AInteger);

                foreach (AType item in argument)
                {
                    result.Add(ConvertCharToInt(item, true));
                }
            }
            else
            {
                int length = argument.Length;
                int destinationTypeSize = GetTypeSize(ATypes.AInteger);
                byte[] bytes = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    bytes[i] = BitConverter.GetBytes(argument[i].asChar)[0];
                }

                if (length == destinationTypeSize && !multipleDimensions)
                {
                    result = AInteger.Create(BitConverter.ToInt32(bytes, 0));
                }
                else
                {
                    result = AArray.Create(ATypes.AInteger);

                    for (int i = 0; i < length; i += destinationTypeSize)
                    {
                        result.Add(AInteger.Create(BitConverter.ToInt32(bytes, i)));
                    }
                }
            }

            return result;
        }

        private AType ConvertSymToInt(AType argument)
        {
            AType result;

            if (argument.IsArray)
            {
                result = AArray.Create(ATypes.AInteger);

                foreach (AType item in argument)
                {
                    result.Add(ConvertSymToInt(item));
                }
            }
            else
            {
                result = AInteger.Create(argument.GetHashCode());
            }

            return result;
        }

        private AType ConvertToFloat(AType argument)
        {
            AType result;
            int typeSize = GetTypeSize(ATypes.AFloat);

            switch (argument.Type)
            {
                case ATypes.AInteger:
                    result = ConvertToFloat(argument, typeSize, false);
                    break;
                case ATypes.AFloat:
                    result = argument.Clone();
                    break;
                case ATypes.AChar:
                    result = ConvertToFloat(argument, typeSize, false);
                    break;
                case ATypes.ASymbol:
                    result = ConvertToFloat(ConvertToInt(argument));
                    break;
                default:
                    throw new Error.Domain(this.DomainErrorText);
            }

            return result;
        }

        private AType ConvertToFloat(AType argument, int destinationTypeSize, bool multipleDimensions)
        {
            AType result;

            if (argument[0].IsArray)
            {
                result = AArray.Create(ATypes.AFloat);

                foreach (AType item in argument)
                {
                    result.Add(ConvertToFloat(item, destinationTypeSize, true));
                }
            }
            else
            {
                int sourceTypeSize = GetTypeSize(argument.Type);
                int length = argument.Length;
                byte[] bytes = new byte[length * sourceTypeSize];

                for (int i = 0; i < length; i++)
                {
                    byte[] itemBytes = (argument[i].Type == ATypes.AInteger)
                        ? BitConverter.GetBytes(argument[i].asInteger)
                        : BitConverter.GetBytes(argument[i].asChar);

                    Array.Copy(itemBytes, 0, bytes, i * sourceTypeSize, sourceTypeSize);
                }

                if (length == destinationTypeSize / sourceTypeSize && !multipleDimensions)
                {
                    result = AFloat.Create(BitConverter.ToDouble(bytes, 0));
                }
                else
                {
                    result = AArray.Create(ATypes.AFloat);

                    for (int i = 0; i < length; i += destinationTypeSize)
                    {
                        result.Add(AFloat.Create(BitConverter.ToDouble(bytes, i)));
                    }
                }
            }

            return result;
        }

        private int GetTypeSize(ATypes argument)
        {
            int result;

            switch (argument)
            {
                case ATypes.AInteger:
                    result = sizeof(Int32);
                    break;
                case ATypes.AFloat:
                    result = sizeof(Double);
                    break;
                case ATypes.ASymbol:
                    result = symbolSize;
                    break;
                case ATypes.AChar:
                    result = charSize;
                    break;
                default:
                    throw new Error.Domain(this.DomainErrorText);
            }

            return result;
        }

        #endregion
    }
}
