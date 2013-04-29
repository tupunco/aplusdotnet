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
    public class Laminate : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateIntegerList2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(2),
                    AInteger.Create(3),
                    AInteger.Create(4)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(5),
                    AInteger.Create(2),
                    AInteger.Create(2)
                )
            );
            AType result = this.engine.Execute<AType>("2 3 4 ~ 5 2 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateIntegerList2IntegerListUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(2),
                    AInteger.Create(3),
                    AInteger.Create(4)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(5),
                    AInteger.Create(2),
                    AInteger.Create(2)
                )
            );
            AType result = this.engineUni.Execute<AType>("2 3 4 ! 5 2 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateFloatToFloat()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(2.4),
                AFloat.Create(5.9)
            );
            AType result = this.engine.Execute<AType>("2.4 ~ 5.9");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateCharacterConstant2CharacterConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("aaaa"),
                Helpers.BuildString("test")
            );
            AType result = this.engine.Execute<AType>("'a' ~ 'test'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateSymbolConstantList2SymbolConstant()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("s1"),
                    ASymbol.Create("s2")
                ),
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("test"),
                    ASymbol.Create("test")
                )
            );
            AType result = this.engine.Execute<AType>("`s1 `s2 ~ `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateBox2Box()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(AInteger.Create(5)),
                ABox.Create(
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(2)
                    )
                )

            );
            AType result = this.engine.Execute<AType>("(<5) ~ < 3 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateInteger2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(4),
                        AInteger.Create(4),
                        AInteger.Create(4)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(4),
                        AInteger.Create(4),
                        AInteger.Create(4)
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(0),
                        AInteger.Create(1),
                        AInteger.Create(2)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(4),
                        AInteger.Create(5)
                    )
                )
            );

            AType result = this.engine.Execute<AType>("4 ~ iota 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateInteger2Null()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                Utils.ANull(ATypes.AInteger),
                Utils.ANull(ATypes.AInteger)
            );

            AType result = this.engine.Execute<AType>("4 ~ ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateNull2Float()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                Utils.ANull(ATypes.AFloat),
                Utils.ANull(ATypes.AFloat)
            );

            AType result = this.engine.Execute<AType>("() ~ 5.7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateNull2SymbolConstant()
        {
            AType expected = AArray.Create(
                ATypes.ANull,
                Utils.ANull(),
                Utils.ANull()
            );

            AType result = this.engine.Execute<AType>("() ~ `a");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateNull2Integer()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                Utils.ANull(ATypes.AInteger),
                Utils.ANull(ATypes.AInteger)
            );

            AType result = this.engine.Execute<AType>("() ~ 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateFunctionScalar2SymbolConstant()
        {
            ScriptScope scope = this.engine.CreateScope();

            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.AFunc,
                f,
                ASymbol.Create("a")
            );

            AType result = this.engine.Execute<AType>("f ~ `a", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        public void LaminateBoxArray2SymbolConstant()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                Helpers.BuildStrand(
                    new AType[]
                    {
                        AInteger.Create(1),
                        AInteger.Create(4),
                        AInteger.Create(3)
                    }
                ),
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("b"),
                    ASymbol.Create("b"),
                    ASymbol.Create("b")
                )
            );

            AType result = this.engine.Execute<AType>("(3;4;1) ~ `b");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void LaminateRankError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) ~ iota 3 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void LaminateRankError2()
        {
            AType result = this.engine.Execute<AType>("() ~ iota 3 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LaminateLengthError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) ~ iota 3 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LaminateLengthError2()
        {
            AType result = this.engine.Execute<AType>("(1 rho 3) ~ 3 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LaminateLengthError3()
        {
            AType result = this.engine.Execute<AType>("2 4 ~ ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Laminate"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void LaminateTypeError1()
        {
            AType result = this.engine.Execute<AType>("'test' ~ 5.3 2.4");
        }
    }
}
