using Microsoft.VisualStudio.TestTools.UnitTesting;

using Antlr.Runtime;

using AplusCore.Compiler.AST;
using AplusCore.Compiler.Grammar;
using AplusCore.Compiler.Grammar.Ascii;

namespace AplusCoreUnitTests.AstNode
{
    [TestClass]
    public class ConstantTest
    {
        private AplusLexer lexer;
        private AplusParser parser;


        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void IntConstantTest()
        {
            string input = "1";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Integer input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(Node.IntConstant(input))
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(new Constant(input, ConstantType.Integer))
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void IntConstantListTest()
        {
            string input = "2 5 6 [2]";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Indexed int list parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                   Node.Indexing(
                       Node.ConstantList(
                            Node.IntConstant("2"),
                            Node.IntConstant("5"),
                            Node.IntConstant("6")
                        ),
                        Node.ExpressionList(
                            Node.ConstantList(
                                Node.IntConstant("2")
                            )
                        )
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.Tree, "Incorrect AST generated!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void FloatConstantTest()
        {
            string input = "1.3";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Float input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(Node.FloatConstant(input))
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(new Constant(input, ConstantType.Double))
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void FloatConstantListTest()
        {
            string input = "-3. .12 -2.2 4e2 -2.1e2 4.1e+4 .2.2.23.4 Inf -Inf";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Float list parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                   Node.ConstantList(
                       Node.FloatConstant("-3."),
                       Node.FloatConstant(".12"),
                       Node.FloatConstant("-2.2"),
                       Node.FloatConstant("4e2"),
                       Node.FloatConstant("-2.1e2"),
                       Node.FloatConstant("4.1e+4"),
                       Node.FloatConstant(".2"),
                       Node.FloatConstant(".2"),
                       Node.FloatConstant(".23"),
                       Node.FloatConstant(".4"),
                       Node.InfConstant("Inf"),
                       Node.InfConstant("-Inf")
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.Tree, "Incorrect AST generated!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void InfinityConstantTest()
        {
            string input = "Inf -Inf Inf";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Float input parse FAILED");

            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(
                        Node.InfConstant("Inf"),
                        Node.InfConstant("-Inf"),
                        Node.InfConstant("Inf")
                   )
                )
            );

            Assert.AreEqual(expectedTree, this.parser.Tree, "Invalid Node created! (Helper method used)");

            Assert.IsTrue(this.parser.Tree == expectedTree, "Operator == Compare Failed!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void SymbolConstantTest()
        {
            string input = "`test";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Symbol input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(Node.SymbolConstant("test"))
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                    Node.ConstantList(new Constant("test", ConstantType.Symbol))
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void SymbolConstantListTest()
        {
            string input = "`see1_2.1 `_2.2 `";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Symbol constant list parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                   Node.ConstantList(
                        Node.SymbolConstant("see1_2.1"),
                        Node.SymbolConstant("_2.2"),
                        Node.SymbolConstant("")
                    )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.Tree, "Incorrect AST generated!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void SingeQuotedConstantTest()
        {
            string input = "'te''st'";
            string expectedText = "te'st";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Singe quoted constant input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.SingeQuotedConstant(expectedText)
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                // for Single quoted characters remove the leading and trailing '
                    new Constant(expectedText, ConstantType.CharacterConstant)
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void SingeQuotedKanaKanjiConstantTest()
        {
            string input = "'こんにちは世界'";
            string expectedText = "こんにちは世界";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Singe quoted constant input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.SingeQuotedConstant(expectedText)
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                // for Single quoted characters remove the leading and trailing '
                    new Constant(expectedText, ConstantType.CharacterConstant)
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }


        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void DoubleQuotedKanaKanjiConstantTest()
        {
            string input = "\"こんにちは世界\"";
            string expectedText = "こんにちは世界";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Double quoted constant input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.DoubleQuotedConstant(expectedText)
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void DoubleQuotedConstantTest()
        {
            string input = "\"@tes\\11st\"";
            string expectedText = "@tes\tst";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Double quoted constant input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.DoubleQuotedConstant(expectedText)
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void NullConstantTest1()
        {
            string input = "";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Null constant input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.NullConstant()
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                    new Constant("", ConstantType.Null)
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void NullConstantTest2()
        {
            string input = "()";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Null constant input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.NullConstant()
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                    new Constant("", ConstantType.Null)
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
         public void NullConstantTest3()
        {
            string input = "\t(\t ) ";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Null constant input parse FAILED");

            ExpressionList expectedNodeA = Node.ExpressionList(
                Node.ExpressionList(
                    Node.NullConstant()
                )
            );
            ExpressionList expectedNodeB = Node.ExpressionList(
                Node.ExpressionList(
                    new Constant("", ConstantType.Null)
                )
            );

            Assert.AreEqual(expectedNodeA, this.parser.Tree, "Invalid Node created! (Helper method used)");
            Assert.AreEqual(expectedNodeB, this.parser.Tree, "Invalid Node created!");

            Assert.IsTrue(this.parser.Tree == expectedNodeA, "Operator == Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void OperatorEqualsTest()
        {
            ExpressionList left = Node.ExpressionList(
                Node.ConstantList(Node.SymbolConstant("`int"))
                );
            left.AddFirst(Node.ConstantList(Node.FloatConstant("1.3")));
            left.AddFirst(Node.ConstantList(Node.IntConstant("2")));

            ExpressionList right = Node.ExpressionList(
                Node.ConstantList(Node.SymbolConstant("`int"))
                );
            right.AddFirst(Node.ConstantList(Node.FloatConstant("1.3")));
            right.AddFirst(Node.ConstantList(Node.IntConstant("2")));

            Assert.IsTrue(left == right, "Operator == Compare Failed!");
            Assert.IsFalse(left != right, "Operator != Compare Failed!");
        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void StrandTest1()
        {
            string input = "(;`3s`ss1`122[1];-Inf;_g;a.b[0 1];c;)";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Strand parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                   Node.Strand(
                        Node.NullConstant(),
                        Node.Indexing(
                            Node.ConstantList(
                                Node.SymbolConstant("3s"),
                                Node.SymbolConstant("ss1"),
                                Node.SymbolConstant("122")
                            ),
                            Node.ExpressionList(
                                Node.ConstantList(
                                    Node.IntConstant("1")
                                )
                            )
                        ),
                        Node.ConstantList(
                            Node.InfConstant("-Inf")
                        ),
                        Node.Identifier("_g", IdentifierType.SystemName),
                        Node.Indexing(
                            Node.Identifier("a.b", IdentifierType.QualifiedName),
                            Node.ExpressionList(
                                Node.ConstantList(
                                    Node.IntConstant("0"),
                                    Node.IntConstant("1")
                                )
                            )
                        ),
                        Node.Identifier("c", IdentifierType.UnQualifiedName),
                        Node.NullConstant()
                   )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.Tree, "Incorrect AST generated!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void StrandTest2()
        {
            string input = "(3;'test';;`11DA`AVV;(\"test\";6.7,8);)";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Strand parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                   Node.Strand(
                        Node.ConstantList(Node.IntConstant("3")),
                        Node.SingeQuotedConstant("test"),
                        Node.NullConstant(),
                        Node.ConstantList(
                            Node.SymbolConstant("11DA"),
                            Node.SymbolConstant("AVV")
                        ),
                        Node.Strand(
                            Node.DoubleQuotedConstant("test"),
                            Node.DyadicFunction(
                                Node.Token(Tokens.CATENATE, ","),
                                Node.ConstantList(
                                    Node.FloatConstant("6.7")
                                ),
                                Node.ConstantList(
                                    Node.IntConstant("8")
                                )
                            )
                        ),
                        Node.NullConstant()
                   )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.Tree, "Incorrect AST generated!");

        }

        [TestCategory("AstNode"), TestCategory("Basic AST Node testing"), TestMethod]
        public void MultiLineStrandTest()
        {
            string input = "(3;\n'hello')";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            Assert.IsTrue(this.parser.Parse(), "Strand parsing FAILED!");

            #region expected AST
            ExpressionList expectedTree = Node.ExpressionList(
                Node.ExpressionList(
                   Node.Strand(
                        Node.ConstantList(Node.IntConstant("3")),
                        Node.SingeQuotedConstant("hello")
                   )
                )
            );
            #endregion

            Assert.AreEqual(expectedTree, this.parser.Tree, "Incorrect AST generated!");
        }

        /// <summary>
        /// Tests bug found in ANTLR's generated Lexer
        /// </summary>
        /// <remarks>
        /// Bug Description:
        ///   The ANTLR generates incorrect code for the following rule:
        ///      " ( ~" )* "
        ///      
        ///   in the generated code the middle part's matching won't accept the 'S' character.
        ///   It seems that the cutpoint in the ascii table is somehow calculated incorrectly
        ///   inside the ANTLR. 
        ///   
        /// Workaround for bug:
        ///   Use the following rule:
        ///     " ( S | ~" )* "
        ///     
        /// Dirtiness level: LOL...
        /// Workaround level: YUCK... (but works:) )
        /// </remarks>
        [TestCategory("AstNode"), TestCategory("Basic AST Node Testing"), TestMethod]
        public void ANTLRLexerBugTest()
        {
            string input = "\"ABCDEFGHIJKLMONPRSTUVWYZ abcdefghijklmnopqrstuvwyz 0123456789\"";
            string expectedStr = "ABCDEFGHIJKLMONPRSTUVWYZ abcdefghijklmnopqrstuvwyz 0123456789";
            this.lexer = new AplusLexer(new ANTLRStringStream(input));
            this.parser = new AplusParser(new CommonTokenStream(lexer));

            // If this throws an exception (MismatchedSetException) then we ran into the bug..
            Assert.IsTrue(this.parser.Parse(), "ANTLR Bug test failed parsing");
            
            ExpressionList expectedTree =
                Node.ExpressionList(
                    Node.ExpressionList(
                        Node.DoubleQuotedConstant(expectedStr)
                    )
                );

            Assert.AreEqual(expectedTree, this.parser.Tree, "Incorrect AST generated!");
        }

    }
}
