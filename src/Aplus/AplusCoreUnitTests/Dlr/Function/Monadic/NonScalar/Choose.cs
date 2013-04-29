using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Choose : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseInteger2SymbolConstantList()
        {
            AType expected = ASymbol.Create("c");

            AType result = this.engine.Execute<AType>("2 # `a`b`c`d`e");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseInteger2SymbolConstantListUni()
        {
            AType expected = ASymbol.Create("c");

            AType result = this.engineUni.Execute<AType>("2 # `a`b`c`d`e");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseRestrictedWholeNumber2CharaterConstantList()
        {
            AType expected = AChar.Create('t');

            AType result = this.engine.Execute<AType>("3.000000000000006 # 'test'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseIntegerList2CharacterConstantMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("ghi"),
                Helpers.BuildString("abc")
            );

            AType result = this.engine.Execute<AType>("2 0 # 3 3 rho 'abcdefghi'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseIntegerList2CharacterConstantMatrixUni()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("ghi"),
                Helpers.BuildString("abc")
            );

            AType result = this.engineUni.Execute<AType>("2 0 # 3 3 S.? 'abcdefghi'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseNull2SymbolConstant()
        {
            AType expected = ASymbol.Create("test");

            AType result = this.engine.Execute<AType>("() # `test");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseNull2Strand()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AInteger.Create(4),
                    AInteger.Create(2)
                }
            );

            AType result = this.engine.Execute<AType>("() # (2;4)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseIntegerMatrix2IntegerMatrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(5), AInteger.Create(6), AInteger.Create(7)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(8), AInteger.Create(9), AInteger.Create(10), AInteger.Create(11))
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(12), AInteger.Create(13), AInteger.Create(14), AInteger.Create(15)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(16), AInteger.Create(17), AInteger.Create(18), AInteger.Create(19)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(20), AInteger.Create(21), AInteger.Create(22), AInteger.Create(23))
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(12), AInteger.Create(13), AInteger.Create(14), AInteger.Create(15)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(16), AInteger.Create(17), AInteger.Create(18), AInteger.Create(19)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(20), AInteger.Create(21), AInteger.Create(22), AInteger.Create(23))
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(5), AInteger.Create(6), AInteger.Create(7)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(8), AInteger.Create(9), AInteger.Create(10), AInteger.Create(11))
                    )
                )
            );

            AType result = this.engine.Execute<AType>("( 2 2 rho 0 1 1 0) # iota 2 3 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseIntegerMatrix2CharacterConstantMatrix()
        {
            AType expected = AArray.Create(ATypes.AChar,
                Helpers.BuildString("hello"),
                Helpers.BuildString("world")
            );

            AType result = this.engine.Execute<AType>("(2 5 rho 5 6 0 0 1 2 1 3 0 4) # 'lowrdhe'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseNestedRestrictedWholeNumber2CharaterConstantList()
        {
            AType expected = AChar.Create('s');

            AType result = this.engine.Execute<AType>("(< 2.000000000000006) # 'test'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseNestedInteger2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(5)
            );

            AType result = this.engine.Execute<AType>("(< 2) # iota 3 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseNestedNull2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("(< ()) # iota 3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseStrand2CharacterMatrix()
        {
            AType expected = Helpers.BuildString("ac");

            AType result = this.engine.Execute<AType>("(0;0 2) # 3 3 rho 'abcdefghi'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseStrandWithNull2CharacterMatrix()
        {
            AType expected = Helpers.BuildString("adg");

            AType result = this.engine.Execute<AType>("(;0) # 3 3 rho 'abcdefghi'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        public void ChooseStrand2IntegerMatrixWithFrame1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(12),
                AInteger.Create(13),
                AInteger.Create(14),
                AInteger.Create(15)
            );

            AType result = this.engine.Execute<AType>("(1;0) # iota 2 3 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ChooseStrand2IntegerMatrixWithFrame2()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(6), AInteger.Create(7))
            );

            AType result = this.engine.Execute<AType>("(0;0 1;2 3) # iota 4 4 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ChooseStrand2IntegerMatrixWithFrame3()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
            );

            AType result = this.engine.Execute<AType>("(0;0 1) # iota 2 2 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ChooseStrand2IntegerMatrixWithFrame4()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4), AInteger.Create(5)),
                AArray.Create(ATypes.AInteger, AInteger.Create(6), AInteger.Create(7), AInteger.Create(8))
            );

            AType result = this.engine.Execute<AType>("(0;iota 1 # rho a) # a := iota 3 3 3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ChooseStrand2IntegerMatrix1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("(0 0;1) # 2 2 rho iota 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ChooseStrand2IntegerMatrix2()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("(b; b := 1) # iota 2 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ChooseNestedMatrix2IntegerArray()
        {
            AType expected = AInteger.Create(12);

            AType result = this.engine.Execute<AType>("(2 2 rho (0;1;1;0)) # iota 3 2 2 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ChooseRankError1()
        {
            this.engine.Execute<AType>("0 # < 5");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ChooseRankError2()
        {
            this.engine.Execute<AType>("0 1 # `s");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ChooseRankError3()
        {
            this.engine.Execute<AType>("(<()) # 5");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ChooseRankError4()
        {
            this.engine.Execute<AType>("(0;1;0) # iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ChooseRankError5()
        {
            this.engine.Execute<AType>("(< 0) # 3");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ChooseRankError6()
        {
            this.engine.Execute<AType>("(1;1) # iota 4");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ChooseRankError7()
        {
            this.engine.Execute<AType>("(2 2 rho (0;1;1;0)) # iota 3 2 2");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ChooseTypeError1()
        {
            this.engine.Execute<AType>("'a' # 4 56 7");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ChooseTypeError2()
        {
            this.engine.Execute<AType>("1.2 # 1 2 3 4");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ChooseTypeError3()
        {
            this.engine.Execute<AType>("(1 0;'abc';5) # iota 3 4 2");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void ChooseIndexError1()
        {
            this.engine.Execute<AType>("3 # 'abc'");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void ChooseIndexError2()
        {
            this.engine.Execute<AType>("0 3 1 # 5.4 7.8");
        }

        [TestCategory("DLR"), TestCategory("Choose"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void ChooseIndexError3()
        {
            this.engine.Execute<AType>("(1;-3) # iota 3 4");
        }
    }
}
