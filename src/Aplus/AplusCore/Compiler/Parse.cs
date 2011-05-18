using System;
using System.IO;
using Antlr.Runtime;
#if DEBUG
using System.Text;
#endif

namespace AplusCore.Compiler
{
    public enum LexerMode
    {
        ASCII,
        APL,
    }
    
    public class Parse
    {
        private static Encoding Latin1 = Encoding.GetEncoding(28591);

        public static AST.Node ASCIIString(string input)
        {
            Antlr.Runtime.Lexer lexer = new Grammar.Ascii.AplusLexer(new ANTLRStringStream(input ?? ""));
            Grammar.AplusParser parser = new Grammar.AplusParser(new CommonTokenStream(lexer));

            bool parseOk = parser.Parse();
            AST.Node tree = parser.tree;

            return tree;
        }

        public static AST.Node APLString(string input)
        {
            Antlr.Runtime.Lexer lexer = new Grammar.Apl.AplusLexer(new ANTLRStringStream(input ?? ""));
            Grammar.AplusParser parser = new Grammar.AplusParser(new CommonTokenStream(lexer));

            bool parseOk = parser.Parse();
            return parser.tree;
        }

        public static AST.Node String(string input, LexerMode mode)
        {
            switch (mode)
            {
                case LexerMode.ASCII:
                    return ASCIIString(input);

                case LexerMode.APL:
                    return APLString(input);

                default:
                    break;
            }

            throw new ParseException("Invalid Parse Mode");
        }

        public static AST.Node LoadFile(string fileName, LexerMode mode)
        {
            using (StreamReader file = new StreamReader(fileName, Parse.Latin1))
            {
                return Parse.String(file.ReadToEnd(), mode);
            }
        }

#if DEBUG
        public static void CreateDotFile(string input, string outputFileName)
        {
            Antlr.Runtime.Lexer lexer = new Grammar.Ascii.AplusLexer(new ANTLRStringStream(input ?? ""));
            Grammar.AplusParser parser = new Grammar.AplusParser(new CommonTokenStream(lexer));

            bool parseOk = parser.Parse();

            Console.WriteLine(parser.tree);
            Console.WriteLine("ok? {0}", parseOk);

            StringBuilder dotString = new StringBuilder();
            dotString.Append("digraph APlus {\n");
            parser.tree.ToDot("", dotString);
            dotString.Append("\n}");

            using (StreamWriter writer = new StreamWriter(new FileStream(outputFileName, FileMode.Create)))
            {
                writer.Write(dotString);
            }
        }
#endif
    }
}
