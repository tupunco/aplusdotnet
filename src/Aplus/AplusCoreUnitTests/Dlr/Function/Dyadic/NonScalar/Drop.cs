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
    public class Drop : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(6),
                AInteger.Create(7)
            );
            AType result = this.engine.Execute<AType>("2 drop 1 3 6 7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2IntegerListUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(6),
                AInteger.Create(7)
            );
            AType result = this.engineUni.Execute<AType>("2 S.- 1 3 6 7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropRestrictedWholeNumber2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(10),
                    AInteger.Create(11)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(12),
                    AInteger.Create(13),
                    AInteger.Create(14)
                )
            );
            AType result = this.engine.Execute<AType>("3.000000000000003 drop iota 5 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2FloatList()
        {
            AType expected = AArray.Create(
                ATypes.AFloat
            );
            AType result = this.engine.Execute<AType>("6 drop 2.5 4 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropNull2CharacterConstant1()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AChar.Create('a'),
                AChar.Create('b')
            );
            AType result = this.engine.Execute<AType>("0 drop 'ab'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropNegativeInteger2CharacterConstant()
        {
            AType expected = Helpers.BuildString("15 Jan");

            AType result = this.engine.Execute<AType>("-4 drop '15 January'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropNull2CharacterConstant2()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AChar.Create('a')
            );
            AType result = this.engine.Execute<AType>("(iota 1) drop 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2Box()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );
            AType result = this.engine.Execute<AType>("1 drop < 7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2BoxList()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );
            AType result = this.engine.Execute<AType>("3 drop (2;7)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropRestrictedWholweNumber2BoxList()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(AInteger.Create(1))
            );
            AType result = this.engine.Execute<AType>("2.0000000000000000009 drop (3;5;1)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2Null()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );
            AType result = this.engine.Execute<AType>("6 drop ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2SymbolList()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );
            AType result = this.engine.Execute<AType>("3 drop `s1`s2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2FunctionList()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );
            AType result = this.engine.Execute<AType>(" 3 drop (+;-)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        public void DropInteger2MixedSimpleArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.AFunc,
                f
            );
            AType result = this.engine.Execute<AType>("2 drop `a`b , f", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        [ExpectedException(typeof(Error.Nonce))]
        public void DropDomainError1()
        {
            AType result = this.engine.Execute<AType>("() drop 6 72");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Drop"), TestMethod]
        [ExpectedException(typeof(Error.Nonce))]
        public void DropDomainError2()
        {
            AType result = this.engine.Execute<AType>("1 3 drop 6 72");
        }
    }
}
