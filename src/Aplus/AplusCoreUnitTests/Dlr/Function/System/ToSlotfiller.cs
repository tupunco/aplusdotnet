using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.System
{
    [TestClass]
    public class ToSlotfiller : AbstractTest
    {
        #region CorrectTestCases

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void EmptyTest()
        {
            AType expected = AArray.Create(ATypes.ABox,
                                           ABox.Create(AArray.Create(ATypes.ANull)),
                                           ABox.Create(AArray.Create(ATypes.ANull))
                                           );

            AType result = this.engine.Execute<AType>("_alsf{()}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void BoxArgument()
        {
            AType expected = AArray.Create(ATypes.ABox,
                                           ABox.Create(AArray.Create(ATypes.ANull)),
                                           ABox.Create(AArray.Create(ATypes.ANull))
                                           );

            AType result = this.engine.Execute<AType>("_alsf{<()}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void SymbolArgument()
        {
            AType expected = AArray.Create(ATypes.ABox,
                                           ABox.Create(ASymbol.Create("symbol")),
                                           ABox.Create(ABox.Create(Utils.ANull()))
                                           );

            AType result = this.engine.Execute<AType>("_alsf{`symbol}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void SlotFillerExample()
        {
            AType expected = this.engine.Execute<AType>("(`small `medium `large `super;(16;32;64;72))");
            AType result = this.engine.Execute<AType>("_alsf{(`small `medium `large `super;(16;32;64;72))}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void SymbolVector()
        {
            AType expected = 
                AArray.Create(ATypes.ABox,
                                ABox.Create(AArray.Create(ATypes.ASymbol, ASymbol.Create("a"), ASymbol.Create("c"))),
                                ABox.Create(
                                    AArray.Create(ATypes.ABox, 
                                                    ABox.Create(ASymbol.Create("b")),
                                                    ABox.Create(ASymbol.Create("d"))
                                                 )
                                            )
                                );

            AType result = this.engine.Execute<AType>("_alsf{`a `b `c `d}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void AlsfExample()
        {
            AType titles = AArray.Create(ATypes.ASymbol,
                                         ASymbol.Create("a"),
                                         ASymbol.Create("b"),
                                         ASymbol.Create("c")
                                         );

            AType content = AArray.Create(ATypes.ABox,
                                    ABox.Create(AInteger.Create(1)),
                                    ABox.Create(AInteger.Create(2)),
                                    ABox.Create(AArray.Create(
                                                              ATypes.AInteger,
                                                              AInteger.Create(3),
                                                              AInteger.Create(4),
                                                              AInteger.Create(5)
                                                              )
                                                 )
                                    );

            AType expected =
                AArray.Create(ATypes.ABox, ABox.Create(titles), ABox.Create(content));

            AType result = this.engine.Execute<AType>(" _alsf{(`a;1;`b;2;`c;3 4 5)}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void DuplicatedKeyExample()
        {
            AType expected =
                AArray.Create(ATypes.ABox,
                              ABox.Create(
                                          AArray.Create(ATypes.ASymbol,
                                                          ASymbol.Create("a"),
                                                          ASymbol.Create("a"),
                                                          ASymbol.Create("a")
                                                          )
                                          ),
                              ABox.Create(AArray.Create(ATypes.ABox,
                                                  ABox.Create(AInteger.Create(1)),
                                                  ABox.Create(AInteger.Create(2)),
                                                  ABox.Create(AInteger.Create(3))
                                                        )
                                          )
                             );

            AType result = this.engine.Execute<AType>("_alsf{(`a;1;`a;2;`a;3)}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void AddedNullExample()
        {
            AType expected =
                AArray.Create(ATypes.ABox,
                              ABox.Create(
                                          AArray.Create(ATypes.ASymbol,
                                                          ASymbol.Create("a"),
                                                          ASymbol.Create("a"),
                                                          ASymbol.Create("a")
                                                          )
                                          ),
                              ABox.Create(AArray.Create(ATypes.ABox,
                                                  ABox.Create(AInteger.Create(1)),
                                                  ABox.Create(AInteger.Create(2)),
                                                  ABox.Create(AArray.Create(ATypes.ANull))
                                                        )
                                          )
                             );

            AType result = this.engine.Execute<AType>("_alsf{(`a;1;`a;2;`a)}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void DroppedNullExample()
        {
            AType expected =
                AArray.Create(ATypes.ABox,
                               ABox.Create(AArray.Create(ATypes.ASymbol, ASymbol.Create("a"))),
                               ABox.Create(AArray.Create(ATypes.ABox, ABox.Create(AInteger.Create(1))))
                             );

            AType result = this.engine.Execute<AType>("_alsf{(`a;1;)}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        public void CloneResult()
        {
            AType input = Helpers.BuildStrand(new AType[]{
                    AInteger.Create(-3), ASymbol.Create("b"), AInteger.Create(1), ASymbol.Create("a")
                }
            );

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".inp", input);

            this.engine.Execute<AType>("x := _alsf{inp}", scope);
            this.engine.Execute<AType>("((1;0) pick x) := -200", scope);

            Assert.AreEqual(AInteger.Create(1), input[1].NestedItem);
        }

        #endregion

        #region Error

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void WrongSequenceError()
        {
            this.engine.Execute<AType>("_alsf{(`a;`b;3 4;`b;4;5)}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void WrongTypeError()
        {
            this.engine.Execute<AType>("_alsf{5}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("ToSlotfiller"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RankError()
        {
            this.engine.Execute("_alsf{ (2 3 rho `a) }");
        }

        #endregion
    }
}
