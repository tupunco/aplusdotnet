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
    public class Ravel : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ravel"), TestMethod]
        public void RavelInteger()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(4));
            AType result = this.engine.Execute<AType>(", 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ravel"), TestMethod]
        public void RavelIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>(", iota 2 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ravel"), TestMethod]
        public void RavelCharacterMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AChar.Create('c'),
                AChar.Create('a'),
                AChar.Create('t'),
                AChar.Create('h'),
                AChar.Create('a'),
                AChar.Create('t'),
                AChar.Create('b'),
                AChar.Create('a'),
                AChar.Create('t')
            );
            AType result = this.engine.Execute<AType>(", 3 3 rho 'cathatbat'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ravel"), TestMethod]
        public void RavelIntegerVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>(", rho iota 1");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ravel"), TestMethod]
        public void RavelBox()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AInteger.Create(8),
                    AInteger.Create(6),
                    AInteger.Create(5),
                    AInteger.Create(3)
                }
            );
                
            AType result = this.engine.Execute<AType>(", 2 2 rho (3;5;6;8)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Ravel"), TestMethod]
        public void RavelNull()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>(", ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
