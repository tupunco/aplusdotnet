using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Compiler.Grammar;
using AplusCore.Compiler.AST;
using AplusCore.Compiler;

namespace AplusCoreUnitTests.AstNode
{
    [TestClass]
    public class UserDefinedOperators
    {
        [TestCategory("AstNode"), TestCategory("User Defined Operator tests"), TestMethod]
        public void UserDefOperatorMonadicMonadic()
        {
            string line = "(f x) a :1";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Operator Parsing FAILED!");
            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.MonadicUserDefOperator(
                    Node.UnQualifiedName("x"),
                    Node.UnQualifiedName("f"),
                    Node.UnQualifiedName("a"),
                    Node.ConstantList(Node.IntConstant("1")),
                    line
                )
            );

            #endregion
            
            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Operator tests"), TestMethod]
        public void UserDefOperatorMonadicDyadic()
        {
            string line = "b (f x) a :1";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Operator Parsing FAILED!");

            #region expected AST

            ExpressionList expectedTree = Node.ExpressionList(
                Node.MonadicUserDefOperator(
                    Node.UnQualifiedName("x"),
                    Node.UnQualifiedName("f"),
                    Node.UnQualifiedName("b"), Node.UnQualifiedName("a"),
                    Node.ConstantList(Node.IntConstant("1")),
                    line
                )
            );

            #endregion
            
            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Operator tests"), TestMethod]
        public void UserDefOperatorDyadicMonadic()
        {
            string line = "(f x y) a :1";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Operator Parsing FAILED!");

            #region expected AST

            ExpressionList expectedTree = Node.ExpressionList(
                Node.DyadicUserDefOperator(
                    Node.UnQualifiedName("x"),
                    Node.UnQualifiedName("f"), Node.UnQualifiedName("y"),
                    Node.UnQualifiedName("a"),
                    Node.ConstantList(Node.IntConstant("1")),
                    line
                )
            );

            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Operator tests"), TestMethod]
        public void UserDefOperatorDyadicDyadic()
        {
            string line = "b (f x y) a :1";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsTrue(parser.Parse(), "User Defined Operator Parsing FAILED!");

            #region expected AST

            ExpressionList expectedTree = Node.ExpressionList(
                Node.DyadicUserDefOperator(
                    Node.UnQualifiedName("x"),
                    Node.UnQualifiedName("f"), Node.UnQualifiedName("y"),
                    Node.UnQualifiedName("b"), Node.UnQualifiedName("a"),
                    Node.ConstantList(Node.IntConstant("1")),
                    line
                )
            );

            #endregion
            
            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("User Defined Operator tests"), TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void UserDefOperatorIncorrect()
        {
            string line = "a (f x) : 1";
            AplusParser parser = TestUtils.BuildASCIIParser(line);

            Assert.IsFalse(parser.Parse(), "User Defined Operator Parsing should fail here");
        }
    }
}
