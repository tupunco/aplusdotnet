using System;
using AplusCore.Runtime;

using DLR = System.Linq.Expressions;
using System.Text;
using AplusCore.Types;
using System.Collections.Generic;
using System.Globalization;
using AplusCore.Compiler.Grammar;

namespace AplusCore.Compiler.AST
{
    public enum ConstantType
    {
        Undefined = 0,
        Integer,
        Double,
        PositiveInfinity,
        NegativeInfinity,
        Symbol,
        CharacterConstant,
        Null
    }

    public class Constant : Node
    {
        #region Variables

        private string value;
        private ConstantType type;

        #endregion

        #region Properties

        public ConstantType Type { get { return this.type; } }
        public object Value { get { return this.value; } }

        public int AsInt { get { return Int32.Parse(value); } }

        /// <summary>
        /// Returns the numbers as an AInteger or an AFloat.
        /// AFloat will be returned if the value can not be represented as an integer.
        /// </summary>
        public AType AsNumericAType
        {
            get
            {
                int number;
                if (Int32.TryParse(this.value, out number))
                {
                    return AInteger.Create(number);
                }
                else
                {
                    return AFloat.Create(this.AsFloat);
                }
            }
        }

        public string AsString { get { return this.value; } }

        public double AsFloat
        {
            get
            {
                switch (this.Type)
                {
                    case ConstantType.PositiveInfinity:
                        return Double.PositiveInfinity;
                    case ConstantType.NegativeInfinity:
                        return Double.NegativeInfinity;
                    default:
                        return Double.Parse(value, CultureInfo.InvariantCulture);
                }
            }
        }

        #endregion

        #region Constructor

        public Constant(string value, ConstantType type)
        {
            this.value = value;
            this.type = type;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            // NOTE: do we need to generate this? most Constants will be encapsulated in ConstantList
            switch (this.type)
            {
                case ConstantType.CharacterConstant:
                    return DLR.Expression.Constant(Runtime.Helpers.BuildString(this.AsString));

                case ConstantType.Null:
                    return DLR.Expression.Constant(AArray.ANull());

                case ConstantType.Integer:
                    return DLR.Expression.Constant(this.AsNumericAType);

                case ConstantType.NegativeInfinity:
                case ConstantType.PositiveInfinity:
                case ConstantType.Double:
                    return DLR.Expression.Constant(AFloat.Create(this.AsFloat));

                case ConstantType.Symbol:
                    return DLR.Expression.Constant(ASymbol.Create(this.AsString));

                case ConstantType.Undefined:
                default:
                    // This Should NEVER happen
                    break;

            }
            throw new Exception("Should Not reach this point!..");
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Constant({0})", this.value);

        }

        public override bool Equals(object obj)
        {
            if (obj is Constant)
            {
                Constant other = (Constant)obj;
                return this.value.Equals(other.value) && (this.type == other.type);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.type.GetHashCode() ^ this.value.GetHashCode();
        }

        #endregion

        #region GraphViz output (Only in DEBUG)
#if DEBUG
        static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder text)
        {
            string name = String.Format("Constant{0}", counter++);
            text.AppendFormat("  {0} [label=\"{1}\"];\n", name, this.value);
            return name;
        }
#endif
        #endregion

    }

    #region Construction helper

    public partial class Node
    {

        public static Constant IntConstant(string value)
        {
            return new Constant(StringProcessor.ProcessAPLNumber(value), ConstantType.Integer);
        }

        public static Constant FloatConstant(string value)
        {
            return new Constant(StringProcessor.ProcessAPLNumber(value), ConstantType.Double);
        }

        /// <summary>
        /// Creates an infinite constant based on the first character of the input
        /// </summary>
        /// <remarks>
        /// Because this needs to work in APL mode also, we check the length of the input value.
        /// If it is above 3 then it is the negative infinity.
        /// </remarks>
        /// <param name="value">input: '-Inf' or 'Inf'</param>
        /// <returns>negative or positive infinite constant</returns>
        public static Constant InfConstant(string value)
        {
            // Check if value.Length > "Inf".Length
            if (value.Length > 3) // Magic Number: 3 is the length of 'Inf' 
            {
                return new Constant(StringProcessor.ProcessAPLNumber(value), ConstantType.NegativeInfinity);
            }
            else
            {
                return new Constant(StringProcessor.ProcessAPLNumber(value), ConstantType.PositiveInfinity);
            }
        }

        public static Constant SymbolConstant(string value)
        {
            string processedSymbol = value.StartsWith("`") 
                ? value.Substring(1, value.Length - 1)
                : value;
            return new Constant(processedSymbol, ConstantType.Symbol);
        }

        public static Constant SingeQuotedConstant(string text)
        {
            // Remove the leading and trailing single quotes (if there is any)
            string processedText = (text.StartsWith("'") && text.EndsWith("'"))
                ? text.Substring(1, text.Length - 2)
                : text;
            return new Constant(processedText.Replace("''", "'"), ConstantType.CharacterConstant);
        }

        public static Constant DoubleQuotedConstant(string text)
        {
            // Remove the leading and trailing double quotes (if there is any)
            string processedText = (text.StartsWith("\"") && text.EndsWith("\""))
                ? text.Substring(1, text.Length - 2)
                : text;
            return new Constant(StringProcessor.ProcessEscapes(processedText), ConstantType.CharacterConstant);
        }

        public static Constant NullConstant()
        {
            return new Constant("", ConstantType.Null);
        }

    }

    #endregion
}
