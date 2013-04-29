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
    public class Pack : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterConstant1()
        {
            AType expected = ASymbol.Create("a");

            AType result = this.engine.Execute<AType>("pack 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterConstant1Uni()
        {
            AType expected = ASymbol.Create("a");

            AType result = this.engineUni.Execute<AType>("M.< 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterConstant2()
        {
            AType expected = ASymbol.Create("");

            AType result = this.engine.Execute<AType>("pack ''");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterConstantVector1()
        {
            AType expected = ASymbol.Create("      a");

            AType result = this.engine.Execute<AType>("pack '      a      '");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterConstantVector1Uni()
        {
            AType expected = ASymbol.Create("      a");

            AType result = this.engineUni.Execute<AType>("M.< '      a      '");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterConstantVector2()
        {
            AType expected = ASymbol.Create("");

            AType result = this.engine.Execute<AType>("pack '         '");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterConstantVector3()
        {
            AType expected = ASymbol.Create("abc  d");

            AType result = this.engine.Execute<AType>("pack 'abc  d'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterMatrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                AArray.Create(ATypes.ASymbol, ASymbol.Create("abc"), ASymbol.Create("  d"), ASymbol.Create("efa")),
                AArray.Create(ATypes.ASymbol, ASymbol.Create("bc"), ASymbol.Create(" de"), ASymbol.Create("fab"))
            );
            AType result = this.engine.Execute<AType>("pack 2 3 3 rho 'abc  def'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        public void PackCharacterMatrix()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("a"),
                ASymbol.Create("abc"),
                ASymbol.Create(""),
                ASymbol.Create("ra")
            );

            AType result = this.engine.Execute<AType>("pack 4 3 rho 'a  abc   r'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void PackTypeError1()
        {
            AType result = this.engine.Execute<AType>("pack 3");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void PackTypeError2()
        {
            AType result = this.engine.Execute<AType>("pack ()");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Pack"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void PackTypeError3()
        {
            AType result = this.engine.Execute<AType>("pack <{+}");
        }
    }
}
