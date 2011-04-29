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
    public class Take : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeNegativeCount()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>("-2 take 1 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeNegativeCountWithFill()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2)
            );
            AType result = this.engine.Execute<AType>("-3 take 1 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeMatrixNegativeCountWithFill()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
            );
            AType result = this.engine.Execute<AType>("-3 take iota 2 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeZeroCount()
        {
            AType expected = AArray.Create(
                ATypes.AInteger
            );
            AType result = this.engine.Execute<AType>("0 take 1 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeZeroCountFromArray()
        {
            AType expected = AArray.Create(ATypes.AInteger);
            expected.Shape = new List<int>() { 0, 3, 4 };
            expected.Rank = expected.Shape.Count;

            AType result = this.engine.Execute<AType>("0 take iota 2 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(4)
            );
            AType result = this.engine.Execute<AType>("2 take 3 4 1 6 7 9");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeRestrictedWholeNumber2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(4),
                AInteger.Create(1),
                AInteger.Create(6)
            );
            AType result = this.engine.Execute<AType>("4.00000000000001 take 3 4 1 6 7 9");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        [ExpectedException(typeof(Error.Nonce))]
        public void TakeNonceError1()
        {
            AType result = this.engine.Execute<AType>("1 3 take 12 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        [ExpectedException(typeof(Error.Nonce))]
        public void TakeNonceError2()
        {
            AType result = this.engine.Execute<AType>("() take 12 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TakeTypeError1()
        {
            AType result = this.engine.Execute<AType>("`test take 3 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TakeTypeError2()
        {
            AType result = this.engine.Execute<AType>("'a' take 67 7");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1)
                )
            );
            AType result = this.engine.Execute<AType>("1 take iota 2 2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2FloatList()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(3.2),
                AFloat.Create(6.4),
                AFloat.Create(2.7)
            );
            AType result = this.engine.Execute<AType>("3 take 3.2 6.4 2.7");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2Integer()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(5)
            );
            AType result = this.engine.Execute<AType>("1 take 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2IntegerListWithFillElement()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(4),
                AInteger.Create(5),
                AInteger.Create(0),
                AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("5 take 2 4 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2SymbolConstantWithFillElement()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("test"),
                ASymbol.Create("")
            );
            AType result = this.engine.Execute<AType>("2 take `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2CharacterConstant()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AChar.Create('a'),
                AChar.Create('b'),
                AChar.Create(' '),
                AChar.Create(' ')
            );
            AType result = this.engine.Execute<AType>("4 take 'ab'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2Null()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(Utils.ANull()),
                ABox.Create(Utils.ANull())
            );

            AType result = this.engine.Execute<AType>("2 take ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            //Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2Box()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(AInteger.Create(1)),
                ABox.Create(AInteger.Create(5))
            );

            AType result = this.engine.Execute<AType>("2 take (1;5;7)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeWithSingleElementArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1)
                )
            );

            AType x = AArray.Create(ATypes.AInteger, 
                AArray.Create(ATypes.AInteger, 
                    AArray.Create(ATypes.AInteger,AInteger.Create(1))
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".x", x);

            AType result = this.engine.Execute<AType>("x take iota 2 2", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Take"), TestMethod]
        public void TakeInteger2SimpleMixedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.AFunc,
                f
            );

            AType result = this.engine.Execute<AType>("-1 take `a`b , f", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }
    }
}
