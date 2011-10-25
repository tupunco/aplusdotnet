using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Compiler.AST;
using AplusCore.Compiler.Grammar;

namespace AplusCoreUnitTests.AstNode
{
    [TestClass]
    public class ControlFlowTests
    {
        [TestCategory("AstNode"), TestCategory("Control Flow Tests"), TestMethod]
        public void DyadicDoTest()
        {
            string line = "1+2 do { 3;4 }";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "Dyadic Do Parsing FAILED!");

            #region expected AST
            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.DyadicFunction(Node.Token(Tokens.ADD),
                    Node.ConstantList(Node.IntConstant("1")),
                    Node.DyadicDo(
                        Node.ConstantList(Node.IntConstant("2")),
                        Node.ExpressionList(
                            Node.ConstantList(Node.IntConstant("3")),
                            Node.ConstantList(Node.IntConstant("4"))
                        )
                    )
                )
            ));
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Control Flow Tests"), TestMethod]
        public void MonadicDoTest()
        {
            string line = "do { 3;4 }";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "Monadic Do Parsing FAILED!");

            #region expected AST
            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.MonadicDo(
                    Node.ExpressionList(
                        Node.ConstantList(Node.IntConstant("3")),
                        Node.ConstantList(Node.IntConstant("4"))
                    )
                )
            ));
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Control Flow Tests"), TestMethod]
        public void ConditionalTestA()
        {
            string line = "1 + if x <7 else 11";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "Conditional Parsing FAILED!");

            #region expected AST
            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.DyadicFunction(Node.Token(Tokens.ADD,"+"),
                    Node.ConstantList(Node.IntConstant("1")),
                    Node.IfElse(
                        Node.UnQualifiedName("x"),
                        Node.MonadicFunction(Node.Token(Tokens.ENCLOSE,"<"), Node.ConstantList(Node.IntConstant("7"))),
                        Node.ConstantList(Node.IntConstant("11"))
                    )
                )
            ));
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Control Flow Tests"), TestMethod]
        public void ConditionalTestB()
        {
            string line = "1 + if (x <7) else 11";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "Conditional Parsing FAILED!");

            #region expected AST
            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.DyadicFunction(Node.Token(Tokens.ADD),
                    Node.ConstantList(Node.IntConstant("1")),
                    Node.IfElse(
                        Node.DyadicFunction(Node.Token(Tokens.LT),
                            Node.UnQualifiedName("x"),
                            Node.ConstantList(Node.IntConstant("7"))
                        ),
                        Node.NullConstant(),
                        Node.ConstantList(Node.IntConstant("11"))
                    )
                )
            ));
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Control Flow Tests"), TestMethod]
        public void WhileTestA()
        {
            string line = "while x <7";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "Conditional Parsing FAILED!");

            #region expected AST
            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.While(
                    Node.UnQualifiedName("x"),
                    Node.MonadicFunction(
                        Node.Token(Tokens.ENCLOSE,"<"),
                        Node.ConstantList(Node.IntConstant("7"))
                    )
                )
            ));
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Control Flow Tests"), TestMethod]
        public void WhileTestB()
        {
            string line = "while (x <7)";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "Conditional Parsing FAILED!");

            #region expected AST
            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.While(
                    Node.DyadicFunction(Node.Token(Tokens.LT),
                        Node.UnQualifiedName("x"),
                        Node.ConstantList(Node.IntConstant("7"))
                    ),
                    Node.NullConstant()
                )
            ));
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Control Flow Tests"), TestMethod]
        public void CaseTestA()
        {
            string line = "case (2) {1;2;3;4;}";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "Conditional Parsing FAILED!");

            #region expected AST
            Node expectedTree = Node.ExpressionList(Node.ExpressionList(
                Node.Case(
                    Node.ConstantList(Node.IntConstant("2")),
                    Node.ExpressionList(
                        Node.ConstantList(Node.IntConstant("1")),
                        Node.ConstantList(Node.IntConstant("2")),
                        Node.ConstantList(Node.IntConstant("3")),
                        Node.ConstantList(Node.IntConstant("4")),
                        Node.NullConstant()
                    )
                )
            ));
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }
    }
}
