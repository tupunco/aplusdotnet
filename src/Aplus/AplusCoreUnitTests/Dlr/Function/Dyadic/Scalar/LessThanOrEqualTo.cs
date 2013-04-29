using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class LessThanOrEqualTo : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 <= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToInteger2NullUni()
        {
            AType result = this.engineUni.Execute<AType>("1 <= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToNull2Null()
        {
            AType result = this.engine.Execute<AType>("() <= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToVector2Vector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("-200 0 90 100 101 200 <= 100");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualTolerabilyTest()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("2.000000000000001 <= 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToSymbol2Symbol()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("`someting <= `word");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToSymbol2Symbol2()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("`word <= `someting");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToChar2Char()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("'a' <= 'b'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToChar2Char2()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("'b' <= 'a'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToFloat2Float()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("1.1 <= 1.2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToFloat2Float2()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1.2 <= 1.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToInt2Float()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("1 <= 1.2");

            Assert.AreEqual(expected, result);
        }
    }
}
