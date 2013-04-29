using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Match : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Match"), TestMethod]
        public void MatchInt2Float()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("1.0000000000001 == 1");

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Match"), TestMethod]
        public void MatchInt2FloatUni()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engineUni.Execute<AType>("1.0000000000001 == 1");

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Match"), TestMethod]
        public void MatchInt2FloatFail()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1.2 == 1");

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Match"), TestMethod]
        public void MatchInt2CharFail()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1 == 'hello'");

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Match"), TestMethod]
        public void MatchSymbol2BoxFail()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("`a == (1;2)");

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

    }
}
