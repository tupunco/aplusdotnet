using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic
{
    [TestClass]
    public class Rank : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseOnNull()
        {
            AType expected = this.engine.Execute<AType>("<()");
            AType result = this.engine.Execute<AType>("(< @ 1) ()");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeError()
        {
            AType function = AFunc.Create("f", TestUtils.MonadicTypeAlternateFunction, 2, "TypeAlternatingMethod");

            var scope = this.engine.CreateScope();
            scope.SetVariable(".f", function);

            this.engine.Execute<AType>("f @ 0 (1 2 3)", scope);
        }
        
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseReduceAddWithCellScalar1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(60),
                AInteger.Create(6)
            );

            AType result = this.engine.Execute<AType>("(+/@1) 2 3 rho 10 20 30 1 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseReduceAddWithCellScalar2()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(11),
                AInteger.Create(22),
                AInteger.Create(33)
            );

            AType result = this.engine.Execute<AType>("(+/@4) 2 3 rho 10 20 30 1 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseReverseWithCellVector()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("Hello"),
                Helpers.BuildString("World")
            );

            AType result = this.engine.Execute<AType>("(rot @ 1 5 6) 2 5 rho 'olleHdlroW'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseReverseWithCellScalar()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(2), AInteger.Create(1), AInteger.Create(0)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(7), AInteger.Create(6), AInteger.Create(5), AInteger.Create(4)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(11), AInteger.Create(10), AInteger.Create(9), AInteger.Create(8))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(15), AInteger.Create(14), AInteger.Create(13), AInteger.Create(12)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(19), AInteger.Create(18), AInteger.Create(17), AInteger.Create(16)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(23), AInteger.Create(22), AInteger.Create(21), AInteger.Create(20))
                )
            );

            AType result = this.engine.Execute<AType>("rot @ 1 iota 2 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseReverseWithFrameScalar()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(8), AInteger.Create(9), AInteger.Create(10), AInteger.Create(11)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(5), AInteger.Create(6), AInteger.Create(7)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(20), AInteger.Create(21), AInteger.Create(22), AInteger.Create(23)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(16), AInteger.Create(17), AInteger.Create(18), AInteger.Create(19)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(12), AInteger.Create(13), AInteger.Create(14), AInteger.Create(15))
                )
            );

            AType result = this.engine.Execute<AType>("rot @ -1 iota 2 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseUserDefinedFunctionWithCellScalar1()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b} : (+/b) > 15", scope);

            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("a @ 1 iota 3 4", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseUserDefinedFunctionWithCellScalar2()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b} : (+/b) > 15", scope);

            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(0))
            );

            AType result = this.engine.Execute<AType>("a @ 0 rtack 2 2 rho 54 4 65 3", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankComplexTest()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                Helpers.BuildStrand(
                    new AType[]{
                        AArray.Create(ATypes.AInteger, AInteger.Create(2)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(0)),
                    }
                ),
                Helpers.BuildStrand(
                    new AType[]{
                        AArray.Create(ATypes.AInteger, AInteger.Create(1)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(0)),
                    }
                ),
                Helpers.BuildStrand(
                    new AType[]{
                        AArray.Create(ATypes.AInteger, AInteger.Create(3)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(0)),
                    }
                )
            );

            AType result = this.engine.Execute<AType>("rho each (rho each) @ -2 rtack 3 2 rho (4; iota 2 3; 2; iota 5; 7; iota 2 3 4)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseReduceAddWithCellVectorDifferentTypeResult()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(6),
                AFloat.Create(2147483653)
                );

            AType result = this.engine.Execute<AType>("(+/ @ 1) 2 2 rho 4 2 6 2147483647 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseReduceAddWithCellArrayDifferentTypeResult()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 2 3 4 rho (iota 18) , 2147483647", scope);

            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(6), AFloat.Create(22), AFloat.Create(38)),
                AArray.Create(ATypes.AFloat, AFloat.Create(54), AFloat.Create(2147483680), AFloat.Create(10))
                );

            AType result = this.engine.Execute<AType>("(+/ @ 1) a", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseNonScalarFunctionWithCellScalar()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("== @ 1 ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankUseScalarFunctionWithCellNullArray()
        {
            //Original interpreter sets type to Integer.
            AType expected = AArray.Create(ATypes.ANull);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 3, 4 };
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>("(rot @ 2) 0 3 4 rho 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        public void RankofRank()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(3), AInteger.Create(5), AInteger.Create(7)
            );

            AType result = this.engine.Execute<AType>("1 2 3 +@0@0 (2 3 4)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.NonFunction))]
        public void RankNonFunctionError()
        {
            this.engine.Execute<AType>("(`sym @ 1) iota 5");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void RankTypeError1()
        {
            this.engine.Execute<AType>("(rot @ 1.00000000000005) 2 5 rho 'olleHdlroW'");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void RankTypeError2()
        {
            this.engine.Execute<AType>("form @ 0 ()");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RankLengthError()
        {
            this.engine.Execute<AType>("(rot @ 1 5 6 7 3) 2 5 rho 'olleHdlroW'");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void RankValenceError1()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b;c} : b+c", scope);
            this.engine.Execute<AType>("a @ 1 iota 2 2", scope);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void RankValenceError2()
        {
            this.engine.Execute<AType>("/ @ 1 iota 2 2");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RankRankError()
        {
            this.engine.Execute<AType>("form @ 1 ()");
        }
    }
}
