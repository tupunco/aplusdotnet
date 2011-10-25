using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Compiler.AST;
using AplusCore.Compiler.Grammar;

namespace AplusCoreUnitTests.AstNode
{
    [TestClass]
    public class Operator
    {
        [TestCategory("AstNode"), TestCategory("Operator AST Node tests"), TestMethod]
        public void monadicOperatorTest1()
        {
            string input = "+/ 5 6 , */ 45 6";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Monadic operator Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.MonadicFunction(
                        Node.Token(Tokens.RADD, "+/"),
                        Node.DyadicFunction(
                            Node.Token(Tokens.CATENATE, ","),
                            Node.ConstantList(
                                Node.IntConstant("5"),
                                Node.IntConstant("6")
                            ),
                            Node.MonadicFunction(
                                Node.Token(Tokens.RMULTIPLY, "*/"),
                                Node.ConstantList(
                                    Node.IntConstant("45"),
                                    Node.IntConstant("6")
                                )
                            )
                        )
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Operator AST Node tests"), TestMethod]
        public void monadicEachOperatorTest1()
        {
            string input = "((((log)) each 5 7))";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Monadic operator Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.EachOperator(
                        Node.Token(Tokens.NATURALLOG,"log"),
                        Node.ConstantList(
                            Node.IntConstant("5"),
                            Node.IntConstant("7")
                        )
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Operator AST Node tests"), TestMethod]
        public void monadicEachOperatorTest2()
        {
            string input = "| each {(5;-2.7;-Inf)}";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Monadic operator Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.EachOperator(
                        Node.Token(Tokens.ABSOLUTEVALUE, "|"),
                        Node.Strand(
                            Node.ConstantList(
                            Node.IntConstant("5")
                            ),
                            Node.ConstantList(
                                Node.FloatConstant("-2.7")
                            ),
                            Node.ConstantList(
                                Node.InfConstant("-Inf")
                            )
                        )
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Operator AST Node tests"), TestMethod]
        public void dyadicEachOperatorTest1()
        {
            string input = "3 ((f[0])each) 7";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic operator Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.EachOperator(
                        Node.Indexing(
                            Node.Identifier("f",IdentifierType.UnQualifiedName),
                            Node.ExpressionList(
                                Node.ConstantList(
                                    Node.IntConstant("0")
                                )
                            )
                        ),
                        Node.ConstantList(
                            Node.IntConstant("3")
                        ),
                        Node.ConstantList(
                            Node.IntConstant("7")
                        )
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }

        [TestCategory("AstNode"), TestCategory("Operator AST Node tests"), TestMethod]
        public void dyadicEachOperatorTest2()
        {
            string input = "(3;4) <= each (8;1)";
            AplusParser parser = TestUtils.BuildASCIIParser(input);

            Assert.IsTrue(parser.Parse(), "Dyadic operator Parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.EachOperator(
                        Node.Token(Tokens.LTE,"<="),
                        Node.Strand(
                            Node.ConstantList(
                                Node.IntConstant("3")
                            ),
                            Node.ConstantList(
                                Node.IntConstant("4")
                            )
                        ),
                        Node.Strand(
                            Node.ConstantList(
                                Node.IntConstant("8")
                            ),
                            Node.ConstantList(
                                Node.IntConstant("1")
                            )                            
                        )
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, parser.Tree, "Incorrect AST generated!");
        }
    }
}
