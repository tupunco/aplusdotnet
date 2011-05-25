using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class Floor : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Floor"), TestMethod]
        public void FloorVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(-9),
                AInteger.Create(-10),
                AInteger.Create(-10),
                AInteger.Create(-10),
                AInteger.Create(-1),
                AInteger.Create(-5)
            );

            AType result = this.engine.Execute<AType>("min 10 10.2 10.5 10.98 -9 -9.2 -9.5 -9.98  -0.1 -5.000000000000000000000002");

            Assert.AreEqual(expected.Type, result.Type, "Type mismatch");
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Floor"), TestMethod]
        public void FloorNull()
        {
            AType result = this.engine.Execute<AType>("min ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }
    }
}
