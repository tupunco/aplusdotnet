using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.Hosting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class Negate : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Negate"), TestMethod]
        public void NegateInteger()
        {
            AType expected = AInteger.Create(-1);
            AType result = this.engine.Execute<AType>(" - 1");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.asInteger == expected.asInteger);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Negate"), TestMethod]
        public void NegateFloat()
        {
            AType expected = AFloat.Create(2.0);
            AType result = this.engine.Execute<AType>(" - -2.0");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.asFloat == expected.asFloat);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Negate"), TestMethod]
        public void NegateVector1()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(-1), AInteger.Create(-2), AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>(" - 1 2 -3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Negate"), TestMethod]
        public void NegateVector2()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(2147483648),
                AFloat.Create(-2),
                AFloat.Create(-1),
                AFloat.Create(3)
            );
            AType result = this.engine.Execute<AType>(" - -2147483648 2 1 -3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Negate"), TestMethod]
        public void NegateMatrix()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(-1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(-2), AInteger.Create(-3))
            );
            AType result = this.engine.Execute<AType>(" - iota 2 2");

            Assert.AreEqual(expected, result);
        }


        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Negate"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void NegateInvalidType()
        {
            this.engine.Execute<AType>(" - 'hello'");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Negate"), TestMethod]
        [ExpectedException(typeof(Error.NonData))]
        public void NegateFunction()
        {
            this.engine.Execute<AType>(" - {+}");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Neagate"), TestMethod]
        public void NegateNull()
        {
            AType result = this.engine.Execute<AType>("- ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Incorrect type");
        }
    }
}
