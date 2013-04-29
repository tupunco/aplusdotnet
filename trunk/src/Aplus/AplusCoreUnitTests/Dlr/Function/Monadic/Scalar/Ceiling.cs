using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.Scalar
{
    [TestClass]
    public class Ceiling : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ceiling"), TestMethod]
        public void CeilingVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(10),
                AInteger.Create(11),
                AInteger.Create(11),
                AInteger.Create(11),
                AInteger.Create(-9),
                AInteger.Create(-9),
                AInteger.Create(-9),
                AInteger.Create(-9),
                AInteger.Create(1),
                AInteger.Create(3)
            );

            AType result = this.engine.Execute<AType>("max 10 10.2 10.5 10.98 -9 -9.2 -9.5 -9.98 0.1 3.000000000000000000000002");

            Assert.AreEqual(expected.Type, result.Type, "Type mismatch");
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ceiling"), TestMethod]
        public void CeilingVectorUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(10),
                AInteger.Create(11),
                AInteger.Create(11),
                AInteger.Create(11),
                AInteger.Create(-9),
                AInteger.Create(-9),
                AInteger.Create(-9),
                AInteger.Create(-9),
                AInteger.Create(1),
                AInteger.Create(3)
            );

            AType result = this.engineUni.Execute<AType>("M.+ 10 10.2 10.5 10.98 -9 -9.2 -9.5 -9.98 0.1 3.000000000000000000000002");

            Assert.AreEqual(expected.Type, result.Type, "Type mismatch");
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ceiling"), TestMethod]
        public void CeilingNull()
        {
            AType result = this.engine.Execute<AType>("max ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ceiling"), TestMethod]
        public void CeilingNullUni()
        {
            AType result = this.engineUni.Execute<AType>("M.+ ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ceiling"), TestMethod]
        public void CeilingVectorIntegerArgument()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(-9),
                AInteger.Create(-9)
            );

            AType result = this.engine.Execute<AType>("max 10 10 10 10 -9 -9");

            Assert.AreEqual(expected.Type, result.Type, "Type mismatch");
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ceiling"), TestMethod]
        public void CeilingVectorIntegerArgumentUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(-9),
                AInteger.Create(-9)
            );

            AType result = this.engineUni.Execute<AType>("M.+ 10 10 10 10 -9 -9");

            Assert.AreEqual(expected.Type, result.Type, "Type mismatch");
            Assert.AreEqual(expected, result);
    }
}
}
