using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class AbsoluteValue : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("AbsoluteValue"), TestMethod]
        public void AbsoluteValueVector()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(3), AFloat.Create(6), AFloat.Create(2147483648)
            );
            AType result = this.engine.Execute<AType>("| -3 6 -2147483648");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("AbsoluteValue"), TestMethod]
        public void AbsoluteValueFloat()
        {
            AType expected = AFloat.Create(3.1);
            AType result = this.engine.Execute<AType>("| -3.1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("AbsoluteValue"), TestMethod]
        public void AbsoluteValueNull()
        {
            AType result = this.engine.Execute<AType>("| ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Incorrect type");
        }
    }
}
