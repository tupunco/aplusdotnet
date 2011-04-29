using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Cast : AbstractTest
    {
        #region Integer

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastInteger2FloatList1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(2),
                AInteger.Create(2),
                AInteger.Create(2),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("`int ? 1.055 1.155 1.255 1.355 1.455 1.555 1.655 1.755 1.855 1.955");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastInteger2FloatList2()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(10),
                AInteger.Create(10),
                AInteger.Create(11),
                AInteger.Create(11),
                AInteger.Create(-9),
                AInteger.Create(-9),
                AInteger.Create(-10),
                AInteger.Create(-10),
                AInteger.Create(10)
            );

            AType result = this.engine.Execute<AType>("`int ? 10 10.2 10.5 10.98 -9 -9.2 -9.5 -9.98 , 10+1e-13");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastInteger2FloatMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(434)),
                AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(87))
            );

            AType result = this.engine.Execute<AType>("`int ? 3 2 rho 2.53 434.245 4 .5 4.87 86.9");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastInteger2CharacterMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(97), AInteger.Create(32), AInteger.Create(98)),
                AArray.Create(ATypes.AInteger, AInteger.Create(99), AInteger.Create(32), AInteger.Create(100))
            );

            AType result = this.engine.Execute<AType>("`int ? 2 3 rho 'a bc d'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastInteger2Null()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("`int ? ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastInteger2SymbolMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(97), AInteger.Create(32), AInteger.Create(32), AInteger.Create(32)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(97), AInteger.Create(98), AInteger.Create(99), AInteger.Create(100)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(97), AInteger.Create(98), AInteger.Create(99), AInteger.Create(32))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(97), AInteger.Create(100), AInteger.Create(32), AInteger.Create(32)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(101), AInteger.Create(32), AInteger.Create(32), AInteger.Create(32)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(97), AInteger.Create(32), AInteger.Create(32), AInteger.Create(32))
                )
            );

            AType result = this.engine.Execute<AType>("`int ? 2 3 rho `a `abcd `abc `ad `e");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError1()
        {
            AType result = this.engine.Execute<AType>("`int ? 2147483648");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError2()
        {
            AType result = this.engine.Execute<AType>("`int ? < 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void CastTypeError1()
        {
            AType result = this.engine.Execute<AType>("`int ? `a , <{+}");
        }

        #endregion

        #region Float

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastFloat2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(1), AFloat.Create(2)),
                AArray.Create(ATypes.AFloat, AFloat.Create(3), AFloat.Create(4), AFloat.Create(5)),
                AArray.Create(ATypes.AFloat, AFloat.Create(6), AFloat.Create(7), AFloat.Create(8))
            );

            AType result = this.engine.Execute<AType>("`float ? iota 3 3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastFloat2CharacterMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(116), AFloat.Create(101)),
                AArray.Create(ATypes.AFloat, AFloat.Create(115), AFloat.Create(116))
            );

            AType result = this.engine.Execute<AType>("`float ?  2 2 rho 'test'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastFloat2SymbolMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(
                    ATypes.AFloat,
                    AArray.Create(ATypes.AFloat, AFloat.Create(119), AFloat.Create(115), AFloat.Create(100)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(121), AFloat.Create(32), AFloat.Create(32))
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AArray.Create(ATypes.AFloat, AFloat.Create(117), AFloat.Create(32), AFloat.Create(32)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(32), AFloat.Create(32), AFloat.Create(32))
                )
            );

            AType result = this.engine.Execute<AType>("`float ? 2 2 rho `wsd `y `u ` `hj");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastFloat2Null()
        {
            AType expected = Utils.ANull(ATypes.AFloat);

            AType result = this.engine.Execute<AType>("`float ? ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError4()
        {
            AType result = this.engine.Execute<AType>("`float ? < 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError6()
        {
            AType result = this.engine.Execute<AType>("`float ? <{+}");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void CastTypeError2()
        {
            AType result = this.engine.Execute<AType>("`float ? `a , < 5");
        }

        #endregion

        #region Symbol

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastSymbol2Integer()
        {
            AType expected = ASymbol.Create("a");

            AType result = this.engine.Execute<AType>("`sym ? 97");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastSymbol2IntegerList()
        {
            AType expected = ASymbol.Create("Hello World");

            AType result = this.engine.Execute<AType>("`sym ? 72 101 108 108 111 32 87 111 114 108 100");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastSymbol2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("Hello"),
                ASymbol.Create("World")
            );

            AType result = this.engine.Execute<AType>("`sym ? 2 5 rho  72 101 108 108 111 87 111 114 108 100");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastSymbol2Float()
        {
            AType expected = ASymbol.Create("Test");

            AType result = this.engine.Execute<AType>("`sym ? 1107.7 100.5 627.2 116.2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastSymbol2Null()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("`sym ? ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastSymbol2CharacterMatrix()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("a"),
                ASymbol.Create("abc"),
                ASymbol.Create(""),
                ASymbol.Create("ra")
            );

            AType result = this.engine.Execute<AType>("`sym ? 4 3 rho 'a  abc   r'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastSymbol2SimpleMixedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("a"),
                f
            );

            AType result = this.engine.Execute<AType>("`sym ? `a , f", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError7()
        {
            AType result = this.engine.Execute<AType>("`sym ? <{+}");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError8()
        {
            AType result = this.engine.Execute<AType>("`sym ? (< 4) , `a");
        }

        #endregion

        #region Char

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastChar2Null()
        {
            AType expected = Utils.ANull(ATypes.AChar);

            AType result = this.engine.Execute<AType>("`char ? ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastChar2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("Hello"),
                Helpers.BuildString("World")
            );

            AType result = this.engine.Execute<AType>("`char ? 2 5 rho  72 101 108 108 111 87 111 114 108 100");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastChar2FloatVector()
        {
            AType expected = Helpers.BuildString("Test");

            AType result = this.engine.Execute<AType>("`char ? 1107.7 100.5 627.2 116.2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastChar2SymbolVector()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("a  "),
                Helpers.BuildString("   "),
                Helpers.BuildString("abc")
            );

            AType result = this.engine.Execute<AType>("`char ? `a ` `abc");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        public void CastChar2SymbolMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(
                    ATypes.AChar,
                    Helpers.BuildString("a   "),
                    Helpers.BuildString("abcd"),
                    Helpers.BuildString("t   ")
                ),
                AArray.Create(
                    ATypes.AChar,
                    Helpers.BuildString("dfg "),
                    Helpers.BuildString("a   "),
                    Helpers.BuildString("abcd")
                ),
                AArray.Create(
                    ATypes.AChar,
                    Helpers.BuildString("t   "),
                    Helpers.BuildString("dfg "),
                    Helpers.BuildString("a   ")
                )
            );

            AType result = this.engine.Execute<AType>("`char ? 3 3 rho `a `abcd `t `dfg");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError3()
        {
            AType result = this.engine.Execute<AType>("`int`float ? 13");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void CastDomainError5()
        {
            AType result = this.engine.Execute<AType>("(<3) ? < 1");
        }
    }
}
