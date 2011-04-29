using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Scan
{
    [TestClass]
    public class ScanMin : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMin"), TestMethod]
        public void ScanMinFloatVector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(4.5),
                AFloat.Create(3),
                AFloat.Create(3),
                AFloat.Create(3),
                AFloat.Create(1.7),
                AFloat.Create(1.7)
            );

            AType result = this.engine.Execute<AType>(@"min\ 4.5 3 7 3.2 1.7 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMin"), TestMethod]
        public void ScanMinIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3))
            );

            AType result = this.engine.Execute<AType>(@"min\ iota 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMin"), TestMethod]
        public void ScanMinNull()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>(@"min\ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanMin"), TestMethod]
        public void ScanMinNullArray()
        {
            AType expected = AArray.Create(ATypes.AFloat);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 3 };
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>(@"min\ 0 2 3 rho 4.5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
