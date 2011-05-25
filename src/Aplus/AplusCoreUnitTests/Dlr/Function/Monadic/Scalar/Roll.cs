using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class Roll : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Roll"), TestMethod]
        public void RollNull()
        {
            AType result = this.engine.Execute<AType>("rand ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Roll"), TestMethod]
        public void RollNumbers()
        {
            AType result = this.engine.Execute<AType>("rand 3 4");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
            Assert.AreEqual(2, result.Length, "Incorrect vector length");
        }
    }
}
