using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Hosting;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Find : AbstractTest
    {
        AType TestFunction = AFunc.Create(
            "a",
            (Func<Scope, AType, AType, AType>)((scope, x, y) =>
            {
                return AInteger.Create(x.asInteger + y.asInteger);
            }),
            2,
            "add 2 number"
        );

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindCharacterConstantMatrix2CharacterConstantMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(2),
                AInteger.Create(4)
            );
            AType result = this.engine.Execute<AType>("(4 3 rho 'fatbatcathat') iota (2 3 rho 'catpat')");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindCharacterConstantMatrixWithFrame2CharacterConstantMatrixWithFrame()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("(3 2 3 rho 'fatbatcathat') iota 2 3 rho 'cathat'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindStrand2NestedFunction()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("a", TestFunction);

            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("(3;2;a;5) iota <{a}", scriptscope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindIntegerList2Float()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("3 4 5 6 2 iota 5.00000000000004");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindIntegerList2FloatUni()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engineUni.Execute<AType>("3 4 5 6 2 I.# 5.00000000000004");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindFloatMatrix2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("(3 3 rho 18.9 9 7.5 13 16.4 11 2 9 13) iota 2 3 rho  3 2 4 2 9 13");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindInteger2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1), AInteger.Create(1))
            );

            AType result = this.engine.Execute<AType>("8 iota 3 3 rho 13 1 12 10 8 10 1 12 9");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindStrand2SymbolConstantList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(4)
            );

            AType result = this.engine.Execute<AType>("(3;4;5;) iota `s1`s2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindIntegerList2Null()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("12 2 3 iota ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindNull2Null()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("() iota ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindNull2Strand()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("() iota (3;4;2)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindFunctionArrayBox2FunctionScalar()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("(+;-;*;%) iota <{*}");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindMixedSimpleArray2FunctionScalar()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("((`a, <{+}) , `b) iota <{+}");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        public void FindMixedNestedArray2SymbolScalar()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("((5;+) , `a) iota `a");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void FindTypeError1()
        {
            AType result = this.engine.Execute<AType>("(4;3;5)iota 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FindLengthError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2) iota iota 3 3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FindLengthError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) iota 1 rho 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FindLengthError3()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) iota ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void FindRankError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 2 4) iota iota 8");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Find"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void FindRankError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) iota 4");
        }
    }
}
