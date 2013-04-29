using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Bins : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsIntegerList2FloatList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(2),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>("-1 0 1 upg 0.3 -0.3 -2 0.1 1 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsIntegerList2FloatListUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(2),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engineUni.Execute<AType>("-1 0 1 I.+ 0.3 -0.3 -2 0.1 1 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsFloatList2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(2),
                AInteger.Create(2)
            );
            AType result = this.engine.Execute<AType>("-3.2 6 9.76 upg -2 -6 8 7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsIntegerList2Integer()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("-1 0 1 upg 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsIntegerVector2MatrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(0)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(1),
                    AInteger.Create(1)
                )
            );
            AType result = this.engine.Execute<AType>("(1 3 rho 5 6 9) upg iota 2 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsMatrix2MatrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("(1 2 3 rho 2 4 6 8 10 15) upg iota 2 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsCharacterConstant2CharacterConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("'g' upg 'test'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsSymbolConstant2SymbolConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("`j upg `a`l`t`z`h");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        public void BinsIntegerVector2Matrix()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("(2 3 rho 1 4 5 7 9 12) upg 2 3 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void BinsLengthError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) upg iota 3 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void BinsLengthError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) upg 1 rho 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void BinsLengthError3()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) upg ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void BinsRankError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2 4) upg iota 8");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Bins"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void BinsRankError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) upg 4");
        }
    }
}
