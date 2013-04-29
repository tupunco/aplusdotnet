using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class NullFunction : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Null"), TestMethod]
        public void Null()
        {
            AType result = this.engine.Execute<AType>("where 1 2 3");

            Assert.IsTrue(result.Type == ATypes.ANull, "Not 'null' type returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Null"), TestMethod]
        public void NullUni()
        {
            AType result = this.engineUni.Execute<AType>("A.< 1 2 3");

            Assert.IsTrue(result.Type == ATypes.ANull, "Not 'null' type returned");
    }

}
}
