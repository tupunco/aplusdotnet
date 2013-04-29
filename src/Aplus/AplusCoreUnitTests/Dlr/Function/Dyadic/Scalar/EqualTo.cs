using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class EqualTo : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToSymbolToSymbol()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("`valami = `valami");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToSymbolToSymbolUni()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engineUni.Execute<AType>("`valami = `valami");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToSymbolToSymbol2()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("`valami = `semmi");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 = ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToNull2Null()
        {
            AType result = this.engine.Execute<AType>("() = ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToInteger2Float()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1 = 3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToFloat2Integer()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("2.6 = 4");

            Assert.AreEqual(expected, result);
        }

        
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToFloat2FloatToleranceTest()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("12345678912345679 = 12345678912345678");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToInteger2String()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(0), AInteger.Create(0), AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("1 = 'test'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToFloat2Symbol()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1.0 = `symbol");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void EqualToVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 = ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("2 = 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(0), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 2 4 = 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(0), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 = 10.0 -7 -4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) = iota 2 2");

            Assert.AreEqual(expected, result);
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToInteger2Strand()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("1 = (1;3)");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        public void EqualToStrand2Strand()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("(1;2;3) = (2;2;4)");

            Assert.AreEqual(expected, result);
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void EqualToMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) = iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("EqualTo"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void EqualToVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 = 4 6");
        }
       
    }
}
