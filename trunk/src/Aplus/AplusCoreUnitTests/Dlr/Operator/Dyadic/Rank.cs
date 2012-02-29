using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic
{
    [TestClass]
    public class Rank : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeError()
        {
            AType function = AFunc.Create("f", TestUtils.DyadicTypeAlternateFunction, 3, "TypeAlternatingMethod");

            var scope = this.engine.CreateScope();
            scope.SetVariable(".f", function);

            this.engine.Execute<AType>("1 2 3 f @ 0 (1 2 3)", scope);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseAddWithCellScalarVector2Vector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                    AInteger.Create(10),
                    AInteger.Create(9),
                    AInteger.Create(5)
            );

            AType result = this.engine.Execute<AType>("3 4 2 (+ @ 0) 7 5 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseAddVector2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                    AArray.Create(ATypes.AFloat, AFloat.Create(10), AFloat.Create(5)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(2147483655), AFloat.Create(13)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(11), AFloat.Create(5))
            );

            AType result = this.engine.Execute<AType>("3 8 2 (+@ 0 1) 3 2 rho 7 2 2147483647 5 9 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void FloatIntMixedTypeResult1()
        {
            AType expected = AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(1.1));

            AType function = AFunc.Create("f", TestUtils.DyadicTypeAlternateFunction, 3, "TypeAlternatingMethod");

            var scope = this.engine.CreateScope();
            scope.SetVariable(".f", function);

            AType result = this.engine.Execute<AType>("1 0 f @ 0 (1 0)", scope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void FloatIntMixedTypeResult2()
        {
            AType expected = AArray.Create(ATypes.AFloat, AFloat.Create(1.1), AFloat.Create(0));

            AType function = AFunc.Create("f", TestUtils.DyadicTypeAlternateFunction, 3, "TypeAlternatingMethod");

            var scope = this.engine.CreateScope();
            scope.SetVariable(".f", function);

            AType result = this.engine.Execute<AType>("0 1 f @ 0 (0 1)", scope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseCatenateWithCellVectorMatrix2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1),
                    AInteger.Create(2),
                    AInteger.Create(0),
                    AInteger.Create(1),
                    AInteger.Create(2),
                    AInteger.Create(3)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(3),
                    AInteger.Create(4),
                    AInteger.Create(5),
                    AInteger.Create(4),
                    AInteger.Create(5),
                    AInteger.Create(6),
                    AInteger.Create(7)
                )
            );

            AType result = this.engine.Execute<AType>("(iota 2 3) , @ 1 iota 2 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseCatenateWithCellOneNumberCharacterVector2CharacterMatrix()
        {
            AType expected = AArray.Create(ATypes.AChar,
                Helpers.BuildString("-->abcd"),
                Helpers.BuildString("-->ABCD")
            );

            AType result = this.engine.Execute<AType>("'-->' , @ 1 rtack 4 ! 'abcdABCD'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseCatenateWithFrameOneNumberCharacterVector2CharacterMatrix()
        {
            AType expected = AArray.Create(ATypes.AChar,
                Helpers.BuildString("0abcd"),
                Helpers.BuildString("1ABCD")
            );

            AType result = this.engine.Execute<AType>("'01' , @ -1 rtack 4 ! 'abcdABCD'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseCatenateWith2NumberIntegerVector2IntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(4), AInteger.Create(5), AInteger.Create(6), AInteger.Create(7)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(8), AInteger.Create(9), AInteger.Create(10), AInteger.Create(11))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(12), AInteger.Create(13), AInteger.Create(14), AInteger.Create(15)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(16), AInteger.Create(17), AInteger.Create(18), AInteger.Create(19)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(20), AInteger.Create(21), AInteger.Create(22), AInteger.Create(23))
                )
            );

            AType result = this.engine.Execute<AType>("(iota 3) (, @ 0 1) iota 2 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseCatenateWith3NumberIntegerVector2IntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(1990), AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1990), AInteger.Create(3), AInteger.Create(4), AInteger.Create(5))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(1991), AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1991), AInteger.Create(3), AInteger.Create(4), AInteger.Create(5))
                )
            );

            AType result = this.engine.Execute<AType>("(1990 + iota 2) , @ 0 1 0 iota 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseCatenateWith3NumberIntegerArray2IntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3),
                        AInteger.Create(0), AInteger.Create(1), AInteger.Create(2), AInteger.Create(3), AInteger.Create(4)),
                    AArray.Create(
                        ATypes.AInteger, AInteger.Create(4), AInteger.Create(5), AInteger.Create(6), AInteger.Create(7),
                        AInteger.Create(5), AInteger.Create(6), AInteger.Create(7), AInteger.Create(8), AInteger.Create(9)),
                    AArray.Create(
                        ATypes.AInteger, AInteger.Create(8), AInteger.Create(9), AInteger.Create(10), AInteger.Create(11),
                        AInteger.Create(10), AInteger.Create(11), AInteger.Create(12), AInteger.Create(13), AInteger.Create(14))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger, AInteger.Create(12), AInteger.Create(13), AInteger.Create(14), AInteger.Create(15),
                        AInteger.Create(15), AInteger.Create(16), AInteger.Create(17), AInteger.Create(18), AInteger.Create(19)),
                    AArray.Create(
                        ATypes.AInteger, AInteger.Create(16), AInteger.Create(17), AInteger.Create(18), AInteger.Create(19),
                        AInteger.Create(20), AInteger.Create(21), AInteger.Create(22), AInteger.Create(23), AInteger.Create(24)),
                    AArray.Create(
                        ATypes.AInteger, AInteger.Create(20), AInteger.Create(21), AInteger.Create(22), AInteger.Create(23),
                        AInteger.Create(25), AInteger.Create(26), AInteger.Create(27), AInteger.Create(28), AInteger.Create(29))
                )
            );

            AType result = this.engine.Execute<AType>("(iota 2 3 4) , @ 1 1 2 (iota 2 3 5)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseCatenateWith3NumberCharacterArray2CharacterArray()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(
                        ATypes.AChar,
                        AArray.Create(
                            ATypes.AChar,
                            Helpers.BuildString("my hat "),
                            Helpers.BuildString("you did"),
                            Helpers.BuildString(" it is ")
                        ),
                        AArray.Create(
                            ATypes.AChar,
                            Helpers.BuildString("my win "),
                            Helpers.BuildString("yours  "),
                            Helpers.BuildString(" it's  ")
                        )
                    ),
                    AArray.Create(
                        ATypes.AChar,
                        AArray.Create(
                            ATypes.AChar,
                            Helpers.BuildString("  that "),
                            Helpers.BuildString(" he did"),
                            Helpers.BuildString("dog is ")
                        ),
                        AArray.Create(
                            ATypes.AChar,
                            Helpers.BuildString("  twin "),
                            Helpers.BuildString(" hers  "),
                            Helpers.BuildString("dog's  ")
                        )
                    )
            );

            AType result = this.engine.Execute<AType>("(2 3 3 rho 'my you it  t hedog') , @ 1 1 1 (2 3 4 rho \"hat  did is win rs  's  \")");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseAsOuterProduct()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(1.2), AFloat.Create(-3), AFloat.Create(98.2), AFloat.Create(5)),
                AArray.Create(ATypes.AFloat, AFloat.Create(12), AFloat.Create(-30), AFloat.Create(982), AFloat.Create(50)),
                AArray.Create(ATypes.AFloat, AFloat.Create(120), AFloat.Create(-300), AFloat.Create(9820), AFloat.Create(500))
            );

            AType result = this.engine.Execute<AType>("1 10 100 (*@ 0 0 0) 1.2 -3 98.2 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankUseAsInnerProduct()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a pdt b : +/ a * b", scope);
            this.engine.Execute<AType>("y InnerProduct x : y (pdt @ 1 1 0)(-1 rot iota rho rho x) flip x", scope);


            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(20), AInteger.Create(23), AInteger.Create(26), AInteger.Create(29)),
                AArray.Create(ATypes.AInteger, AInteger.Create(56), AInteger.Create(68), AInteger.Create(80), AInteger.Create(92))
            );

            AType result = this.engine.Execute<AType>("InnerProduct{iota 2 3; iota 3 4}", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankNullCase1()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("y f x : x", scope);

            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("2 f @ 1 iota 0", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        public void RankNullCase2()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("y f x : x", scope);

            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("2 f @ 0 iota 0", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainError()
        {
            this.engine.Execute<AType>("(iota 2 4 4) + @ 1 2 -1 (iota 2 3 9)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.NonFunction))]
        public void NonFunctionError()
        {
            this.engine.Execute<AType>("(iota 2 4 4) () @ 1 1 (iota 2 3 9)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Mismatch))]
        public void RankMismatchError1()
        {
            this.engine.Execute<AType>("(iota 2 4 4) , @ 1 1 (iota 2 3 9)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeError1()
        {
            this.engine.Execute<AType>("(iota 2 4 4) , @ 1.1 (iota 2 3 9)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LengthError1()
        {
            this.engine.Execute<AType>("(iota 2 4 4) , @ (iota 2 2) (iota 2 3 9)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Mismatch))]
        public void RankMismatchError2()
        {
            this.engine.Execute<AType>("(iota 24) (+ @ 0) iota 23");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void RankLengthError1()
        {
            this.engine.Execute<AType>("(iota 24) (+ @ 1) iota 23");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rank"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void RankValenceError1()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f a : a+a", scope);

            this.engine.Execute<AType>("3 (f@ 0) 2", scope);
        }
    }
}
