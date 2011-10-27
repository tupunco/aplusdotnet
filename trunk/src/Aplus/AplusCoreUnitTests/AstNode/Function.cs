using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Compiler.AST;
using AplusCore.Compiler.Grammar;

namespace AplusCoreUnitTests.AstNode
{
    [TestClass]
    public class Function
    {
        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void MonadicFunction1()
        {
            string input = "+*%- 1 2 3 4.0";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Monadic Parsing FAILED!");

            ConstantList floatList = Node.ConstantList(Node.IntConstant("1"));
            floatList.AddLast(Node.IntConstant("2"));
            floatList.AddLast(Node.IntConstant("3"));
            floatList.AddLast(Node.FloatConstant("4.0"));

            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.MonadicFunction(Node.Token(Tokens.IDENTITY),
                    Node.MonadicFunction(Node.Token(Tokens.SIGN),
                        Node.MonadicFunction(Node.Token(Tokens.RECIPROCAL),
                            Node.MonadicFunction(Node.Token(Tokens.NEGATE),
                                floatList
                            )
                        )
                    )
                )
            ));

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void MonadicFunction2()
        {
            string input = "((((+))))*5";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Monadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.MonadicFunction(
                    Node.Token(Tokens.IDENTITY,"+"),
                    Node.MonadicFunction(
                        Node.Token(Tokens.SIGN),
                        Node.ConstantList(
                            Node.IntConstant("5")
                        )
                    )
                )
            ));

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void MonadicFunction3()
        {
            string input = "|{-76}";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Monadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.MonadicFunction(
                        Node.Token(Tokens.ABSOLUTEVALUE, "|"),
                        Node.ConstantList(
                            Node.IntConstant("-76")
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void MonadicFunction4()
        {
            string input = "(iota 3 4)[1; 1 3]";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Monadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.Indexing(
                        Node.MonadicFunction(
                            Node.Token(Tokens.INTERVAL, "iota"),
                            Node.ConstantList(
                                Node.IntConstant("3"),
                                Node.IntConstant("4")
                            )
                        ),
                        Node.ExpressionList(
                            Node.ConstantList(
                                Node.IntConstant("1")
                            ),
                            Node.ConstantList(
                                Node.IntConstant("1"),
                                Node.IntConstant("3")
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void DyadicFunction1()
        {
            string input = "1 * 2 - 4 pi 3";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.DyadicFunction(Node.Token(Tokens.MULTIPLY),
                    Node.ConstantList(Node.IntConstant("1")),
                    Node.DyadicFunction(Node.Token(Tokens.SUBTRACT),
                        Node.ConstantList(Node.IntConstant("2")),
                        Node.DyadicFunction(Node.Token(Tokens.CIRCLE),
                            Node.ConstantList(Node.IntConstant("4")),
                            Node.ConstantList(Node.IntConstant("3"))
                        )
                    )
                )
            ));

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void DyadicFunction2()
        {
            string input = "(3 ,2) < (3,6) + 1 8";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.DyadicFunction(
                        Node.Token(Tokens.LT),
                        Node.DyadicFunction(
                            Node.Token(Tokens.CATENATE),
                            Node.ConstantList(
                                Node.IntConstant("3")
                            ),
                            Node.ConstantList(
                                Node.IntConstant("2")
                            )
                        ),
                        Node.DyadicFunction(
                            Node.Token(Tokens.ADD),
                            Node.DyadicFunction(
                                Node.Token(Tokens.CATENATE),
                                Node.ConstantList(
                                    Node.IntConstant("3")
                                ),
                                Node.ConstantList(
                                    Node.IntConstant("6")
                                )
                            ),
                            Node.ConstantList(
                            Node.IntConstant("1"),
                            Node.IntConstant("8")
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void DyadicFunction3()
        {
            string input = " {g} >= 2 * %{2}";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.DyadicFunction(
                        Node.Token(Tokens.GTE),
                        Node.ExpressionList(
                            Node.Identifier("g",IdentifierType.UnQualifiedName)
                        ),
                        Node.DyadicFunction(
                            Node.Token(Tokens.MULTIPLY),
                            Node.ConstantList(
                                Node.IntConstant("2")
                            ),
                            Node.MonadicFunction(
                                Node.Token(Tokens.RECIPROCAL),
                                Node.ConstantList(
                                    Node.IntConstant("2")
                                )
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void DyadicFunction4()
        {
            string input = "*{(2,3);b[iota 2]}";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.DyadicFunction(
                        Node.Token(Tokens.MULTIPLY),
                        Node.DyadicFunction(
                            Node.Token(Tokens.CATENATE),
                            Node.ConstantList(
                                Node.IntConstant("2")
                            ),
                            Node.ConstantList(
                                Node.IntConstant("3")
                            )
                        ),
                        Node.Indexing(
                            Node.Identifier("b",IdentifierType.UnQualifiedName),
                            Node.ExpressionList(
                                Node.MonadicFunction(
                                    Node.Token(Tokens.INTERVAL),
                                    Node.ConstantList(
                                        Node.IntConstant("2")
                                    )
                                )
                            )
                        )
                    )
                )
            );

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void MiscellaneousFunction()
        {
            string input = "4 + + + 5";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.DyadicFunction(
                    Node.Token(Tokens.ADD,"+"),
                    Node.ConstantList(
                        Node.IntConstant("4")
                    ),
                    Node.MonadicFunction(
                        Node.Token(Tokens.IDENTITY,"+"),
                        Node.MonadicFunction(
                            Node.Token(Tokens.IDENTITY,"+"),
                            Node.ConstantList(
                                Node.IntConstant("5")
                            )
                        )
                    )
                )
            ));

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void GeneralFunctionOnLeftTest()
        {
            string input = "+{1;2} - 3";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic Parsing FAILED!");

            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.DyadicFunction(
                    Node.Token(Tokens.SUBTRACT, "-"),
                    Node.DyadicFunction(Node.Token(Tokens.ADD),
                        Node.ConstantList(Node.IntConstant("1")),
                        Node.ConstantList(Node.IntConstant("2"))
                    ),
                    Node.ConstantList(Node.IntConstant("3"))
                )
            ));

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void UserDefinedFunctionCall()
        {
            string input = "f{a.g;'abc'[1];(3.4;_h)}";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.UserDefInvoke(
                        Node.Identifier("f",IdentifierType.UnQualifiedName),
                        Node.ExpressionList(
                            Node.Identifier("a.g",IdentifierType.QualifiedName),
                            Node.Indexing(
                                Node.SingeQuotedConstant("abc"),
                                Node.ExpressionList(
                                    Node.ConstantList(
                                        Node.IntConstant("1")
                                    )
                                )
                            ),
                            Node.Strand(
                                Node.ConstantList(
                                    Node.FloatConstant("3.4")
                                ),
                                Node.Identifier("_h", IdentifierType.SystemName)
                            )
                        )
                    )
                )
            );

            Assert.IsTrue(parser.Parse(), "Dyadic Parsing FAILED!");
            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void DyadicUserDefInfixInvoke()
        {
            string input = "1 f 2";
            AplusParser parser = TestUtils.BuildASCIIParser(input);
            parser.FunctionInfo = new AplusCore.Compiler.FunctionInformation(".");
            parser.FunctionInfo.RegisterGlobalFunction(".f");

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.UserDefInvoke(
                        Node.Identifier("f", IdentifierType.UnQualifiedName),
                        Node.ExpressionList(
                            Node.ConstantList(Node.IntConstant("2")),
                            Node.ConstantList(Node.IntConstant("1"))
                        )
                    )
                )
            );

            Assert.IsTrue(parser.Parse(), "Parsing of infix invocation of user defined function failed");
            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Function AST Node tests"), TestMethod]
        public void MonadicUserDefInfixInvoke()
        {
            string input = "f 2";
            AplusParser parser = TestUtils.BuildASCIIParser(input);
            parser.FunctionInfo = new AplusCore.Compiler.FunctionInformation(".");
            parser.FunctionInfo.RegisterGlobalFunction(".f");

            Node expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.UserDefInvoke(
                        Node.Identifier("f", IdentifierType.UnQualifiedName),
                        Node.ExpressionList(
                            Node.ConstantList(Node.IntConstant("2"))
                        )
                    )
                )
            );

            Assert.IsTrue(parser.Parse(), "Parsing of infix invocation of user defined function failed");
            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }
    }
}
