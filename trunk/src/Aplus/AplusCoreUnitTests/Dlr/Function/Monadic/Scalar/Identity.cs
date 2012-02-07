using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

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

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Identity"), TestMethod]
        public void IdentityChar()
        {
            AType expected = Helpers.BuildString("abc");
            AType result = this.engine.Execute<AType>("+ 'abc'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Identity"), TestMethod]
        public void IdentitySymbol()
        {
            AType expected = ASymbol.Create("abc");
            AType result = this.engine.Execute<AType>("+ `abc");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Identity"), TestMethod]
        public void IdentityBox()
        {
            AType expected = ABox.Create(AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)));
            AType result = this.engine.Execute<AType>("+ <1 2 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Identity"), TestMethod]
        public void IdentifyFunction()
        {
            AType expected = this.engine.Execute<AType>("+");
            AType result = this.engine.Execute<AType>("+ {+}");

            Assert.AreEqual(expected, result);
        }
    }
}
