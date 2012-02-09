using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Reduction
{
    [TestClass]
    public class ReduceOr : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceOr"), TestMethod]
        public void ReduceOrIntegerVector()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("?/ 0 0 1 0 0 0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceOr"), TestMethod]
        public void ReduceOrIntegerVector2()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("?/ 0 0 0 0 0 0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceOr"), TestMethod]
        public void ReduceOrFloatNumber()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("?/ 5.0000000000000004");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndFloatNumber2()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("?/ 0.0000000000000000001");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceOr"), TestMethod]
        public void ReduceOrIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("?/ 3 3 rho 1 1 0 0 0 0 1 0 0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceOr"), TestMethod]
        public void ReduceOrNull()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("?/ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceOr"), TestMethod]
        public void ReduceOrCharacterArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0))
            );

            AType result = this.engine.Execute<AType>("?/ 0 2 2 rho 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceOr"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ReduceOrTypeError()
        {
            AType result = this.engine.Execute<AType>("?/ 5.4");
        }
    }
}
