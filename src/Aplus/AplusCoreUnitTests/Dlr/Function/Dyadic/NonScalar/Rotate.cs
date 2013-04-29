using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Runtime;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Rotate : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotatePositiveInteger2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(5),
                AInteger.Create(8),
                AInteger.Create(2)
            );
            AType result = this.engine.Execute<AType>("5 rot 2 3 5 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotatePositiveInteger2IntegerListUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(5),
                AInteger.Create(8),
                AInteger.Create(2)
            );
            AType result = this.engineUni.Execute<AType>("5 S.| 2 3 5 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotateNegativeInteger2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(5),
                AInteger.Create(3),
                AInteger.Create(4)
            );
            AType result = this.engine.Execute<AType>("-5 rot 4 5 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotateNull2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>("0 rot 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotatePositiveInteger2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(3),
                    AInteger.Create(4),
                    AInteger.Create(5)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1),
                    AInteger.Create(2)
                )
            );
            AType result = this.engine.Execute<AType>("1 rot iota 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotateIntegerList2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(10),
                    AInteger.Create(5)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(3),
                    AInteger.Create(13),
                    AInteger.Create(8)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(6),
                    AInteger.Create(1),
                    AInteger.Create(11)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(4),
                    AInteger.Create(14)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(12),
                    AInteger.Create(7),
                    AInteger.Create(2)
                )
            );
            AType result = this.engine.Execute<AType>("0 -2 1 rot iota 5 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotatematrixMatrix2matrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(0),
                        AInteger.Create(5)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(10),
                        AInteger.Create(15)
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(4),
                        AInteger.Create(9)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(14),
                        AInteger.Create(3)
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(8),
                        AInteger.Create(13)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(2),
                        AInteger.Create(7)
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(12),
                        AInteger.Create(1)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(6),
                        AInteger.Create(11)
                    )
                )
            );
            AType result = this.engine.Execute<AType>("(iota 2 2) rot iota 4 2 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotatePositiveInteger2Char()
        {
            AType expected = AChar.Create('a');

            AType result = this.engine.Execute<AType>("3 rot 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotateNegativeInteger2OneElementVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(6)
            );

            AType result = this.engine.Execute<AType>("-2 rot 1 rho 6");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotateNull2SymbolConstant()
        {
            AType expected = ASymbol.Create("test");

            AType result = this.engine.Execute<AType>("() rot `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void RotateIntegerList2NestedMixedMatrix()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ABox,
                AArray.Create(
                    ATypes.ABox,
                    ABox.Create(AInteger.Create(2)),
                    ASymbol.Create("b")
                ),
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("a"),
                    f
                ),
                AArray.Create(
                    ATypes.AFunc,
                    f,
                    ABox.Create(AInteger.Create(4))
                )
            );

            AType result = this.engine.Execute<AType>("-1 1 rot 3 2 rho `a , (<4) , f , `b , (<2) , f", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RotateLengthError1()
        {
            AType result = this.engine.Execute<AType>("3 1 rot iota 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RotateLengthError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) rot iota 4 3 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RotateLengthError3()
        {
            AType result = this.engine.Execute<AType>("() rot iota 2 1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RotateRankError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) rot iota 4 2 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RotateRankError2()
        {
            AType result = this.engine.Execute<AType>("3 3 rot ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RotateRankError3()
        {
            AType result = this.engine.Execute<AType>("() rot 2 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void RotateLeftFloatTypeError()
        {
            AType result = this.engine.Execute<AType>("2.2 rot 2");
        }
    }
}
