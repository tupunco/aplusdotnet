using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Left : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Left"), TestMethod]
        public void LeftFunction()
        {
            AType expected = (new int[] { 3, 4, 5 }).ToAArray();
            AType result = this.engine.Execute<AType>("3 4 5 where 'test'");

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Left"), TestMethod]
        public void LeftFunctionUni()
        {
            AType expected = (new int[] { 3, 4, 5 }).ToAArray();
            AType result = this.engineUni.Execute<AType>("3 4 5 A.< 'test'");

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
    }

}
}
