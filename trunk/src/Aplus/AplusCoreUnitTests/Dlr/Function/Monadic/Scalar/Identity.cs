using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class Identity : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Identity"), TestMethod]
        public void IdentityNull()
        {
            AType result = this.engine.Execute<AType>("+ ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Incorrect type");
        }
    }
}
