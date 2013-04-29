using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Reverse : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseFloat()
        {
            AType expected = AFloat.Create(8.3);
            AType result = this.engine.Execute<AType>("rot 8.3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseIntegerVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(7),
                AInteger.Create(56),
                AInteger.Create(4),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>("rot 3 2 4 56 7");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseIntegerVectorUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(7),
                AInteger.Create(56),
                AInteger.Create(4),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engineUni.Execute<AType>("S.| 3 2 4 56 7");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseCharacterConstant()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AChar.Create('t'),
                AChar.Create('s'),
                AChar.Create('e'),
                AChar.Create('t')
            );
            AType result = this.engine.Execute<AType>("rot 'test'");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseBox()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(AInteger.Create(23)),
                ABox.Create(
                    AArray.Create(
                        ATypes.ABox,
                        ABox.Create(AInteger.Create(2)),
                        ABox.Create(AInteger.Create(5))
                        )
                ),
                ABox.Create(AInteger.Create(3)),
                ABox.Create(AInteger.Create(4))

            );
            AType result = this.engine.Execute<AType>("rot (4;3;(2;5);23)");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseSymbolConstantVector()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("s3"),
                ASymbol.Create("s2"),
                ASymbol.Create("s1")
            );
            AType result = this.engine.Execute<AType>("rot `s1`s2`s3");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(2),
                    AInteger.Create(3)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1)
                )
            );
            AType result = this.engine.Execute<AType>("rot iota 2 2");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseSimpleMixedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.AFunc,
                f,
                ASymbol.Create("b"),
                ASymbol.Create("a")
            );

            AType result = this.engine.Execute<AType>("rot `a`b , f", scope);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Reverse"), TestMethod]
        public void ReverseNestedMixedArray()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("b"),
                ABox.Create(AInteger.Create(8)),
                ASymbol.Create("a"),
                ABox.Create(AInteger.Create(5))
            );

            AType result = this.engine.Execute<AType>("rot (<5), `a , (<8), `b");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
