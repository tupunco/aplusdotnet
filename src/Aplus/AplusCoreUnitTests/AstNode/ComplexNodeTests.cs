using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Globalization;
using Antlr.Runtime;
using AplusCore.Compiler.Grammar;
using AplusCore.Compiler.AST;
using AplusCore.Compiler;
using AplusCore.Compiler.Grammar.Ascii;

namespace AplusCoreUnitTests.AstNode
{
    [TestClass]
    public class ComplexNodeTests
    {
        private AplusLexer lexer;
        private AplusParser parser;

        [TestCategory("AstNode"), TestCategory("Complex AST Node tests"), TestMethod]
        public void ExpressionListTestA()
        {
            string input = "+{2;(4 5)}; -{12}; { 1;3;\n4;5}";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Expression List Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.DyadicFunction(
                        Node.Token(Tokens.ADD,"+"),
                        Node.ConstantList(Node.IntConstant("2")),
                        Node.ConstantList(Node.IntConstant("4"), Node.IntConstant("5"))
                    ),
                    Node.MonadicFunction(
                        Node.Token(Tokens.NEGATE,"-"),
                        Node.ConstantList(Node.IntConstant("12"))
                    ),
                    Node.ExpressionList(
                        Node.ConstantList(Node.IntConstant("1")),
                        Node.ConstantList(Node.IntConstant("3")),
                        Node.ConstantList(Node.IntConstant("4")),
                        Node.ConstantList(Node.IntConstant("5"))
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.tree, "Incorrect AST generated!");

        }

        [TestCategory("AstNode"), TestCategory("Complex AST Node tests"), TestMethod]
        public void ExpressionListTestB()
        {
            string input = "+{(2);4 5}; -{(12)}; { 1;3;\n4;5}";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Expression List Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.DyadicFunction(
                        Node.Token(Tokens.ADD,"+"),
                        Node.ConstantList(Node.IntConstant("2")),
                        Node.ConstantList(Node.IntConstant("4"), Node.IntConstant("5"))
                    ),
                    Node.MonadicFunction(
                        Node.Token(Tokens.NEGATE,"-"),
                        Node.ConstantList(Node.IntConstant("12"))
                    ),
                    Node.ExpressionList(
                        Node.ConstantList(Node.IntConstant("1")),
                        Node.ConstantList(Node.IntConstant("3")),
                        Node.ConstantList(Node.IntConstant("4")),
                        Node.ConstantList(Node.IntConstant("5"))
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.tree, "Incorrect AST generated!");

        }

        [TestCategory("AstNode"), TestCategory("Complex AST Node tests"), TestMethod]
        //[Ignore] // TODO: This test runs really really slow!! fix?
        public void VeryComplexTest()
        {
            string input = "3;c:={ {10[10][11][12]} + 1 6 -2 +. 4 10 3 +. 4 5 6 + 11 + c:=(+5;1);1+2};{(;;2;;);1;}";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Expression List Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree =
            Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(Node.IntConstant("3")),
            #region assign
                    Node.Assign(
                        Node.UnQualifiedName("c"),
                        Node.ExpressionList(
                            Node.DyadicFunction(Node.Token(Tokens.ADD,"+"),
                                Node.ExpressionList(
                                    Node.Indexing(
                                        Node.Indexing(
                                            Node.Indexing(
                                                Node.ConstantList(Node.IntConstant("10")),
                                                Node.ExpressionList(
                                                    Node.ConstantList(Node.IntConstant("10"))
                                                )
                                            ),
                                            Node.ExpressionList(
                                                Node.ConstantList(Node.IntConstant("11"))
                                            )
                                        ),
                                        Node.ExpressionList(
                                            Node.ConstantList(Node.IntConstant("12"))
                                        )
                                     )
                                ),
                                Node.DyadicFunction(Node.Token(Tokens.OPADD,"+."),
                                    Node.ConstantList(Node.IntConstant("1"), Node.IntConstant("6"), Node.IntConstant("-2")),
                                    Node.DyadicFunction(Node.Token(Tokens.OPADD,"+."),
                                        Node.ConstantList(Node.IntConstant("4"), Node.IntConstant("10"), Node.IntConstant("3")),
                                        Node.DyadicFunction(Node.Token(Tokens.ADD,"+"),
                                            Node.ConstantList(Node.IntConstant("4"), Node.IntConstant("5"), Node.IntConstant("6")),
                                            Node.DyadicFunction(Node.Token(Tokens.ADD,"+"),
                                                Node.ConstantList(Node.IntConstant("11")),
                                                Node.Assign(
                                                    Node.UnQualifiedName("c"),
                                                    Node.Strand(
                                                        Node.MonadicFunction(
                                                            Node.Token(Tokens.IDENTITY),
                                                            Node.ConstantList(Node.IntConstant("5"))
                                                        ),
                                                        Node.ConstantList(Node.IntConstant("1"))
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            ),
                            Node.DyadicFunction(Node.Token(Tokens.ADD,"+"),
                                  Node.ConstantList(Node.IntConstant("1")),
                                  Node.ConstantList(Node.IntConstant("2"))
                            )
                        )
                    ),
            #endregion
                    Node.ExpressionList(
                        Node.Strand(
                            Node.NullConstant(),
                            Node.NullConstant(),
                            Node.ConstantList(Node.IntConstant("2")),
                            Node.NullConstant(),
                            Node.NullConstant()
                        ),
                        Node.ConstantList(Node.IntConstant("1")),
                        Node.NullConstant()
                    )
                )
            );

            #endregion

            Assert.AreEqual(expectedTree, this.parser.tree, "Incorrect AST generated!");

        }
    }
}
