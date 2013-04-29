using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class NaturalLog : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("NaturalLog"), TestMethod]
        public void NaturalLogInteger()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(Math.Log(2)),
                AFloat.Create(Math.Log(5)),
                AFloat.Create(Math.Log(25))
            );
            AType result = this.engine.Execute<AType>("log 2 5 25");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("NaturalLog"), TestMethod]
        public void NaturalLogIntegerUni()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(Math.Log(2)),
                AFloat.Create(Math.Log(5)),
                AFloat.Create(Math.Log(25))
            );
            AType result = this.engineUni.Execute<AType>("M.& 2 5 25");

            Assert.AreEqual(expected, result);
        }

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
