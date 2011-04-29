using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Transpose : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Transpose"), TestMethod]
        public void TransposeMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(4)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(5))
            );

            AType result = this.engine.Execute<AType>("flip iota 2 3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Transpose"), TestMethod]
        public void TransposeMatrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(3)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(4)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(5))
                )
            );

            AType result = this.engine.Execute<AType>("flip iota 2 3 1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Transpose"), TestMethod]
        public void TransposeNull()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("flip ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Transpose"), TestMethod]
        public void TransposeSymbolConstant()
        {
            AType expected = ASymbol.Create("test");

            AType result = this.engine.Execute<AType>("flip `test");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }
    }
}
