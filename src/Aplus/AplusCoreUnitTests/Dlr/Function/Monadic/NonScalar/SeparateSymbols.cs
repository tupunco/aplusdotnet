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
    public class SeparateSymbols : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("SeparateSymbols"), TestMethod]
        public void SeparateSymbolsSymbolConstant()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create(""),
                ASymbol.Create("a")
            );

            AType result = this.engine.Execute<AType>("dot `.a");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("SeparateSymbols"), TestMethod]
        public void SeparateSymbolsSymbolConstantList()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(ATypes.ASymbol, ASymbol.Create(""), ASymbol.Create("x")),
                AArray.Create(ATypes.ASymbol, ASymbol.Create(""), ASymbol.Create("y")),
                AArray.Create(ATypes.ASymbol, ASymbol.Create("c"), ASymbol.Create("z")),
                AArray.Create(ATypes.ASymbol, ASymbol.Create("a.b"), ASymbol.Create("d"))
            );

            AType result = this.engine.Execute<AType>("dot `x `.y `c.z `a.b.d");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("SeparateSymbols"), TestMethod]
        public void SeparateSymbolsSymbolConstantMatrix()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(
                    ATypes.ASymbol,
                    AArray.Create(ATypes.ASymbol, ASymbol.Create(""), ASymbol.Create("")),
                    AArray.Create(ATypes.ASymbol, ASymbol.Create(""), ASymbol.Create("y"))
                ),
                AArray.Create(
                    ATypes.ASymbol,
                    AArray.Create(ATypes.ASymbol, ASymbol.Create("y.w.q"), ASymbol.Create("e")),
                    AArray.Create(ATypes.ASymbol, ASymbol.Create(""), ASymbol.Create("o"))
                )
            );

            AType result = this.engine.Execute<AType>("dot 2 2 rho ` `y `y.w.q.e `.o");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("SeparateSymbols"), TestMethod]
        public void SeparateSymbolsNull()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("dot ()");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("SeparateSymbols"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void SeparateSymbolsTypeError1()
        {
            AType result = this.engine.Execute<AType>("dot 8");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("SeparateSymbols"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void SeparateSymbolsTypeError2()
        {
            AType result = this.engine.Execute<AType>("dot `a`b , <{+}");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("SeparateSymbols"), TestMethod]
        [ExpectedException(typeof(Error.MaxRank))]
        public void SeparateSymbolsMaxRankError()
        {
            AType result = this.engine.Execute<AType>("dot 1 1 1 1 1 1 1 1 1 rho `a");
        }
    }
}
