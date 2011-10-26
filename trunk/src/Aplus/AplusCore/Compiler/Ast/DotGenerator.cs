using System;
using System.IO;
using System.Text;

using Antlr.Runtime;

namespace AplusCore.Compiler.AST
{
    public static class DotGenerator
    {
        #region Variables

        private static int counter = 0;
        private static StringBuilder text = new StringBuilder();

        #endregion

        #region Entry Point

        public static bool CreateDotFile(string input, string outputFileName)
        {
            Antlr.Runtime.Lexer lexer = new Grammar.Ascii.AplusLexer(new ANTLRStringStream(input ?? ""));
            Grammar.AplusParser parser = new Grammar.AplusParser(new CommonTokenStream(lexer));

            bool parseOk = parser.Parse();

            if (parseOk)
            {
                CreateDotFile(parser.Tree, outputFileName);
            }

            return parseOk;
        }

        public static void CreateDotFile(Node input, string outputFileName)
        {
            text.Append("digraph APlus {\n");
            ToDot("", input);
            text.Append("\n}");

            using (StreamWriter writer = new StreamWriter(new FileStream(outputFileName, FileMode.Create)))
            {
                writer.Write(text);
            }
        }

        #endregion

        #region Node converters

        private static string ToDot(string parent, Node node)
        {
            Type type = node.GetType();

            if (type == typeof(Assign))
            {
                return ToDot(parent, (Assign)node);
            }
            else if (type == typeof(BuiltInFunction))
            {
                return ToDot(parent, (BuiltInFunction)node);
            }
            else if (type == typeof(BuiltInOperator))
            {
                return ToDot(parent, (BuiltInOperator)node);
            }
            else if (type == typeof(Case))
            {
                return ToDot(parent, (Case)node);
            }
            else if (type == typeof(Constant))
            {
                return ToDot(parent, (Constant)node);
            }
            else if (type == typeof(ConstantList))
            {
                return ToDot(parent, (ConstantList)node);
            }
            else if (type == typeof(Dependency))
            {
                return ToDot(parent, (Dependency)node);
            }
            else if (type == typeof(DyadicDo))
            {
                return ToDot(parent, (DyadicDo)node);
            }
            else if (type == typeof(DyadicFunction))
            {
                return ToDot(parent, (DyadicFunction)node);
            }
            else if (type == typeof(EachOperator))
            {
                return ToDot(parent, (EachOperator)node);
            }
            else if (type == typeof(ExpressionList))
            {
                return ToDot(parent, (ExpressionList)node);
            }
            else if (type == typeof(Identifier))
            {
                return ToDot(parent, (Identifier)node);
            }
            else if (type == typeof(If))
            {
                return ToDot(parent, (If)node);
            }
            else if (type == typeof(Indexing))
            {
                return ToDot(parent, (Indexing)node);
            }
            else if (type == typeof(MonadicDo))
            {
                return ToDot(parent, (MonadicDo)node);
            }
            else if (type == typeof(MonadicFunction))
            {
                return ToDot(parent, (MonadicFunction)node);
            }
            else if (type == typeof(RankOperator))
            {
                return ToDot(parent, (RankOperator)node);
            }
            else if (type == typeof(Strand))
            {
                return ToDot(parent, (Strand)node);
            }
            else if (type == typeof(SystemCommand))
            {
                return ToDot(parent, (SystemCommand)node);
            }
            else if (type == typeof(Token))
            {
                return ToDot(parent, (Token)node);
            }
            else if (type == typeof(UserDefFunction))
            {
                return ToDot(parent, (UserDefFunction)node);
            }
            else if (type == typeof(UserDefInvoke))
            {
                return ToDot(parent, (UserDefInvoke)node);
            }
            else if (type == typeof(While))
            {
                return ToDot(parent, (While)node);
            }


            throw new Exception("Type");
        }

        private static string ToDot(string parent, Assign node)
        {
            string thisID = String.Format("Assign{0}", counter++);
            string targetID = ToDot(thisID, node.Target);
            string expressionID = ToDot(thisID, node.Expression);

            text.AppendFormat("  {0} [label=\":=\"];\n", thisID);
            text.AppendFormat("  {0} -> {1};\n", thisID, targetID);
            text.AppendFormat("  {0} -> {1};\n", thisID, expressionID);

            return thisID;
        }

        private static string ToDot(string parent, BuiltInFunction node)
        {
            string name = String.Format("BuiltIn{0}", counter++);

            text.AppendFormat(" {0} [label={1}];\n", name, node.Function);

            return name;
        }

        private static string ToDot(string parent, BuiltInOperator node)
        {
            string name = String.Format("BuiltIn{0}", counter++);

            text.AppendFormat(" {0} [label={1}];\n", name, ToDot(name, node.Operator));

            return name;
        }

        private static string ToDot(string parent, Case node)
        {
            string name = String.Format("Case{0}", counter++);
            string exprName = ToDot(name, node.Expression);
            string casesName = ToDot(name, node.CaseList);

            text.AppendFormat("  {0} [label=\"Case\"];\n", name);
            text.AppendFormat("  {0} -> {1};\n", name, exprName);
            text.AppendFormat("  {0} -> {1};\n", name, casesName);

            return name;
        }

