using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Scan
{
    [TestClass]
    public class ScanAnd : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAnd"), TestMethod]
        public void ScanAndIntegerVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>(@"&\ 1 1 1 0 1 1 0 1");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAnd"), TestMethod]
        public void ScanAndFloatVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>(@"&\ 2 3.00000000000000004 4 2 0 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAnd"), TestMethod]
        public void ScanAndFloatMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(2), AInteger.Create(8)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(1))
            );

            AType result = this.engine.Execute<AType>(@"&\ 3 2.00000000000005 8 , iota 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAnd"), TestMethod]
        public void ScanAndNull()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>(@"&\ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAnd"), TestMethod]
        public void ScanAndNullArray()
        {
            AType expected = AArray.Create(ATypes.AInteger);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 3 };
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>(@"&\ 0 2 3 rho 4.5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAnd"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ScanAndTypeError1()
        {
            AType result = this.engine.Execute<AType>(@"&\ 3.5 7 5");
        }
    }
}
