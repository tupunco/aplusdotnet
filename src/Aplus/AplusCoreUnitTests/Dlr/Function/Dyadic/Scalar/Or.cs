using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class Or : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrVector2Integer()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("0 0 5 1 ? 0 9 0 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrNull2Integer()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("() ? 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        public void OrNull2Null()
        {
            AType result = this.engine.Execute<AType>("() ? ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void OrTypeError1()
        {
            AType result = this.engine.Execute<AType>("3.4 ? 1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Or"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void OrTypeError2()
        {
            AType result = this.engine.Execute<AType>("1 ? <6");
        }
    }
}
