using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Count : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountScalarInteger()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("# 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountScalarIntegerUni()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engineUni.Execute<AType>("# 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountScalarFloat()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("# 10.0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountVector()
        {
            AType expected = AInteger.Create(3);
            AType result = this.engine.Execute<AType>("# 5 8 7.0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountVectorUni()
        {
            AType expected = AInteger.Create(3);
            AType result = this.engineUni.Execute<AType>("# 5 8 7.0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountString()
        {
            AType expected = AInteger.Create(5);
            AType result = this.engine.Execute<AType>("# 'hello'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountMatrix()
        {
            AType expected = AInteger.Create(2);
            AType result = this.engine.Execute<AType>("# iota 2 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountNull()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("# ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountStrand()
        {
            AType expected = AInteger.Create(3);
            AType result = this.engine.Execute<AType>("# (1 4 5;2 8.0;'hello')");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Count"), TestMethod]
        public void CountStrand2()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("# (1;2;'bee')");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));

            result = this.engine.Execute<AType>("# (1;2;('bee';'good'))");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
