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
    public class Reciprocal : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reciprocal"), TestMethod]
        public void ReciprocalVector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(2),
                AFloat.Create(1/1.5),
                AFloat.Create(-.5),
                AFloat.Create(0.01),
                AFloat.Create(Double.PositiveInfinity),
                AFloat.Create(Double.NegativeInfinity),
                AFloat.Create(0),
                AFloat.Create(0)
            );

            AType result = this.engine.Execute<AType>("% .5 1.5 -2 100 0 -1e-309 Inf -Inf");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reciprocal"), TestMethod]
        public void ReciprocalNull()
        {
            AType result = this.engine.Execute<AType>("% ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reciprocal"), TestMethod]
        [ExpectedException(typeof(Error.NonData))]
        public void ReciprocalFunction()
        {
            this.engine.Execute("% {+}");
        }
    }
}
