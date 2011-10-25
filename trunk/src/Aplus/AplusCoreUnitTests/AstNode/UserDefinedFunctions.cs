using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Compiler.AST;
using AplusCore.Compiler.Grammar;

namespace AplusCoreUnitTests.AstNode
{
    [TestClass]
    public class UserDefinedFunctionsTest
    {
        [TestCategory("AstNode"), TestCategory("User Defined Function tests"), TestMethod]
        public void UserDefOneLineA()
        {
            string line = "f{}: 5";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Function Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.UserDefFunction(
                    Node.UnQualifiedName("f"),
                    Node.ExpressionList(),
                    Node.ConstantList(Node.IntConstant("5"))
                )
            );

            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Function tests"), TestMethod]
        public void UserDefOneLineB()
        {
            string line = "f{}: \n5";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Function Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.UserDefFunction(
                    Node.UnQualifiedName("f"),
                    Node.ExpressionList(),
                    Node.ConstantList(Node.IntConstant("5"))
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Function tests"), TestMethod]
        public void UserDefMultiLineA()
        {
            string line = "f{a;b}: { 1 2\n3;\n'bello'\n\n}";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Function Parsing FAILED!");

            #region expected AST

            ExpressionList expectedTree = Node.ExpressionList(
                Node.UserDefFunction(
                    Node.UnQualifiedName("f"),
                    Node.ExpressionList(Node.UnQualifiedName("a"), Node.UnQualifiedName("b")),
                    Node.ExpressionList(
                        Node.ConstantList(Node.IntConstant("1"), Node.IntConstant("2"), Node.IntConstant("3")),
                        Node.SingeQuotedConstant("bello")
                    )
                )
            );

            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Function tests"), TestMethod]
        public void UserDefMultiLineB()
        {
            string line = "f{\nc\n}: -c";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Function Parsing FAILED!");

            #region expected AST

            ExpressionList expectedTree = Node.ExpressionList(
                Node.UserDefFunction(
                    Node.UnQualifiedName("f"),
                    Node.ExpressionList(Node.UnQualifiedName("c")),
                    Node.MonadicFunction(Node.Token(Tokens.NEGATE, "-"), Node.UnQualifiedName("c"))
                )
            );

            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Function tests"), TestMethod]
        public void UserDefDyadicForm()
        {
            string line = "a f b: 11";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Function Parsing FAILED!");

            #region expected AST

            ExpressionList expectedTree = Node.ExpressionList(
                Node.UserDefFunction(
                    Node.UnQualifiedName("f"),
                    Node.ExpressionList(Node.UnQualifiedName("a"), Node.UnQualifiedName("b")),
                    Node.ConstantList(Node.IntConstant("11"))
                )
            );

            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Function tests"), TestMethod]
        public void UserDefMonadicForm()
        {
            string line = "f b: 11";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Function Parsing FAILED!");

            #region expected AST

            ExpressionList expectedTree = Node.ExpressionList(
                Node.UserDefFunction(
                    Node.UnQualifiedName("f"),
                    Node.ExpressionList(Node.UnQualifiedName("b")),
                    Node.ConstantList(Node.IntConstant("11"))
                )
            );

            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Function tests"), TestMethod]
        public void InfixUserDefinitionandInvoke()
        {
            string line = "a f b: { a + b }\n 1 f 2";
            AplusParser parser = TestUtils.BuildASCIIParser(line);
            parser.FunctionInfo = new AplusCore.Compiler.FunctionInformation(".");

            Node expectedTree = Node.ExpressionList(
                Node.UserDefFunction(
                    Node.Identifier("f", IdentifierType.UnQualifiedName),
                    Node.ExpressionList(
                        Node.UnQualifiedName("a"),
                        Node.UnQualifiedName("b")
                    ),
                    Node.ExpressionList(
                        Node.DyadicFunction(
                            Node.Token(Tokens.ADD, "+"),
                            Node.UnQualifiedName("a"),
                            Node.UnQualifiedName("b")
                        )
                    )
                ),
                Node.ExpressionList(
                    Node.UserDefInvoke(
                        Node.UnQualifiedName("f"),
                        Node.ExpressionList(
                            Node.ConstantList(Node.IntConstant("2")),
                            Node.ConstantList(Node.IntConstant("1"))
                        )
                    )
                )
            );

            Assert.IsTrue(parser.Parse(), "User Defined Function Parsing FAILED!");
            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }
    }
}
