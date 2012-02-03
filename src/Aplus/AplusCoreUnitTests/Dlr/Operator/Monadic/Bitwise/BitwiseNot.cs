using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.Hosting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Bitwise
{
    [TestClass]
    public class BitwiseNot : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Bitwise Not"), TestMethod]
        public void BitwiseNotVector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(-2), AInteger.Create(-3), AInteger.Create(-4)
            );

            AType result = this.engine.Execute<AType>("bwnot 1 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Bitwise Not"), TestMethod]
        public void BitwiseNotFloatVector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(-2), AInteger.Create(-3), AInteger.Create(-4)
            );

            AType result = this.engine.Execute<AType>("bwnot 1.0 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Bitwise Not"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void NotRestrictedWholeNumberArgument()
        {
            this.engine.Execute<AType>("bwnot 1 2 3.3");
        }
    }
}
