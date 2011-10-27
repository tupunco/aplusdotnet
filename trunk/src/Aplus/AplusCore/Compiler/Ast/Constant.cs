using System;
using System.Globalization;

using AplusCore.Compiler.Grammar;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Specifies the constant type in the A+ AST.
    /// </summary>
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

    /// <summary>
    /// Represents a constant in an A+ AST.
    /// </summary>
    public class Constant : Node
    {
        #region Variables

        private string value;
        private ConstantType type;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.Constant; }
        }

        /// <summary>
        /// Gets the <see cref="ConstantType"/> the node represents.
        /// </summary>
        public ConstantType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the value of the constant as an integer.
        /// </summary>
        public int AsInt
        {
            get { return Int32.Parse(value); }
        }

        /// <summary>
        /// Gets the value of the constant as an <see cref="AType"/> number.
        /// </summary>
        /// <remarks>
        /// Returns the numbers as an AInteger or an AFloat.
        /// AFloat will be returned if the value can not be represented as an integer.
        /// </remarks>
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

        /// <summary>
        /// Gets the value of the constant node as a string.
        /// </summary>
        public string AsString
        {
            get { return this.value; }
        }

        /// <summary>
        /// Gets the value of the constant node as a double.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of <see cref="Constant"/> AST node.
        /// </summary>
        /// <param name="value">The value of the constant.</param>
        /// <param name="type">The type of the constant.</param>
        public Constant(string value, ConstantType type)
        {
            this.value = value;
            this.type = type;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;
            // NOTE: do we need to generate this? most Constants will be encapsulated in ConstantList
            switch (this.type)
            {
                case ConstantType.CharacterConstant:
                    result = DLR.Expression.Constant(Runtime.Helpers.BuildString(this.AsString));
                    break;

                case ConstantType.Null:
                    result = DLR.Expression.Constant(Utils.ANull());
                    break;

                case ConstantType.Integer:
                    result = DLR.Expression.Constant(this.AsNumericAType);
                    break;

                case ConstantType.NegativeInfinity:
                case ConstantType.PositiveInfinity:
                case ConstantType.Double:
                    result = DLR.Expression.Constant(AFloat.Create(this.AsFloat));
                    break;

                case ConstantType.Symbol:
                    result = DLR.Expression.Constant(ASymbol.Create(this.AsString));
                    break;

                case ConstantType.Undefined:
                default:
                    // This Should NEVER happen
                    throw new Exception("Should Not reach this point!..");
            }

            return result;
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
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Build an integer constant.
        /// </summary>
        /// <param name="value">The integer value as a string</param>
        /// <returns>Returns an <see cref="AST.Constant"/> with a <see cref="ConstantType.Integer"/>.</returns>
        public static Constant IntConstant(string value)
        {
            return new Constant(StringProcessor.ProcessAPLNumber(value), ConstantType.Integer);
        }

        /// <summary>
        /// Build a float constant.
        /// </summary>
        /// <param name="value">The float value as a string</param>
        /// <returns>Returns an <see cref="AST.Constant"/> with a <see cref="ConstantType.Double"/>.</returns>
        public static Constant FloatConstant(string value)
        {
            return new Constant(StringProcessor.ProcessAPLNumber(value), ConstantType.Double);
        }

        /// <summary>
        /// Creates an infinite constant based on the first character of the input.
        /// </summary>
        /// <remarks>
        /// Because this needs to work in APL mode also, we check the length of the input value.
        /// If it is above 3 then it is the negative infinity.
        /// </remarks>
        /// <param name="value">input: '-Inf' or 'Inf'</param>
        /// <returns>
        /// Returns an <see cref="AST.Constant"/> with a
        /// <see cref="ConstantType.NegativeInfinity"/> or <see cref="ConstantType.PositiveInfinity"/>.
        /// </returns>
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

        /// <summary>
        /// Build a symbol constant.
        /// </summary>
        /// <remarks>
        /// The leading ` (backtick) will be removed if found.
        /// </remarks>
        /// <param name="value">The symbol value as a string.</param>
        /// <returns>Returns an <see cref="AST.Constant"/> with a <see cref="ConstantType.Symbol"/>.</returns>
        public static Constant SymbolConstant(string value)
        {
            string processedSymbol = value.StartsWith("`")
                ? value.Substring(1, value.Length - 1)
                : value;
            return new Constant(processedSymbol, ConstantType.Symbol);
        }

        /// <summary>
        /// Build a character constant from a single qouted string.
        /// </summary>
        /// <remarks>
        /// The leading and trailing single qoutes will be removed.
        /// </remarks>
        /// <param name="text">The text constant.</param>
        /// <returns>Returns an <see cref="AST.Constant"/> with a <see cref="ConstantType.CharacterConstant"/>.</returns>
        public static Constant SingeQuotedConstant(string text)
        {
            // Remove the leading and trailing single quotes (if there is any)
            string processedText = (text.StartsWith("'") && text.EndsWith("'"))
                ? text.Substring(1, text.Length - 2)
                : text;
            return new Constant(processedText.Replace("''", "'"), ConstantType.CharacterConstant);
        }

        /// <summary>
        /// Build a character constant from a double qouted string.
        /// </summary>
        /// <remarks>
        /// The leading and trailing double qoutes will be removed.
        /// </remarks>
        /// <param name="text">The text constant.</param>
        /// <returns>Returns an <see cref="AST.Constant"/> with a <see cref="ConstantType.CharacterConstant"/>.</returns>
        public static Constant DoubleQuotedConstant(string text)
        {
            // Remove the leading and trailing double quotes (if there is any)
            string processedText = (text.StartsWith("\"") && text.EndsWith("\""))
                ? text.Substring(1, text.Length - 2)
                : text;
            return new Constant(StringProcessor.ProcessEscapes(processedText), ConstantType.CharacterConstant);
        }

        /// <summary>
        /// Build a null constant.
        /// </summary>
        /// <returns>Returns an <see cref="AST.Constant"/> with a <see cref="ConstantType.Null"/>.</returns>
        public static Constant NullConstant()
        {
            return new Constant("", ConstantType.Null);
        }
    }

    #endregion
}
