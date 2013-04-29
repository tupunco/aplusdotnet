using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Runtime;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class TransposeAxes : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisIntegerList2ArrayRank3()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(4), AInteger.Create(8)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(5), AInteger.Create(9)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(6), AInteger.Create(10)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(7), AInteger.Create(11))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(12), AInteger.Create(16), AInteger.Create(20)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(13), AInteger.Create(17), AInteger.Create(21)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(14), AInteger.Create(18), AInteger.Create(22)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(15), AInteger.Create(19), AInteger.Create(23))
                )
            );
            
            AType result = this.engine.Execute<AType>("0 2 1 flip iota 2 3 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisIntegerListWithDups2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(4)
            );

            AType result = this.engine.Execute<AType>("0 0 flip iota 2 3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisNumber2Null()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("0 flip ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisNumber2NullUni()
        {
            AType expected = Utils.ANull();

            AType result = this.engineUni.Execute<AType>("0 S.\\ ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisRestrictedWholeNumberList2ArrayRank3()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(12)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(17)),
                AArray.Create(ATypes.AInteger, AInteger.Create(10), AInteger.Create(22))
            );

            AType result = this.engine.Execute<AType>("1.00000000000005 0 0 flip iota 2 3 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisIntegerList2ArrayRank8()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1970)),
                AArray.Create(ATypes.AInteger, AInteger.Create(723), AInteger.Create(2693))
            );

            AType result = this.engine.Execute<AType>("1 0 0 0 1 1 0 0 flip iota 8 4 3 2 2 4 5 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisNumber2CharacterConstantList()
        {
            AType expected = Helpers.BuildString("test");

            AType result = this.engine.Execute<AType>("0 flip 'test'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisIntegerMatrix2ArrayRank4()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(5))
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(6), AInteger.Create(7))
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(8), AInteger.Create(9)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(12), AInteger.Create(13))
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(10), AInteger.Create(11)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(14), AInteger.Create(15))
                    )
                )
            );

            AType result = this.engine.Execute<AType>("(2 2 rho 0 2 1 3) flip iota 2 2 2 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisIntegerList2Strand()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                Helpers.BuildStrand(new AType[]{AInteger.Create(6), AInteger.Create(3)}),
                Helpers.BuildStrand(new AType[]{AInteger.Create(8), AInteger.Create(4)})
            );

            AType result = this.engine.Execute<AType>("1 0 flip 2 2 rho (3;4;6;8)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        public void TransposeAxisIntegerList2NestedMixedArray()
        {
            ScriptScope scope = this.engine.CreateScope();

            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("a"),
                    f,
                    ABox.Create(AInteger.Create(2))
                ),
                AArray.Create(
                    ATypes.ABox,
                    ABox.Create(AInteger.Create(4)),
                    ASymbol.Create("b"),
                    f
                )
            );

            AType result = this.engine.Execute<AType>("1 0 flip 3 2 rho `a , (<4) , f , `b , (<2) , f", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TransposeAxesTypeError1()
        {
            AType result = this.engine.Execute<AType>("`a flip 4 2 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError1()
        {
            AType result = this.engine.Execute<AType>("3 -3 3 flip iota 3 4 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError2()
        {
            AType result = this.engine.Execute<AType>("3 4 2 0 flip iota 3 4 2 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError3()
        {
            AType result = this.engine.Execute<AType>("0 0 1 0 1 3 flip iota 3 4 2 5 7 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError4()
        {
            AType result = this.engine.Execute<AType>("0 2 0 0 flip iota 3 4 2 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError5()
        {
            AType result = this.engine.Execute<AType>("0 1 0 3 flip iota 3 4 2 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError6()
        {
            AType result = this.engine.Execute<AType>("1 flip `s1`s2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError7()
        {
            AType result = this.engine.Execute<AType>("1 flip ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TransposeAxesDomainError8()
        {
            AType result = this.engine.Execute<AType>("() flip 'a'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void TransposeAxesRankError1()
        {
            AType result = this.engine.Execute<AType>("0 1 0 flip iota 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void TransposeAxesRankError2()
        {
            AType result = this.engine.Execute<AType>("0 flip < 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void TransposeAxesRankError3()
        {
            AType result = this.engine.Execute<AType>("() flip ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void TransposeAxesRankError4()
        {
            AType result = this.engine.Execute<AType>("() flip 1 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void TransposeAxesRankError5()
        {
            AType result = this.engine.Execute<AType>("2 3 flip `test");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("TransposeAxes"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void TransposeAxesRankError6()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) flip iota 3 3");
        }
    }
}
