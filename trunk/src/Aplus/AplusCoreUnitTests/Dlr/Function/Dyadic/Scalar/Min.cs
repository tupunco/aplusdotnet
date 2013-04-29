using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class Min : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Min"), TestMethod]
        public void MinVector2Integer()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(99.5),
                AFloat.Create(100),
                AFloat.Create(91.1),
                AFloat.Create(100),
                AFloat.Create(99)
            );
            AType result = this.engine.Execute<AType>("99.5 100 91.1 112 99 min 100");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Min"), TestMethod]
        public void MinVector2IntegerUni()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(99.5),
                AFloat.Create(100),
                AFloat.Create(91.1),
                AFloat.Create(100),
                AFloat.Create(99)
            );
            AType result = this.engineUni.Execute<AType>("99.5 100 91.1 112 99 M.- 100");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Min"), TestMethod]
        public void MinNull2Null()
        {
            AType result = this.engine.Execute<AType>("() min ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Min"), TestMethod]
        public void MinFloat2Null()
        {
            AType result = this.engine.Execute<AType>("3.3 min ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Min"), TestMethod]
        public void MinInteger2Float()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(99.5),
                AFloat.Create(100),
                AFloat.Create(91.1),
                AFloat.Create(100),
                AFloat.Create(99)
            );
            AType result = this.engine.Execute<AType>("100 100 92 112 99 min 99.5 100 91.1 100 99");

            Assert.AreEqual(expected, result);
        }
    }
}
