using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class And : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndFloat2Float()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1));
            AType result = this.engine.Execute<AType>("0.000000000000001 1.000000000000001 & 0.000000000000001 1.000000000000001");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndFloat2FloatUni()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1));
            AType result = this.engineUni.Execute<AType>("0.000000000000001 1.000000000000001 & 0.000000000000001 1.000000000000001");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndInteger2Float()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1));
            AType result = this.engine.Execute<AType>("1 1 & 0.000000000000001 1.000000000000001");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 & ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndNull2Null()
        {
            AType result = this.engine.Execute<AType>("() & ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void AndInteger2FloatTypeError()
        {
            AType result = this.engine.Execute<AType>("1 & 3.1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndFloatTolerablyInteger2Integer()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("2.0 & 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void AndVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 & ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("2 & 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 -2 4 & 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 &  10.0 -7 4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        public void AndMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) & iota 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void AndMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) & iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void AndVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 & 4 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void AndTypeErrorString()
        {
            AType result = this.engine.Execute<AType>("1 & 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("And"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void AndTypeErrorStrand()
        {
            AType result = this.engine.Execute<AType>("1 & (1;3)");
        }
    }
}
