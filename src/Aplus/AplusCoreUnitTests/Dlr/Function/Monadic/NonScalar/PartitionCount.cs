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
    public class PartitionCount : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        public void PartitionCountIntegerList1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("bag 1 0 0 0 1 0");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        public void PartitionCountIntegerList2()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("bag 1 0 0 0 1 1 0 1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        public void PartitionCountOneElementVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("bag 1 rho 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        public void PartitionCountScalar()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("bag 7");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        public void PartitionCountNull()
        {
            AType expected = Utils.ANull();
            AType result = this.engine.Execute<AType>("bag ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        public void PartitionCountRestrictedWholeNumber()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(3),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("bag 3.00000000000006 7 4 0 0 2 9 8.00000000000004");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        public void PartitionCountComplexExample()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    Helpers.BuildString("example."),
                    Helpers.BuildString("an "),
                    Helpers.BuildString("is "),
                    Helpers.BuildString("This ")
                }
            );

            AType result = this.engine.Execute<AType>("( bag 1 , a = ' ') bag a := 'This is an example.'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PartitionCountDomainError()
        {
            AType result = this.engine.Execute<AType>("bag 0 0 0 0 1 1 0 1");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PartitionCountRankError()
        {
            AType result = this.engine.Execute<AType>("bag iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("PartitionCount"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void PartitionCountTypeError()
        {
            AType result = this.engine.Execute<AType>("bag <{+}");
        }
    }
}
