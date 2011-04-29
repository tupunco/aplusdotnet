using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Scan
{
    [TestClass]
    public class ScanMax : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMax"), TestMethod]
        public void ScanMaxFloatVector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(4.5),
                AFloat.Create(4.5),
                AFloat.Create(7),
                AFloat.Create(7),
                AFloat.Create(7),
                AFloat.Create(8)
            );

            AType result = this.engine.Execute<AType>(@"max\ 4.5 3 7 3.2 1.7 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMax"), TestMethod]
        public void ScanMaxIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(5), AInteger.Create(6), AInteger.Create(7)),
                AArray.Create(ATypes.AInteger, AInteger.Create(8), AInteger.Create(9), AInteger.Create(10), AInteger.Create(11))
            );

            AType result = this.engine.Execute<AType>(@"max\ iota 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMax"), TestMethod]
        public void ScanMaxNull()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>(@"max\ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMax"), TestMethod]
        public void ScanMaxNullArray()
        {
            AType expected = AArray.Create(ATypes.AFloat);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 3 };
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>(@"max\ 0 2 3 rho 4.5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
