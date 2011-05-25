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
    public class Divide : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 % ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideNull2Null()
        {
            AType result = this.engine.Execute<AType>("() % ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideInteger2Float()
        {
            AType expected = AFloat.Create(1 / 3.1);
            AType result = this.engine.Execute<AType>("1 % 3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideFloat2Integer()
        {
            AType expected = AFloat.Create(2.6 / 4);
            AType result = this.engine.Execute<AType>("2.6 % 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void DivideVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 % ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideIntegerWithZero()
        {
            AType expected = AFloat.Create(double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("1 % 0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(double.PositiveInfinity),
                AFloat.Create(1),
                AFloat.Create(2 / 4.0)
            );
            AType result = this.engine.Execute<AType>("2 % 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(0),
                AFloat.Create(2 / 3.0),
                AFloat.Create(4 / 3.0)
            );
            AType result = this.engine.Execute<AType>("0 2 4 % 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(0 / 10.0),
                AFloat.Create(12 / -7.0),
                AFloat.Create(-4 / 4.0)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 % 10.0 -7 4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivideMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(1 / 2.0)),
                AArray.Create(ATypes.AFloat, AFloat.Create(2 / 3.0), AFloat.Create(3 / 4.0))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) % 2 2 rho 1 2 3 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivisonOfPositiveNumberByZero()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("3 % 0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        public void DivisonOfNegativeNumberByZero()
        {
            AType expected = AFloat.Create(Double.NegativeInfinity);
            AType result = this.engine.Execute<AType>("-6 % 0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void DivideMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) % iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void DivideVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 % 4 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DivideTypeErrorString()
        {
            AType result = this.engine.Execute<AType>("1 % 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DivideTypeErrorStrand()
        {
            AType result = this.engine.Execute<AType>("1 % (1;3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Divide"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DivideDomainErrorZeroPerZero()
        {
            AType result = this.engine.Execute<AType>("0 % 0");
        }
    }
}