        private static string ToDot(string parent, Constant node)
        {
            string name = String.Format("Constant{0}", counter++);
            text.AppendFormat("  {0} [label=\"{1}\"];\n", name, node.AsString);
            return name;
        }

        private static string ToDot(string parent, ConstantList node)
        {
            string name = String.Format("ConstantList{0}", counter++);
            foreach (Constant item in node.List)
            {
                string itemName = ToDot(name, item);
                text.AppendFormat("  {0} -> {1};\n", name, itemName);
            }
            return name;
        }

        private static string ToDot(string parent, Dependency node)
        {
            string name = String.Format("Dependency{0}", counter++);

            if (node.IsItemwise)
            {
                text.AppendFormat("  subgraph cluster_{0}_cond {{ style=dotted; color=lightgrey; label=\"Parameters\";\n", name);
                string parametersName = ToDot(name, node.Indexer);
                text.AppendFormat("  }}\n");
                text.AppendFormat("  {0} -> {1};\n", name, parametersName);
            }

            text.AppendFormat("  subgraph cluster_{0}_true {{ style=dotted; color=lightgrey; label=\"CodeBlock\";\n", name);
            string codeblockName = ToDot(name, node.FunctionBody);
            text.AppendFormat("  }}\n");
            text.AppendFormat("  {0} -> {1};\n", name, codeblockName);

            return name;
        }

