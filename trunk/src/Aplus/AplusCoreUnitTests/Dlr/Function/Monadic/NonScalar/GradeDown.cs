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
    public class GradeDown : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownIntegerVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(3),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(4)
            );
            AType result = this.engine.Execute<AType>("dng 7 4 2 5 1");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownIntegerVectorUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(3),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(4)
            );
            AType result = this.engineUni.Execute<AType>("I.- 7 4 2 5 1");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownIntegerVectorWithDuplicateElement()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(0),
                AInteger.Create(2),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("dng 4 2 4 5");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(2),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("dng 3 3 rho 6 2 9 3 6 7 5 3 2");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownTestComprasionTolarence()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("dng (7+4e-13), 7");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownFloatListWithDuplicateElement()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(4),
                AInteger.Create(3)
            );

            AType result = this.engine.Execute<AType>("dng 10.2 6 999 0 6");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownCharacterConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(3),
                AInteger.Create(2),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("dng 'test'");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        public void GradeDownSymbolConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(0),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("dng `apl `a `apple");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void GradeDownRankError1()
        {
            AType result = this.engine.Execute<AType>("dng 4");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void GradeDownRankError2()
        {
            AType result = this.engine.Execute<AType>("dng <4");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GradeDownTypeError1()
        {
            AType result = this.engine.Execute<AType>("dng (2;6)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("GradeDown"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void GradeDownTypeError2()
        {
            AType result = this.engine.Execute<AType>("dng `a`b , <{+}");
        }
    }
}
