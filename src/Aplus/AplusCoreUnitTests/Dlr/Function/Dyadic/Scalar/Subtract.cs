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
    public class Subtract :AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 - ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractNull2Null()
        {
            AType result = this.engine.Execute<AType>("() - ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractInteger2Float()
        {
            AType expected = AFloat.Create(-2.1);
            AType result = this.engine.Execute<AType>("1 - 3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractFloat2Integer()
        {
            AType expected = AFloat.Create(-1.4);
            AType result = this.engine.Execute<AType>("2.6 - 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractInteger2IntegerResultFloat()
        {
            AType expected = AFloat.Create(Int32.MinValue - 1.0);
            AType result = this.engine.Execute<AType>(String.Format("{0} - 1", Int32.MinValue));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void SubtractVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 - ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(2), AInteger.Create(0), AInteger.Create(-2)
            );
            AType result = this.engine.Execute<AType>("2 - 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(-3), AInteger.Create(-1), AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 2 4 - 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(-10), AFloat.Create(19), AFloat.Create(-8)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 -  10.0 -7 4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        public void SubtractMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) - iota 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void SubtractMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) - iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void SubtractVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 - 4 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void SubtractTypeErrorString()
        {
            AType result = this.engine.Execute<AType>("1 - 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Subtract"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void SubtractTypeErrorStrand()
        {
            AType result = this.engine.Execute<AType>("1 - (1;3)");
        }
    }
}
