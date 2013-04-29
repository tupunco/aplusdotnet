using System.IO;
using System.Text;

using Antlr.Runtime;

namespace AplusCore.Compiler
{
    public enum LexerMode
    {
        ASCII,
        APL,
        UNI,
    }
    
    public class Parse
    {
        private static Encoding Latin1 = Encoding.GetEncoding(28591);

        public static AST.Node ASCIIString(string input, FunctionInformation functionInfo)
        {
            Antlr.Runtime.Lexer lexer = new Grammar.Ascii.AplusLexer(new ANTLRStringStream(input ?? ""));
            Grammar.AplusParser parser = new Grammar.AplusParser(new CommonTokenStream(lexer));
            parser.FunctionInfo = functionInfo;

            bool parseOk = parser.Parse();
            AST.Node tree = parser.Tree;

            return tree;
        }

        public static AST.Node APLString(string input, FunctionInformation functionInfo)
        {
            Antlr.Runtime.Lexer lexer = new Grammar.Apl.AplusLexer(new ANTLRStringStream(input ?? ""));
            Grammar.AplusParser parser = new Grammar.AplusParser(new CommonTokenStream(lexer));
            parser.FunctionInfo = functionInfo;

            bool parseOk = parser.Parse();
            return parser.Tree;
        }

        public static AST.Node UNIString(string input, FunctionInformation functionInfo)
        {
            Antlr.Runtime.Lexer lexer = new Grammar.Uni.AplusLexer(new ANTLRStringStream(input ?? ""));
            Grammar.AplusParser parser = new Grammar.AplusParser(new CommonTokenStream(lexer));
            parser.FunctionInfo = functionInfo;

            bool parseOk = parser.Parse();
            return parser.Tree;
        }

        public static AST.Node String(string input, LexerMode mode, FunctionInformation functionInfo)
        {
            switch (mode)
            {
                case LexerMode.ASCII:
                    return ASCIIString(input, functionInfo);

                case LexerMode.APL:
                    return APLString(input, functionInfo);

                case LexerMode.UNI:
                    return UNIString(input, functionInfo);

                default:
                    break;
            }

            throw new ParseException("Invalid Parse Mode");
        }

        public static AST.Node LoadFile(string fileName, LexerMode mode, FunctionInformation functionInfo)
        {
            using (StreamReader file = new StreamReader(fileName, Parse.Latin1))
            {
                return Parse.String(file.ReadToEnd(), mode, functionInfo);
            }
        }
    }
}
