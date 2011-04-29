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
    public class Expand : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandIntegerList2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(0),
                AInteger.Create(8)
            );
            AType result = this.engine.Execute<AType>("1 0 1 \\ 4 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandIntegerList2SymbolConstantList()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create(""),
                ASymbol.Create("test"),
                ASymbol.Create(""),
                ASymbol.Create("test")
            );
            AType result = this.engine.Execute<AType>("0 1 0 1 \\ `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandIntegerList2OneElementMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(5)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(5)
                )
            );
            AType result = this.engine.Execute<AType>("1 0 1 \\ 1 1 rho 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandIntegerList2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(0)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(2),
                    AInteger.Create(3)
                )
            );
            AType result = this.engine.Execute<AType>("1 0 1 \\ iota 2 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandNull2SymbolConstant()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("")
            );
            AType result = this.engine.Execute<AType>("0 \\ `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandOne2CharacterConstant()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AChar.Create('a')
            );
            AType result = this.engine.Execute<AType>("1 \\ 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandNull2Float()
        {
            AType expected = AArray.Create(
                ATypes.AFloat
            );
            AType result = this.engine.Execute<AType>("() \\ 3.4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        public void ExpandIntegerVector2NestedMixedArray1()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);


            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("a"),
                    ABox.Create(AInteger.Create(4))
                ),
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create(""),
                    ASymbol.Create("")
                ),
                AArray.Create(
                    ATypes.AFunc,
                    f,
                    ASymbol.Create("b")
                ),
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create(""),
                    ASymbol.Create("")
                )
            );

            AType result = this.engine.Execute<AType>("1 0 1 0 \\ 2 2 rho `a , (<4) , f , `b", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Rotate"), TestMethod]
        public void ExpandIntegerList2NestedMixedMatrix2()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(
                    ATypes.ASymbol,
                    ASymbol.Create("a"),
                    ASymbol.Create(""),
                    ABox.Create(AInteger.Create(4))
                ),
                AArray.Create(
                    ATypes.AFunc,
                    f,
                    ABox.Create(Utils.ANull()),
                    ASymbol.Create("b")
                ),
                AArray.Create(
                    ATypes.ABox,
                    ABox.Create(AInteger.Create(2)),
                    ABox.Create(Utils.ANull()),
                    f
                )
            );

            AType result = this.engine.Execute<AType>("1 0 1 \\@ 1 1 rtack 3 2 rho `a , (<4) , f , `b , (<2) , f", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ExpandDomainError()
        {
            AType result = this.engine.Execute<AType>("1 2 0 \\ 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ExpandTypeError1()
        {
            AType result = this.engine.Execute<AType>("3.0000000000003 4.3 \\ 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ExpandTypeError2()
        {
            AType result = this.engine.Execute<AType>("(< 4) \\ 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ExpandRankError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) \\ 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ExpandRankError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) \\ ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ExpandLengthError1()
        {
            AType result = this.engine.Execute<AType>("1 0 1 1 \\ 2 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ExpandLengthError2()
        {
            AType result = this.engine.Execute<AType>("1 0 1 \\ ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ExpandLengthError3()
        {
            AType result = this.engine.Execute<AType>("1 \\ ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Expand"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ExpandLengthError4()
        {
            AType result = this.engine.Execute<AType>("() \\ (3;7)");
        }
    }
}
