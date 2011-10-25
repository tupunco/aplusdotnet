using Antlr.Runtime;

using AplusCore.Compiler.Grammar;
using AplusCore.Compiler.Grammar.Ascii;

namespace AplusCoreUnitTests.AstNode
{
    public static class TestUtils
    {
        /// <summary>
        /// Builds an ASCII parser for the given input
        /// </summary>
        /// <param name="input">Input string to parse.</param>
        /// <returns>An <see cref="AplusParser"/>.</returns>
        public static AplusParser BuildASCIIParser(string input)
        {
            AplusLexer lexer = new AplusLexer(new ANTLRStringStream(input));
            return new AplusParser(new CommonTokenStream(lexer));
        }

        /// <summary>
        /// Builds an APL parser for the given input
        /// </summary>
        /// <param name="input">Input string to parse.</param>
        /// <returns>An <see cref="AplusParser"/>.</returns>
        public static AplusParser BuildAPLParser(string input)
        {
            AplusLexer lexer = new AplusLexer(new ANTLRStringStream(input));
            return new AplusParser(new CommonTokenStream(lexer));
        }
    }
}
