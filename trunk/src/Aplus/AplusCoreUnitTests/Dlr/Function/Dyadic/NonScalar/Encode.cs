using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Encode : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeInteger2Integer()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("5 unpack 7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeInteger2IntegerUni()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engineUni.Execute<AType>("5 M.> 7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeIntegerList2Integer1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("4 2 6 3 unpack 401");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeIntegerList2Integer2()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(5),
                AInteger.Create(59)
            );

            AType result = this.engine.Execute<AType>("24 60 60 unpack 7559");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeIntegerListStartsWith0ToInteger2()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("0 100 unpack 101");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeIntegerListContains0ToInteger2()
        {
            // corner case, the last 0 will allways contain the remaining part
            // eg. 24 0 60 60 unpack 7200 will result 0 2 0 0
            
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("24 0  60 60 unpack 3600");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeInteger2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(5),
                AInteger.Create(0),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("6 unpack 401 456 -664");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeIntegerList2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(3)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(0)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(-5), AInteger.Create(-1)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(-5), AInteger.Create(-1))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(0)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(0))
                )
            );

            AType result = this.engine.Execute<AType>("4 2 -6 3 unpack 2 2 rho 401 321 563 123");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeNegativeInteger2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(-2),
                AInteger.Create(-4),
                AInteger.Create(-3)
            );

            AType result = this.engine.Execute<AType>("-5 unpack 53 -34 52");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeIntegerList2NegativeIntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(21), AInteger.Create(22), AInteger.Create(21)),
                AArray.Create(ATypes.AInteger, AInteger.Create(15), AInteger.Create(21), AInteger.Create(4))
            );

            AType result = this.engine.Execute<AType>("23 34 unpack -53 -13 -64");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeNegativeIntegerList2NegativeIntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(-40), AInteger.Create(-39)),
                AArray.Create(ATypes.AInteger, AInteger.Create(-4), AInteger.Create(-2), AInteger.Create(-10), AInteger.Create(-7))
            );

            AType result = this.engine.Execute<AType>("-42 -23 unpack -4 -2 -56 -76");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeFloatList2FloatList()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(2), AFloat.Create(0), AFloat.Create(0)),
                AArray.Create(ATypes.AFloat, AFloat.Create(1), AFloat.Create(0), AFloat.Create(0)),
                AArray.Create(ATypes.AFloat, AFloat.Create(6), AFloat.Create(3.2), AFloat.Create(6))
            );

            AType result = this.engine.Execute<AType>("4.4 2.5 6.5 unpack 45 3.2 6");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        public void EncodeInteger2Null()
        {
            AType expected = Utils.ANull(ATypes.AFloat);

            AType result = this.engine.Execute<AType>(" 3 unpack ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void EncodeRankError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) unpack ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Encode"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void EncodeTypeError1()
        {
            AType result = this.engine.Execute<AType>("3 unpack < 6");
        }
    }
}