        private static string ToDot(string parent, DyadicDo node)
        {
            string name = String.Format("DyadicDo{0}", counter++);
            text.AppendFormat("  subgraph cluster_{0}_cond {{ style=dotted; color=blue; label=\"Condition\";\n", name);
            string exprName = ToDot(name, node.Expression);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  subgraph cluster_{0}_code {{ style=dotted; color=black; label=\"Code Block\";\n", name);
            string codeBlockName = ToDot(name, node.Codeblock);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  {0} [label=\"DO\"];\n", name);
            text.AppendFormat("  {0} -> {1};\n", name, exprName);
            text.AppendFormat("  {0} -> {1};\n", name, codeBlockName);


            return name;
        }

        private static string ToDot(string parent, DyadicFunction node)
        {
            string name = String.Format("Dyadic{0}", counter++);
            string leftName = ToDot(name, node.Left);
            string rightName = ToDot(name, node.Right);

            text.AppendFormat("  {0} [label=\"{1} ({2})\"];\n", name, node.Token.Text, node.TokenType);
            text.AppendFormat("  {0} -> {1};\n", name, leftName);
            text.AppendFormat("  {0} -> {1};\n", name, rightName);

            return name;
        }

        private static string ToDot(string parent, EachOperator node)
        {
            string name = String.Format("EachOP{0}", counter++);
            string funcDot = ToDot(name, node.Function);
            text.AppendFormat(" {0} -> {1};\n", name, funcDot);

            if (node.isDyadic)
            {
                string leftArg = ToDot(name, node.LeftArgument);
                text.AppendFormat("  {0} -> {1};\n", name, leftArg);
            }

            if (node.RightArgument != null)
            {
                string rightArg = ToDot(name, node.RightArgument);
                text.AppendFormat(" {0} -> {1};\n", name, rightArg);
                text.AppendFormat(" {0} [label=\"{1} Each\"]", name, node.Function);
            }

            return name;
        }

        private static string ToDot(string parent, RankOperator node)
        {
            string name = String.Format("RankOP{0}", counter++);
            string funcDot = ToDot(name, node.Function);
            text.AppendFormat(" {0} -> {1};\n", name, funcDot);

            if (node.isDyadic)
            {
                string leftArg = ToDot(name, node.LeftArgument);
                text.AppendFormat("  {0} -> {1};\n", name, leftArg);
            }
            string conditionArg = ToDot(name, node.Condition);
            text.AppendFormat("  {0} -> {1};\n", name, conditionArg);

            if (node.RightArgument != null)
            {
                string rightArg = ToDot(name, node.RightArgument);
                text.AppendFormat(" {0} -> {1};\n", name, rightArg);
                text.AppendFormat(" {0} [label=\"{1} Rank\"]", name, node.Function);
            }

            return name;
        }

        private static string ToDot(string parent, ExpressionList node)
        {
            string name = String.Format("ExpressionList{0}", counter++);

            foreach (Node item in node.Items)
            {
                string nodeName = ToDot(name, item);
                text.AppendFormat("  {0} -> {1};\n", name, nodeName);
            }

            return name;
        }

        private static string ToDot(string parent, Identifier node)
        {
            string name = String.Format("ID{0}", counter++);
            text.AppendFormat("  {0} [label=\"{1}\"];\n", name, node.Name);
            return name;
        }

        private static string ToDot(string parent, If node)
        {
            string name = String.Format("IF{0}", counter++);

            text.AppendFormat("  subgraph cluster_{0}_cond {{ style=dotted; color=blue; label=\"Condition\";\n", name);
            string exprName = ToDot(name, node.Expression);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  subgraph cluster_{0}_true {{ style=dotted; color=green; label=\"True\";\n", name);
            string trueCaseName = ToDot(name, node.TrueCase);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  {0} -> {1};\n", name, exprName);
            text.AppendFormat("  {0} -> {1};\n", name, trueCaseName);

            if (node.HaveFalseCase)
            {
                text.AppendFormat("  {0} [label=\"IF-Else\"];\n", name);
                text.AppendFormat("  subgraph cluster_{0}_false {{ style=dotted; color=red; label=\"False\";\n", name);
                string falseCaseName = ToDot(name, node.FalseCase);
                text.AppendFormat("  }}\n");
                text.AppendFormat("  {0} -> {1};\n", name, falseCaseName);
            }
            else
            {
                text.AppendFormat("  {0} [label=\"IF\"];\n", name);
            }

            return name;
        }

        private static string ToDot(string parent, Indexing node)
        {
            string name = String.Format("Indexing{0}", counter++);
            string itemName = ToDot(name, node.Item);
            string indexExprName = ToDot(name, node.IndexExpression);

            text.AppendFormat("  {0} [label=\"Indexing\"];", name);
            text.AppendFormat("  {0} -> {1};\n", name, itemName);
            text.AppendFormat("  {0} -> {1};\n", name, indexExprName);

            return name;
        }

        private static string ToDot(string parent, MonadicDo node)
        {
            string name = String.Format("MonadicDo{0}", counter++);
            text.AppendFormat("  subgraph cluster_{0}_block {{ style=dotted; color=black; label=\"Protected Block\";\n", name);
            string codeBlockName = ToDot(name, node.Codeblock);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  {0} [label=\"DO\"];\n", name);
            text.AppendFormat("  {0} -> {1};\n", name, codeBlockName);


            return name;
        }

        private static string ToDot(string parent, MonadicFunction node)
        {
            string name = String.Format("Monadic{0}", counter++);
            string exprName = ToDot(name, node.Expression);

            text.AppendFormat("  {0} [label=\"{1} ({2})\"];\n", name, node.Token.Text, node.TokenType);
            text.AppendFormat("  {0} -> {1};\n", name, exprName);

            return name;
        }

        private static string ToDot(string parent, Strand node)
        {
            string name = String.Format("Strand{0}", counter++);
            foreach (Node argument in node.Items)
            {
                string itemName = ToDot(name, argument);
                text.AppendFormat("  {0} -> {1};\n", name, itemName);
            }
            return name;
        }

        private static string ToDot(string parent, SystemCommand node)
        {
            string name = String.Format("SystemCommand{0}", counter++);
            text.AppendFormat("  {0}_cmd [label=\"{1}\"];\n", name, node.Command);
            text.AppendFormat("  {0}_arg [label=\"{1}\"];\n", name, node.Argument);
            text.AppendFormat("  {0} -> {0}_cmd;\n", name);
            text.AppendFormat("  {0} -> {0}_arg;\n", name);

            return name;
        }

        private static string ToDot(string parent, Token node)
        {
            string name = String.Format("Token{0}", counter++);
            text.AppendFormat(" {0} [label=\"{1}\"]", name, node);
            return node.ToString();
        }

        private static string ToDot(string parent, UserDefFunction node)
        {
            string name = String.Format("FunctionDef{0}", counter++);

            text.AppendFormat("  subgraph cluster_{0}_cond {{ style=dotted; color=lightgrey; label=\"Parameters\";\n", name);
            string parametersName = ToDot(name, node.Parameters);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  subgraph cluster_{0}_true {{ style=dotted; color=lightgrey; label=\"CodeBlock\";\n", name);
            string codeblockName = ToDot(name, node.Codeblock);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  {0} -> {1};\n", name, parametersName);
            text.AppendFormat("  {0} -> {1};\n", name, codeblockName);

            return name;
        }

        private static string ToDot(string parent, UserDefInvoke node)
        {
            string name = String.Format("FunctionInvoke{0}", counter++);

            text.AppendFormat("  {0} [label=\"Invoke: {1}\"]", name, node.Method.Name);

            text.AppendFormat("  subgraph cluster_{0}_args {{ style=dotted; color=lightgrey; label=\"Arguments\";\n", name);
            string argumentsName = ToDot(name, node.Arguments);
            text.AppendFormat("  }}\n");

            text.AppendFormat("  {0} -> {1};\n", name, argumentsName);

            return name;
        }

        private static string ToDot(string parent, While node)
        {
            string name = String.Format("While{0}", counter++);
            string exprName = ToDot(name, node.Expression);
            string codeBlockName = ToDot(name, node.CodeBlock);

            text.AppendFormat("  {0} [label=\"While\"];\n", name);
            text.AppendFormat("  {0} -> {1};\n", name, exprName);
            text.AppendFormat("  {0} -> {1};\n", name, codeBlockName);

            return name;

        }

        #endregion
    }
}
