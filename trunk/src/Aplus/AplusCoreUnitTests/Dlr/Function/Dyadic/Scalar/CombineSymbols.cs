using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class CombineSymbols : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("CombineSymbols"), TestMethod]
        public void CombineSymbolsNull()
        {
            AType result = this.engine.Execute<AType>("`c dot ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Incorrect type returned");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("CombineSymbols"), TestMethod]
        public void CombineSymbolsNull2Null()
        {
            AType result = this.engine.Execute<AType>("() dot ()");

            Assert.AreEqual<ATypes>(ATypes.ANull, result.Type, "Incorrect type returned");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("CombineSymbols"), TestMethod]
        public void CombineSymbolsVector2Vector1()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("c.x"),
                ASymbol.Create("d.y"),
                ASymbol.Create(".z")
            );
            AType result = this.engine.Execute<AType>("`c dot `x `d.y `.z");
            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.ASymbol);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("CombineSymbols"), TestMethod]
        public void CombineSymbolsVector2Vector2()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("b.c.x"),
                ASymbol.Create("a.y")
            );
            AType result = this.engine.Execute<AType>("`b.c `a dot `x `y");
            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.ASymbol);
        }
    }
}
