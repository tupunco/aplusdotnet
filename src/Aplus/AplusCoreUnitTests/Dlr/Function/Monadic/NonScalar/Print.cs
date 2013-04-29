using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Print : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Print"), TestMethod]
        public void SimplePrint()
        {
            AType expected = Helpers.BuildString("Hello");
            AType result = this.engine.Execute<AType>("drop 'Hello'");

            Assert.AreEqual<AType>(expected, result, "Invalid return value");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Print"), TestMethod]
        public void SimplePrintUni()
        {
            AType expected = Helpers.BuildString("Hello");
            AType result = this.engineUni.Execute<AType>("S.- 'Hello'");

            Assert.AreEqual<AType>(expected, result, "Invalid return value");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
    }
}
}
