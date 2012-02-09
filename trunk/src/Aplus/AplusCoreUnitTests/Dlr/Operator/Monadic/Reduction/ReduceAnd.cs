using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Reduction
{
    [TestClass]
    public class ReduceAnd : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndOneLengthVector()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("&/ 1 rho 1");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndTolerablyWholeNumber()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("&/ 0.0000000000000000001");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndTolerablyWholeNumber2()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("&/ 1.0000000000000000001");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndIntegerVector()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("&/ 0 0 1 1 0 0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("&/ 3 3 rho 1 1 0 1 0 1 1 0 0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndNull()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("&/ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        public void ReduceAndCharacterArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1))
            );

            AType result = this.engine.Execute<AType>("&/ 0 2 2 rho 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ReduceAnd"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ReduceAndNotTolerablyWholeNumber()
        {
            AType result = this.engine.Execute<AType>("&/ 1.1");
        }
    }
}
