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
    public class Pick : AbstractTest
    {
        #region Case 1

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickEmptyLeftArgument2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(1),
                AInteger.Create(3),
                AInteger.Create(4)
            );

            AType result = this.engine.Execute<AType>("(iota 0) pick 2 1 3 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickEmptyLeftArgument2Symbol()
        {
            AType expected = AArray.Create(ATypes.ASymbol, ASymbol.Create("test"));

            AType result = this.engine.Execute<AType>("() pick `test");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Case 2

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickInteger2SimpleInteger()
        {
            AType expected = AInteger.Create(6);

            AType result = this.engine.Execute<AType>("0 pick 6");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickInteger2SimpleCharacter()
        {
            AType expected = AChar.Create('a');

            AType result = this.engine.Execute<AType>("(1 1 rho 0) pick 'a'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Case 3

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2SlotFiller1()
        {
            AType expected = AInteger.Create(7);

            AType result = this.engine.Execute<AType>("`a pick (`x`a;(2;7))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2SlotFiller2()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("`test pick (`test;< iota 3)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2SlotFiller3()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);

            AType expected = ABox.Create(scope.GetVariable<AType>(".a"),ATypes.AFunc);

            AType result = this.engine.Execute<AType>("`a pick (`a; <{a})",scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2SlotFiller4()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);
            this.engine.Execute<AType>("b{a}: a*a", scope);
            AType expected = this.engine.Execute<AType>("f := <{+}", scope);

            AType result = this.engine.Execute<AType>("`c pick (`a`b`c;(a;b;f))", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstantVector2SlotFiller1()
        {
            AType expected = AInteger.Create(4);

            AType result = this.engine.Execute<AType>("`a`b pick (`a ;<(`b;<4))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstantVector2SlotFiller2()
        {
            AType expected = AInteger.Create(20);

            AType result = this.engine.Execute<AType>("`was pick (`this`was`that;(10;20;30))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller1()
        {
            AType expected = AInteger.Create(20);

            AType result = this.engine.Execute<AType>("`was pick (`this`was`that`was;(10;20;30;40))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller2()
        {
            AType expected = AInteger.Create(40);

            AType result = this.engine.Execute<AType>("`d pick (2 2 rho `a`b`c`d;(10;20;30;40))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller3()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("`b pick (`a`b;1 2 rho (4;2))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller4()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("`c pick (2 2 rho `a`b`c`d;2 2 rho (6;7;2;9))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller5()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);
            this.engine.Execute<AType>("b{a}: a*a", scope);

            AType expected = ABox.Create(scope.GetVariable<AType>(".b"));

            AType result = this.engine.Execute<AType>("`b pick (`a`c`b;(a;+;b))", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller6()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("`a pick ((`a , < 5) , `b;(3;5;7))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller7()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("`a pick ((`a , < 5) , `b;(3;5;7))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickSymbolConstant2ExtendedSlotFiller8()
        {
            AType expected = AInteger.Create(9);

            AType result = this.engine.Execute<AType>("`c pick (`a`b`c;((<4) , `g) , <9)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Case 4

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickInteger2SimpleIntegerVector()
        {
            AType expected = AInteger.Create(4);

            AType result = this.engine.Execute<AType>("1 pick 23 4 45 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickOneElementVector2SimpleSymbolConstantVector()
        {
            AType expected = ASymbol.Create("s1");

            AType result = this.engine.Execute<AType>("(1 rho 0) pick `s1`s2`s3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickOneElementMatrix2SimpleSymbolConstantVector()
        {
            AType expected = AChar.Create('t');

            AType result = this.engine.Execute<AType>("(1 1 rho 3) pick 'test'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Case 5

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickIntegerNestedElement()
        {
            AType expected = AInteger.Create(5);

            AType result = this.engine.Execute<AType>("0 pick < 5");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickIntegerNestedVector1()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("0 pick (3;`s)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickIntegerNestedVector2()
        {
            AType expected = Helpers.BuildStrand(
            new AType[]{
                AArray.Create(ATypes.AInteger, AInteger.Create(10), AInteger.Create(20)),
                AArray.Create(
                    ATypes.AChar,
                    AArray.Create(ATypes.AChar, AChar.Create('c'), AChar.Create('d')),
                    AArray.Create(ATypes.AChar, AChar.Create('e'), AChar.Create('f')))
            });

            AType result = this.engine.Execute<AType>("1 pick ('ab';(2 2 rho 'cdef'; 10 20))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickOneElementMatrix2NestedVector()
        {
            AType expected = AInteger.Create(5);

            AType result = this.engine.Execute<AType>("(1 1 rho 2) pick (2;4;5;6;3)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNull2NestedVector()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    Utils.ANull(),
                    AInteger.Create(4),
                    AInteger.Create(2)
                }
            );

            AType result = this.engine.Execute<AType>("() pick (2;4;)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickPathVector2NestedVector1()
        {
            AType expected = Helpers.BuildString("test");

            AType result = this.engine.Execute<AType>("1 0 pick (3;('test';`s))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickPathVector2NestedVector2()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(ATypes.AChar, AChar.Create('c'), AChar.Create('d')),
                AArray.Create(ATypes.AChar, AChar.Create('e'), AChar.Create('f'))
            );

            AType result = this.engine.Execute<AType>("1 0 pick ('ab';(2 2 rho 'cdef'; 10 20))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void MultiPickPathNumber2NestedVector()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                AArray.Create(ATypes.AChar, AChar.Create('c'), AChar.Create('d')),
                AArray.Create(ATypes.AChar, AChar.Create('e'), AChar.Create('f'))
            );

            AType result = this.engine.Execute<AType>("0 pick 1 pick ('ab';(2 2 rho 'cdef'; 10 20))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickPathNumber2NestedNull()
        {
            AType expected = AArray.Create(ATypes.ANull);

            AType result = this.engine.Execute<AType>("0 pick < ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void MultiPickPathNumber2MixedNestedVector1()
        {
            AType expected = ASymbol.Create("a");

            AType result = this.engine.Execute<AType>("1 pick ((<4) , `a) , <9");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void MultiPickPathNumber2MixedNestedVector2()
        {
            AType expected = AInteger.Create(4);

            AType result = this.engine.Execute<AType>("1 pick (3;4) , `a`b");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Case 6

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedInteger2NestetVector()
        {
            AType expected = AInteger.Create(6);

            AType result = this.engine.Execute<AType>("(<2) pick (1;3;6;4)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedOneElementIntegerVector2NestetVector()
        {
            AType expected = AInteger.Create(4);

            AType result = this.engine.Execute<AType>("(< 1 rho 3) pick (1;3;6;4)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedOneElementIntegerVector2NestedElement()
        {
            AType expected = AInteger.Create(6);

            AType result = this.engine.Execute<AType>("(< 1 rho 0) pick < 6");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedNull2ArrayNestedElement()
        {
            AType expected = AInteger.Create(4);

            AType result = this.engine.Execute<AType>("(< ()) pick 1 1 1 1 rho < 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedSymbolConstant2NestetVector()
        {
            AType expected = AInteger.Create(5);

            AType result = this.engine.Execute<AType>("(< `a) pick (`a;< 5)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedSymbolConstant2ExtendedSlotFiller()
        {
            AType expected = AInteger.Create(10);

            AType result = this.engine.Execute<AType>("(< `a) pick (2 2 rho `a`b`c`d;(10;20;30;40))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedElement2NestedCharacterMatrix()
        {
            AType expected = AChar.Create('b');

            AType result = this.engine.Execute<AType>("(< 0 1) pick 2 2 rho ('a';'b';'c';'d')");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNestedNull2NestedElement()
        {
            AType expected = AInteger.Create(7);

            AType result = this.engine.Execute<AType>("(< ()) pick < 7");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickStrand2NestedStrand()
        {
            AType expected = AChar.Create('d');

            AType result = this.engine.Execute<AType>("(1;0;0 1) pick ('ab';(2 2 rho('c';'d';'e';'f');10 20))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickStrandWithNull2NestedElement()
        {
            AType expected = AInteger.Create(7);

            AType result = this.engine.Execute<AType>("(1;) pick (3;< 7)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickNumber2NestedNull()
        {
            AType expected = AArray.Create(ATypes.ANull);

            AType result = this.engine.Execute<AType>("(< 0) pick < ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        //A+ DLR Extension!
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickMixedNestedArray2NestedArray1()
        {
            AType expected = AInteger.Create(5);

            AType result = this.engine.Execute<AType>("((<1) , `b) pick (1;(`a`b;(2;5)))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        //A+ DLR Extension!
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        public void PickMixedNestedArray2NestedArray2()
        {
            AType expected = AInteger.Create(5);

            AType result = this.engine.Execute<AType>("(`b , < 1) pick (`a`b;(2;(3;5)))");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion


        #region Error

        #region Case 2

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError8()
        {
            AType result = this.engine.Execute<AType>("2 pick `s1");
        }

        #endregion

        #region Case 3

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError1()
        {
            AType result = this.engine.Execute<AType>("`a pick (`test;< iota 3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError2()
        {
            AType result = this.engine.Execute<AType>("`y pick (`x`a;(2;7))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError3()
        {
            AType result = this.engine.Execute<AType>("`a`r pick (`a ;<(`b;<4))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError1()
        {
            AType result = this.engine.Execute<AType>("`a`b pick (`a ; < 4 2 3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError9()
        {
            AType result = this.engine.Execute<AType>("(2 1 rho `a`b) pick (`a ;<(`b;<4))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError10()
        {
            AType result = this.engine.Execute<AType>("(1 1 1 1 rho `a) pick (`a ;<(`b;<4))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError10()
        {
            AType result = this.engine.Execute<AType>("`a pick ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError11()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);
            this.engine.Execute<AType>("b{a}: a*a", scope);

            this.engine.Execute<AType>("`c pick (`a`c`b;(a;+;b))", scope);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError12()
        {
            this.engine.Execute<AType>("`a pick (`a;<{+})");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError14()
        {
            this.engine.Execute<AType>("`a pick `a , < 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError15()
        {
            this.engine.Execute<AType>("`b pick ((`a,<{+}),`b;(3;5;7))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError16()
        {
            this.engine.Execute<AType>("`b pick (`a`b`c;((<4) , `g) , <9)");
        }

        #endregion

        #region Case 4

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError6()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) pick 2 34 3 3 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError7()
        {
            AType result = this.engine.Execute<AType>(" 0 1 pick 3 2 23 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError8()
        {
            AType result = this.engine.Execute<AType>(" 0 1 pick iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError7()
        {
            AType result = this.engine.Execute<AType>("2 pick iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError7()
        {
            AType result = this.engine.Execute<AType>("3 pick 2 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError9()
        {
            AType result = this.engine.Execute<AType>("2 pick ()");
        }

        #endregion

        #region Case 5

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void PickTypeError1()
        {
            AType result = this.engine.Execute<AType>("'a' pick (1;2 3;4)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError2()
        {
            AType result = this.engine.Execute<AType>("1 0 pick (1;2 3;4)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError4()
        {
            AType result = this.engine.Execute<AType>("2 pick (1;3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError5()
        {
            AType result = this.engine.Execute<AType>("1 3 pick (1;(3;7))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError1()
        {
            AType result = this.engine.Execute<AType>("1 0 pick (2;2 2 rho (3;4;7;1))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError10()
        {
            AType result = this.engine.Execute<AType>("2 pick < `s1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError11()
        {
            AType result = this.engine.Execute<AType>("(1 2 rho 1 0) pick ('ab';(2 2 rho 'cdef'; 10 20))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError13()
        {
            AType result = this.engine.Execute<AType>("1 pick < ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError17()
        {
            AType result = this.engine.Execute<AType>("1 0 pick (`a;`b`c;`d)");
        }

        #endregion

        #region Case 6

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError3()
        {
            AType result = this.engine.Execute<AType>("(< 'a') pick (2;3;1)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError4()
        {
            AType result = this.engine.Execute<AType>("(< `s) pick (2;3;1)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError2()
        {
            AType result = this.engine.Execute<AType>("(< 1 1 rho 1) pick (3;4)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError3()
        {
            AType result = this.engine.Execute<AType>("(< 0 1) pick 2 3 2 rho ('a';'b';'c';'d')");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError5()
        {
            AType result = this.engine.Execute<AType>("(< 1 1) pick iota 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError4()
        {
            AType result = this.engine.Execute<AType>("(< 0 1) pick < 7");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError6()
        {
            AType result = this.engine.Execute<AType>("(< 0 2) pick 2 2 rho ('a';'b';'c';'d')");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError6()
        {
            AType result = this.engine.Execute<AType>("(< () ) pick (3;4;6)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError5()
        {
            AType result = this.engine.Execute<AType>("(1;0;0;1) pick ('ab';(2 2 rho('c';'d';'e';'f');10 20))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError9()
        {
            AType result = this.engine.Execute<AType>("(0;) pick (2 2 rho < 'a' ; 4)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError12()
        {
            AType result = this.engine.Execute<AType>("(1 1 1 1 rho < 0 1) pick 2 2 rho ('a';'b';'c';'d')");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError13()
        {
            AType result = this.engine.Execute<AType>("(3 1 rho (1;0;0 1)) pick ('ab';(2 2 rho('c';'d';'e';'f');10 20))");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError11()
        {
            AType result = this.engine.Execute<AType>("(< 0) pick ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PickRankError14()
        {
            AType result = this.engine.Execute<AType>("(1;0) pick (2;<7)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError12()
        {
            AType result = this.engine.Execute<AType>("(< 1) pick < ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError14()
        {
            AType result = this.engine.Execute<AType>("(< 0) pick ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void PickIndexError15()
        {
            AType result = this.engine.Execute<AType>("(0;0) pick (();3)");
        }

        //A+ interpreter: segmentation fault.
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Pick"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PickDomainError13()
        {
            AType result = this.engine.Execute<AType>("(<{+}) pick (3;4)");
        }

        #endregion

        #endregion
    }
}
