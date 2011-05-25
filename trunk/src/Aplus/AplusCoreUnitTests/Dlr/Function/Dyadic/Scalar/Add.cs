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
    public class Add : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 + ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddNull2Null()
        {
            AType result = this.engine.Execute<AType>("() + ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddInteger2Float()
        {
            AType expected = AFloat.Create(4.1);
            AType result = this.engine.Execute<AType>("1 + 3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddFloat2Integer()
        {
            AType expected = AFloat.Create(6.6);
            AType result = this.engine.Execute<AType>("2.6 + 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddFloat2Float()
        {
            AType expected = AFloat.Create(1234567891234 + 2.2);
            AType result = this.engine.Execute<AType>("1234567891234 + 2.2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddInteger2IntegerResultFloat()
        {
            AType expected = AFloat.Create(Int32.MaxValue + 1.0);
            AType result = this.engine.Execute<AType>(String.Format("{0} + 1", Int32.MaxValue));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void AddVector2Null()
        {
            AType result = this.engine.Execute<AType>("1 2 + ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddInteger2Vector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(3), AInteger.Create(5), AInteger.Create(7)
            );
            AType result = this.engine.Execute<AType>("2 + 0 2 4 + 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddInteger2Vector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(2), AInteger.Create(4), AInteger.Create(6)
            );
            AType result = this.engine.Execute<AType>("2 + 0 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddVector2Integer()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(3), AInteger.Create(5), AInteger.Create(7)
            );
            AType result = this.engine.Execute<AType>("0 2 4 + 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(10), AFloat.Create(5), AFloat.Create(0)
            );
            AType result = this.engine.Execute<AType>("0 12 -4 +  10.0 -7 4.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddFloatVector2Vector()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(0), AFloat.Create(1234567891234), AFloat.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 1234567891234 0 + 0 0 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(2)),
                AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(6))
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) + iota 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        public void AddFloatMatrix2Matrix()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(2 * 1000), AFloat.Create(1 * 1000)),
                AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(123456789 * 1000.0))
            );
            AType result = this.engine.Execute<AType>("(2 2 rho 0 0 0 0) + ((2 2 rho 2 1 0 123456789) * 1000)");


            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void AddMatrix2MatrixLengthError()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) + iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void AddVectorLengthError()
        {
            AType result = this.engine.Execute<AType>("1 2 3 + 4 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void AddTypeErrorString()
        {
            AType result = this.engine.Execute<AType>("1 + 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void AddTypeErrorStrand()
        {
            AType result = this.engine.Execute<AType>("1 + (1;3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Add"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void AddTypeErrorSymbol()
        {
            AType result = this.engine.Execute<AType>("1 + `test");
        }
    }
}
