using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Restructure : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestructureInt2CharacterConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("abc"),
                Helpers.BuildString("ABC")
            );
            AType result = this.engine.Execute<AType>("3 ! 'abcABC'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestructureInteger2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(0),
                        AInteger.Create(1),
                        AInteger.Create(2),
                        AInteger.Create(3)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(4),
                        AInteger.Create(5),
                        AInteger.Create(6),
                        AInteger.Create(7)
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(8),
                        AInteger.Create(9),
                        AInteger.Create(10),
                        AInteger.Create(11)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(12),
                        AInteger.Create(13),
                        AInteger.Create(14),
                        AInteger.Create(15)
                    )
                )
            );
            AType result = this.engine.Execute<AType>("2.0000000000000001 ! iota 4 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestrucutreNegativeInteger2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1),
                    AInteger.Create(2)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(1),
                    AInteger.Create(2),
                    AInteger.Create(3)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(2),
                    AInteger.Create(3),
                    AInteger.Create(4)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(3),
                    AInteger.Create(4),
                    AInteger.Create(5)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(4),
                    AInteger.Create(5),
                    AInteger.Create(6)
                )
            );
            AType result = this.engine.Execute<AType>("-3 ! iota 7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestrucutreNegativeInteger2Strand()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                AArray.Create(
                    ATypes.AInteger,
                    ABox.Create(AInteger.Create(4)),
                    ABox.Create(AInteger.Create(5))
                ),
                AArray.Create(
                    ATypes.ABox,
                    ABox.Create(AInteger.Create(5)),
                    ABox.Create(AInteger.Create(1))
                ),
                AArray.Create(
                    ATypes.ABox,
                    ABox.Create(AInteger.Create(1)),
                    ABox.Create(AInteger.Create(4))
                )
            );
            AType result = this.engine.Execute<AType>("-2 ! (4;5;1;4)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestructureInteger2SymbolConstant()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("test")
            );
            AType result = this.engine.Execute<AType>("1 ! `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestructureInteger2SymbolConstantUni()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("test")
            );
            AType result = this.engineUni.Execute<AType>("1 S.! `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestructureIntegerList2NestedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := {+}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("a"),
                    ASymbol.Create("b")
                ),
                AArray.Create(
                    ATypes.ABox,
                    ABox.Create(AInteger.Create(3)),
                    ABox.Create(AInteger.Create(4))
                ),
                AArray.Create(
                    ATypes.AFunc,
                    ABox.Create(f),
                    ABox.Create(AInteger.Create(6))
                )
            );
            AType result = this.engine.Execute<AType>("2 ! (`a`b , (3;4)) , (f;6)", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestructureInteger2NestedArray()
        {
            AType expected = AArray.Create(
                ATypes.ANull,
                AArray.Create(ATypes.ANull),
                AArray.Create(ATypes.ANull),
                AArray.Create(ATypes.ANull),
                AArray.Create(ATypes.ANull)
            );

            AType result = this.engine.Execute<AType>("0 ! (3;4;7)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        public void RestructureMovingAverageOfLength3()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(1),
                AFloat.Create(2),
                AFloat.Create(3),
                AFloat.Create(4),
                AFloat.Create(5)
            );

            AType result = this.engine.Execute<AType>("(+/@1 rtack -3 ! iota 7) % 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RestructureLengthError1()
        {
            AType result = this.engine.Execute<AType>("3 ! 3 2 1 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RestructureLengthError2()
        {
            AType result = this.engine.Execute<AType>("-4 ! 3 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RestructureLengthError3()
        {
            AType result = this.engine.Execute<AType>("-2 ! ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        [ExpectedException(typeof(Error.Nonce))]
        public void RestructureNonceError()
        {
            AType result = this.engine.Execute<AType>("3 1 ! 3 2 1 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        [ExpectedException(typeof(Error.MaxRank))]
        public void RestructureMaxRankError()
        {
            AType result = this.engine.Execute<AType>("1 ! 1 1 1 1 1 1 1 1 1 rho 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Restructure"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RestructureRankError()
        {
            AType result = this.engine.Execute<AType>("3 ! 6");
        }
    }
}
