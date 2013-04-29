using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class LessThan : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanFloat2Float()
        {
            AType result = this.engine.Execute<AType>("1.1 < 1.3");
            AType excepted = AInteger.Create(1);

            Assert.AreEqual(excepted, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanFloat2FloatUni()
        {
            AType result = this.engineUni.Execute<AType>("1.1 < 1.3");
            AType excepted = AInteger.Create(1);

            Assert.AreEqual(excepted, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanFloat2Int()
        {
            AType result = this.engine.Execute<AType>("1.1 < 2");
            AType excepted = AInteger.Create(1);

            Assert.AreEqual(excepted, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanChar2Char()
        {
            AType excepted = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("'a' < 'c'");

            Assert.AreEqual(excepted, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanChar2Char2()
        {
            AType excepted = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("'c' < 'a'");

            Assert.AreEqual(excepted, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanSym2Sym()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("`valami < `semmi");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanSym2Sym2()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("`semmi < `valami");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 < ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanNull2Null()
        {
            AType result = this.engine.Execute<AType>("() < ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanVector2Vector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("-200 0 90 100 101 200 < 100");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThan"), TestMethod]
        public void LessThanTolerabilyTest()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("6 < 6.0000000000000000000001");

            Assert.AreEqual(expected, result);
        }
    }
}
