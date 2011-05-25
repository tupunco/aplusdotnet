using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class Exponential : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Exponential"), TestMethod]
        public void ExponentialVector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(Math.Exp(-1)),
                AFloat.Create(1),
                AFloat.Create(Math.E),
                AFloat.Create(Math.Exp(2)),
                AFloat.Create(Double.PositiveInfinity),
                AFloat.Create(Double.PositiveInfinity),
                AFloat.Create(0)
            );
            AType result = this.engine.Execute<AType>("^ -1 0 1 2 710 Inf -Inf");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Exponential"), TestMethod]
        public void ExponentialNull()
        {
            AType result = this.engine.Execute<AType>("^ ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Incorrect type");
        }
    }
}
