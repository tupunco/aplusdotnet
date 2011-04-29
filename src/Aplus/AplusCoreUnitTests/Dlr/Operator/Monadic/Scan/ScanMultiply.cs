using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Scan
{
    [TestClass]
    public class ScanMultiply : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMultiply"), TestMethod]
        public void ScanMultiplyIntegerVector1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(12),
                AInteger.Create(84),
                AInteger.Create(2688)
            );

            AType result = this.engine.Execute<AType>(@"*\ 3 4 7 32");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMultiply"), TestMethod]
        public void ScanMultiplyIntegerVector2()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(12345),
                AFloat.Create(83896620),
                AFloat.Create(1.023538764e+10)

            );

            AType result = this.engine.Execute<AType>(@"*\ 12345 6796 122");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMultiply"), TestMethod]
        public void ScanMultiplyIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(5), AInteger.Create(12), AInteger.Create(21)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(45), AInteger.Create(120), AInteger.Create(231))

            );

            AType result = this.engine.Execute<AType>(@"*\ iota 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMultiply"), TestMethod]
        public void ScanMultiplyNull()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>(@"*\ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMultiply"), TestMethod]
        public void ScanMultiplyNullArray()
        {
            AType expected = AArray.Create(ATypes.AInteger);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 3 };
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>(@"*\ 0 2 3 rho < 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
