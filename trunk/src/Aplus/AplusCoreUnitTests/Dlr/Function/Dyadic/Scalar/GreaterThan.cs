using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class GreaterThan : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 > ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanNull2Null()
        {
            AType result = this.engine.Execute<AType>("() > ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanInteger2Float()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1 > 3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanFloat2FloatToleranceTest()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("12345678912345679 > 12345678912345678");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanFloat2Integer()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("2.6 > 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanTolerabilyTest()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("3.0000000000000000000001 > 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanInteger2StringTypeError()
        {
            AType result = this.engine.Execute<AType>("1 > 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanFloat2Symbol()
        {
            AType result = this.engine.Execute<AType>("1.0 > `symbol");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void GreaterThanVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 > ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(0), AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("2 > 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(0), AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("0 2 4 > 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 > 10.0 -7 -4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        public void GreaterThanMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) > iota 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanInteger2Strand()
        {
            AType result = this.engine.Execute<AType>("1 > (1;3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GreaterThanStrand2Strand()
        {
            AType result = this.engine.Execute<AType>("(1;2;3) > (2;2;4)");
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void GreaterThanMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) > iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("GreaterThan"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void GreaterThanVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 > 4 6");
        }
       
    }
}
