using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Shape : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeNull()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(0));
            AType result = this.engine.Execute<AType>("rho ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeScalar()
        {
            AType expected = AArray.Create(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("rho 1");
            Assert.AreEqual(expected, result, "Shape of integer failed");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));


            result = this.engine.Execute<AType>("rho 56.54");
            Assert.AreEqual(expected, result, "Shape of float failed");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));


            result = this.engine.Execute<AType>("rho 'd'");
            Assert.AreEqual(expected, result, "Shape of char failed");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeScalarUni()
        {
            AType expected = AArray.Create(ATypes.AInteger);

            AType result = this.engineUni.Execute<AType>("S.? 1");
            Assert.AreEqual(expected, result, "Shape of integer failed");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));


            result = this.engineUni.Execute<AType>("S.? 56.54");
            Assert.AreEqual(expected, result, "Shape of float failed");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));


            result = this.engineUni.Execute<AType>("S.? 'd'");
            Assert.AreEqual(expected, result, "Shape of char failed");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeVector()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(4));
            AType result = this.engine.Execute<AType>("rho 5 6.2 7 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeVectorUni()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(4));
            AType result = this.engineUni.Execute<AType>("S.? 5 6.2 7 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeString()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(8));
            AType result = this.engine.Execute<AType>("rho 'hello A+'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeMatrix()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(5));
            AType result = this.engine.Execute<AType>("rho iota 3 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Shape"), TestMethod]
        public void ShapeStrand()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(3));

            AType result = this.engine.Execute<AType>("rho (1;2;'bee')");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));

            result = this.engine.Execute<AType>("rho (1;2;('bee';'good'))");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
