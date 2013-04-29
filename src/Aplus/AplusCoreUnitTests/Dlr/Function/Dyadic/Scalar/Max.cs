using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class Max : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Max"), TestMethod]
        public void MaxInt2Float()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(3.1),
                AFloat.Create(7),
                AFloat.Create(10)
            );
            AType result = this.engine.Execute<AType>("2 5 8 max 3.1 7.0 10.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Max"), TestMethod]
        public void MaxInt2FloatUni()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(3.1),
                AFloat.Create(7),
                AFloat.Create(10)
            );
            AType result = this.engineUni.Execute<AType>("2 5 8 M.+ 3.1 7.0 10.0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Max"), TestMethod]
        public void MaxFloat2Int()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(3),
                AFloat.Create(7),
                AFloat.Create(10)
            );
            AType result = this.engine.Execute<AType>("2.1 5.2 8.3 max 3 7 10");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Max"), TestMethod]
        public void MaxVector2Vector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(7),
                AInteger.Create(10)
            );
            AType result = this.engine.Execute<AType>("2 5 8 max 3 7 10");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Max"), TestMethod]
        public void MaxNull2Null()
        {
            AType result = this.engine.Execute<AType>("() max ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Max"), TestMethod]
        public void MaxFloat2Null()
        {
            AType result = this.engine.Execute<AType>("3.3 max ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }
    }
}
