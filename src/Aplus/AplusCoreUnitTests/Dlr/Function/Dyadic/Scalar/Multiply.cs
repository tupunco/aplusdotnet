using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class Multiply : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyFloat2Float()
        {
            AType expected = AFloat.Create(1.2 * 3.4);
            AType result = this.engine.Execute<AType>("1.2 * 3.4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyFloat2FloatUni()
        {
            AType expected = AFloat.Create(1.2 * 3.4);
            AType result = this.engineUni.Execute<AType>("1.2 * 3.4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 * ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyNull2Null()
        {
            AType result = this.engine.Execute<AType>("() * ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyInteger2Float()
        {
            AType expected = AFloat.Create(3.1);
            AType result = this.engine.Execute<AType>("1 * 3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyFloat2Integer()
        {
            AType expected = AFloat.Create(2.6 * 4);
            AType result = this.engine.Execute<AType>("2.6 * 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyInteger2IntegerResultFloat()
        {
            AType expected = AFloat.Create(Int32.MaxValue * 2.0);
            AType result = this.engine.Execute<AType>(String.Format("{0} * 2", Int32.MaxValue));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void MultiplyVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 * ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(4), AInteger.Create(8)
            );
            AType result = this.engine.Execute<AType>("2 * 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(6), AInteger.Create(12)
            );
            AType result = this.engine.Execute<AType>("0 2 4 * 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyInfinityVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(double.PositiveInfinity),
                AFloat.Create(double.NegativeInfinity),
                AFloat.Create(double.PositiveInfinity)
            );
            AType result = this.engine.Execute<AType>("-Inf Inf -Inf * -1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(0), AFloat.Create(12 * -7.0), AFloat.Create(-16)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 * 10.0 -7 4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        public void MultiplyMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(9))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) * iota 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void MultiplyMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) * iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void MultiplyVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 * 4 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void MultiplyTypeErrorString()
        {
            AType result = this.engine.Execute<AType>("1 * 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Multiply"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void MultiplyTypeErrorStrand()
        {
            AType result = this.engine.Execute<AType>("1 * (1;3)");
        }
    }
}
