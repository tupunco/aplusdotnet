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
    public class Unpack : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        public void UnpackSymbolConstant1()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AChar.Create('a')
            );

            AType result = this.engine.Execute<AType>("unpack `a");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        public void UnpackSymbolConstant2()
        {
            AType expected = Utils.ANull(ATypes.AChar);

            AType result = this.engine.Execute<AType>("unpack `");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        public void UnpackSymbolConstantVector1()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create('b'), AChar.Create(' ')),
                AArray.Create(ATypes.AChar, AChar.Create('e'), AChar.Create(' '), AChar.Create(' ')),
                AArray.Create(ATypes.AChar, AChar.Create('g'), AChar.Create('f'), AChar.Create('h'))
            );

            AType result = this.engine.Execute<AType>("unpack `ab `e `gfh");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        public void UnpackSymbolConstantVector1Uni()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create('b'), AChar.Create(' ')),
                AArray.Create(ATypes.AChar, AChar.Create('e'), AChar.Create(' '), AChar.Create(' ')),
                AArray.Create(ATypes.AChar, AChar.Create('g'), AChar.Create('f'), AChar.Create('h'))
            );

            AType result = this.engineUni.Execute<AType>("M.> `ab `e `gfh");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        public void UnpackSymbolConstantVector2()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create(' '), AChar.Create(' ')),
                AArray.Create(ATypes.AChar, AChar.Create(' '), AChar.Create(' '), AChar.Create(' ')),
                AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create('b'), AChar.Create('c'))
            );

            AType result = this.engine.Execute<AType>("unpack `a ` `abc");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        public void UnpackSymbolConstantMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(
                    ATypes.AChar,
                    AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create(' '), AChar.Create(' '), AChar.Create(' ')),
                    AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create('b'), AChar.Create('c'), AChar.Create('d')),
                    AArray.Create(ATypes.AChar, AChar.Create('t'), AChar.Create(' '), AChar.Create(' '), AChar.Create(' '))
                ),
                AArray.Create(
                    ATypes.AChar,
                    AArray.Create(ATypes.AChar, AChar.Create('d'), AChar.Create('f'), AChar.Create('g'), AChar.Create(' ')),
                    AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create(' '), AChar.Create(' '), AChar.Create(' ')),
                    AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create('b'), AChar.Create('c'), AChar.Create('d'))
                ),
                AArray.Create(
                    ATypes.AChar,
                    AArray.Create(ATypes.AChar, AChar.Create('t'), AChar.Create(' '), AChar.Create(' '), AChar.Create(' ')),
                    AArray.Create(ATypes.AChar, AChar.Create('d'), AChar.Create('f'), AChar.Create('g'), AChar.Create(' ')),
                    AArray.Create(ATypes.AChar, AChar.Create('a'), AChar.Create(' '), AChar.Create(' '), AChar.Create(' '))
                )
            );

            AType result = this.engine.Execute<AType>("unpack 3 3 rho `a `abcd `t `dfg");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        public void PackAndUnpackCharacterConstantVector()
        {
            AType expected = Helpers.BuildString("Hello World!");

            AType result = this.engine.Execute<AType>("unpack pack 'Hello World!'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void UnpackTypeError1()
        {
            AType result = this.engine.Execute<AType>("unpack 3");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void UnpackTypeError2()
        {
            AType result = this.engine.Execute<AType>("unpack `a`b , <{+}");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Unpack"), TestMethod]
        [ExpectedException(typeof(Error.MaxRank))]
        public void UnpackMaxRankError()
        {
            AType result = this.engine.Execute<AType>("unpack 1 1 1 1 1 1 1 1 1 rho `a");
        }
    }
}
