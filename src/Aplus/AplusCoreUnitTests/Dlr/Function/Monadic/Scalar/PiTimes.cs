using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class PiTimes : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PiTimes"), TestMethod]
        public void PiTimesVector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(0),
                AFloat.Create(Math.PI),
                AFloat.Create(2 * Math.PI),
                AFloat.Create(0.5 * Math.PI),
                AFloat.Create(Double.PositiveInfinity)
            );

            AType result = this.engine.Execute<AType>("pi 0 1 2 .5 1e308");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PiTimes"), TestMethod]
        public void PiTimesVectorUni()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(0),
                AFloat.Create(Math.PI),
                AFloat.Create(2 * Math.PI),
                AFloat.Create(0.5 * Math.PI),
                AFloat.Create(Double.PositiveInfinity)
            );

            AType result = this.engineUni.Execute<AType>("M.^ 0 1 2 .5 1e308");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PiTimes"), TestMethod]
        public void PiTimesNull()
        {
            AType result = this.engine.Execute<AType>("pi ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Incorrect type");
        }
    }
}
