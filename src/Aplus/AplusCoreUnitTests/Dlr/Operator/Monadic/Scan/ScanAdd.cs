using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Scan
{
    [TestClass]
    public class ScanAdd : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAdd"), TestMethod]
        public void ScanAddIntegerVector1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(4),
                AInteger.Create(-1),
                AInteger.Create(1),
                AInteger.Create(7)
            );

            AType result = this.engine.Execute<AType>(@"+\ 1 3 -5 2 6");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAdd"), TestMethod]
        public void ScanAddIntegerVector2()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(3),
                AFloat.Create(2147483650),
                AFloat.Create(2147483656),
                AFloat.Create(2147483664),
                AFloat.Create(4294967311),
                AFloat.Create(4294967313)
            );

            AType result = this.engine.Execute<AType>(@"+\ 3 2147483647 6 8 2147483647 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAdd"), TestMethod]
        public void ScanAddFloatMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(54.34), AFloat.Create(45), AFloat.Create(12.3)),
                AArray.Create(ATypes.AFloat, AFloat.Create(142.24), AFloat.Create(79), AFloat.Create(67.1)),
                AArray.Create(ATypes.AFloat, AFloat.Create(156.24), AFloat.Create(146.3), AFloat.Create(162.1))
            );

            AType result = this.engine.Execute<AType>(@"+\ 3 3 rho 54.34 45 12.3 87.9 34 54.8 14 67.3 95");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAdd"), TestMethod]
        public void ScanAddNull()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>(@"+\ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAdd"), TestMethod]
        public void ScanAddNullArray()
        {
            AType expected = AArray.Create(ATypes.AInteger);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 3};
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>(@"+\ 0 2 3 rho 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAdd"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ScanAddTypeError()
        {
            AType result = this.engine.Execute<AType>(@"+\ 1 rho < 6");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ScanAdd"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void ScanAddValenceError()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);

            AType result = this.engine.Execute<AType>(@"a\ 5 6", scope);
        }
    }
}
