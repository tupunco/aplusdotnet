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
    public class Interval : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        public void IntervalEmptyVector()
        {
            AType expected = AArray.Create(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("iota 0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        public void IntervalInteger()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), 
                AInteger.Create(2), AInteger.Create(3), 
                AInteger.Create(4), AInteger.Create(5)
            );
            AType result = this.engine.Execute<AType>("iota 6");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        public void IntervalIntegerUni()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1),
                AInteger.Create(2), AInteger.Create(3),
                AInteger.Create(4), AInteger.Create(5)
            );
            AType result = this.engineUni.Execute<AType>("I.# 6");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void IntervalFloat()
        {
            AType result = this.engine.Execute<AType>("iota 2.2");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        public void IntervalFloatTolerablyInteger()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>("iota 4.0");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        public void IntervalVector()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4), AInteger.Create(5)),
                AArray.Create(ATypes.AInteger, AInteger.Create(6), AInteger.Create(7), AInteger.Create(8))
            );
            AType result = this.engine.Execute<AType>("iota 3 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void IntervalMatrix()
        {
            AType result = this.engine.Execute<AType>("iota iota 2 2");
        }


        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void IntervalInvalidType()
        {
            this.engine.Execute<AType>(" iota 'hello'");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void IntervalInvalidBoxType()
        {
            this.engine.Execute<AType>(" iota (1;2)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void IntervalInvalidFloatArgument()
        {
            this.engine.Execute<AType>("iota 123456789124");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.MaxRank))]
        public void IntervalMaxRank()
        {
            this.engine.Execute<AType>("iota 1 2 3 4 5 6 7 8 9 1 2 4");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void IntervalNegativeArrayArgument()
        {
            this.engine.Execute<AType>("iota 2 3 -3");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Interval"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void IntervalNegativeArgument()
        {
            this.engine.Execute<AType>("iota -3.0");
        }
    }
}
