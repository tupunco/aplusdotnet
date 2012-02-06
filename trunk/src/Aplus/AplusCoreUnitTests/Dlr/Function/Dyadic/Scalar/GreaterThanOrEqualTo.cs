using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class GreaterThanOrEqualTo : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualChar2Char()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("'c' >= 'b'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualChar2Char2()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("'b' >= 'c'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualSym2Sym()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("`something >= `something");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualSym2Sym2()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("`something >= `word");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 >= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualNull2Null()
        {
            AType result = this.engine.Execute<AType>("() >= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualInteger2Float()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1 >= 3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualFloat2Integer()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("2.6 >= 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualTolerabilyTest()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("2 >= 2.000000000000001");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void Float2FloatToleranceTest()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("12345678912345679 >= 12345678912345678");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanOrEqualInteger2StringTypeError()
        {
            AType result = this.engine.Execute<AType>("1 >= 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanOrEqualFloat2Symbol()
        {
            AType result = this.engine.Execute<AType>("1.0 >= `symbol");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void GreaterOrEqualThanVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 >= ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(1), AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("2 >= 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(0), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 2 4 >= 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 >= 10.0 -7 -4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) >= iota 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanOrEqualInteger2Strand()
        {
            AType result = this.engine.Execute<AType>("1 >= (1;3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanOrEqualStrand2Strand()
        {
            AType result = this.engine.Execute<AType>("(1;2;3) >= (2;2;4)");
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void GreaterThanOrEqualMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) >= iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThanOrEqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void GreaterThanOrEqualVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 >= 4 6");
        }
    }
}
