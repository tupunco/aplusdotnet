using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class Or : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrVector2Integer()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 0 5 1 ? 0 9 0 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrVector2IntegerUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1)
            );
            AType result = this.engineUni.Execute<AType>("0 0 5 1 | 0 9 0 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrFloat2Integer()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0.000000000000001 2.000000000000001 ? 0 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrFloat2Float()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0.000000000000001 2.000000000000001 ? 0.000000000000001 1.000000000000001");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrNull2Integer()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("() ? 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrNull2Null()
        {
            AType result = this.engine.Execute<AType>("() ? ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void OrTypeError1()
        {
            AType result = this.engine.Execute<AType>("3.4 ? 1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void OrTypeError1b()
        {
            AType result = this.engine.Execute<AType>("1 ? 3.4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void OrTypeError2()
        {
            AType result = this.engine.Execute<AType>("1 ? <6");
        }
    }
}
