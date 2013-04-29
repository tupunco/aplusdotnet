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
    public class Decode : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeInteger2IntegerList()
        {
            AType expected = AInteger.Create(33);

            AType result = this.engine.Execute<AType>("2 pack 4 7 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeInteger2IntegerListUni()
        {
            AType expected = AInteger.Create(33);

            AType result = this.engineUni.Execute<AType>("2 M.< 4 7 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeFloat2FloatList()
        {
            AType expected = AFloat.Create(33);

            AType result = this.engine.Execute<AType>("2 pack 4. 7 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeIntegerList2IntegerList()
        {
            AType expected = AInteger.Create(7559);

            AType result = this.engine.Execute<AType>("24 60 60 pack 2 5 59");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeInteger2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(24),
                AInteger.Create(45),
                AInteger.Create(66),
                AInteger.Create(87)
            );

            AType result = this.engine.Execute<AType>("4 pack iota 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeIntegerList2IntegerListWithIntMaxValue()
        {
            AType expected = AFloat.Create(4294967297);

            AType result = this.engine.Execute<AType>("1 2 pack 2147483647 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeIntegerList2IntegerMatrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(48),AInteger.Create(61)),
                AArray.Create(ATypes.AInteger, AInteger.Create(74), AInteger.Create(87)),
                AArray.Create(ATypes.AInteger, AInteger.Create(100), AInteger.Create(113)),
                AArray.Create(ATypes.AInteger, AInteger.Create(126), AInteger.Create(139))
            );

            AType result = this.engine.Execute<AType>("3 2 4 pack iota 3 4 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeFloatList2IntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(
                    ATypes.AFloat,
                    AArray.Create(ATypes.AFloat, AFloat.Create(64), AFloat.Create(76)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(88), AFloat.Create(100)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(112), AFloat.Create(124)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(136), AFloat.Create(148))
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AArray.Create(ATypes.AFloat, AFloat.Create(160), AFloat.Create(172)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(184), AFloat.Create(196)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(208), AFloat.Create(220)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(232), AFloat.Create(244))
                )
            );

            AType result = this.engine.Execute<AType>("3 4.5 2 pack iota 3 2 4 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeIntegerList2FloatMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(22.8),
                AFloat.Create(28.9),
                AFloat.Create(20)
            );

            AType result = this.engine.Execute<AType>("3 4 pack 2 3 rho 4.2 6.6 3 6 2.5 8");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeOneElementIntegerList2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(6),
                AInteger.Create(9)
            );

            AType result = this.engine.Execute<AType>("(1 rho 2) pack iota 2 3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeInteger2Null()
        {
            AType expected = AFloat.Create(0);

            AType result = this.engine.Execute<AType>("3 pack ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeNull2Null()
        {
            AType expected = AFloat.Create(0);

            AType result = this.engine.Execute<AType>("() pack ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        public void DecodeOneElementIntegerList2OneElementFloatList()
        {
            AType expected = AFloat.Create(7.6);

            AType result = this.engine.Execute<AType>("2 pack 1 rho 7.6");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DecodeTypeError1()
        {
            AType result = this.engine.Execute<AType>("`s pack 23 4 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DecodeTypeError2()
        {
            AType result = this.engine.Execute<AType>("2 pack < 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DecodeRankError1()
        {
            AType result = this.engine.Execute<AType>("2 3 pack 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DecodeRankError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) pack 2 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DecodeRankError3()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) pack ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DecodeRankError4()
        {
            AType result = this.engine.Execute<AType>("() pack 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void DecodeLengthError1()
        {
            AType result = this.engine.Execute<AType>("2 3 pack 3 5 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void DecodeLengthError2()
        {
            AType result = this.engine.Execute<AType>("2 3 pack iota 3 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void DecodeLengthError3()
        {
            AType result = this.engine.Execute<AType>("2 3 1 pack ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Decode"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void DecodeLengthError4()
        {
            AType result = this.engine.Execute<AType>("() pack iota 2 3");
        }
    }
}
