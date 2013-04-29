using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class GradeUp : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpIntegerVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(2),
                AInteger.Create(1),
                AInteger.Create(3),
                AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("upg 7 4 2 5 1");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpIntegerVectorUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(2),
                AInteger.Create(1),
                AInteger.Create(3),
                AInteger.Create(0)
            );
            AType result = this.engineUni.Execute<AType>("I.+ 7 4 2 5 1");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpIntegerVectorWithDuplicateElement()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>("upg 4 2 4 5");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("upg 3 3 rho 6 2 9 3 6 7 5 3 2");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpTestComprasionTolarence()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("upg (7+4e-13), 7");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpFloatListWithDuplicateElement()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(1),
                AInteger.Create(4),
                AInteger.Create(0),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("upg 10.2 6 999 0 6");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpCharacterConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(0),
                AInteger.Create(3)
            );

            AType result = this.engine.Execute<AType>("upg 'test'");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        public void GradeUpSymbolConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("upg `apl `a `apple");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void GradeUpRankError1()
        {
            AType result = this.engine.Execute<AType>("upg 4");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void GradeUpRankError2()
        {
            AType result = this.engine.Execute<AType>("upg <4");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GradeUpTypeError1()
        {
            AType result = this.engine.Execute<AType>("upg (2;6)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeUp"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GradeUpTypeError2()
        {
            AType result = this.engine.Execute<AType>("upg (<{+}) , `a`b");
        }
    }
}
