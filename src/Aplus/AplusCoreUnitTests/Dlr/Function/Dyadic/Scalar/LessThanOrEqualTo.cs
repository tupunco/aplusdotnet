using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class LessThanOrEqualTo : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 <= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToNull2Null()
        {
            AType result = this.engine.Execute<AType>("() <= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void LessThanOrEqualToVector2Vector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("-200 0 90 100 101 200 <= 100");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("LessThanOrEqualTo"), TestMethod]
        public void GreaterThanOrEqualTolerabilyTest()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("2.000000000000001 <= 2");

            Assert.AreEqual(expected, result);
        }
    }
}
