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
    public class NaturalLog : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("NaturalLog"), TestMethod]
        public void NaturalLogVector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(0),
                AFloat.Create(Math.Log(10)),
                AFloat.Create(Math.Log(100)),
                AFloat.Create(Double.NegativeInfinity),
                AFloat.Create(Double.PositiveInfinity)
            );
            AType result = this.engine.Execute<AType>("log 1 10 100 0 Inf");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("NaturalLog"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void NaturalLogDomainError()
        {
            AType result = this.engine.Execute<AType>("log -5");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("NaturalLog"), TestMethod]
        public void NaturalLogNull()
        {
            AType result = this.engine.Execute<AType>("log ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Incorrect type");
        }
    }
}
