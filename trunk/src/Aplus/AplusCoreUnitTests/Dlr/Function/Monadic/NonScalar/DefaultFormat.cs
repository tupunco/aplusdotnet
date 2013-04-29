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
    public class DefaultkFormat : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatInteger()
        {
            AType expected = Helpers.BuildString(" 435");

            AType result = this.engine.Execute<AType>("form 435");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatIntegerUni()
        {
            AType expected = Helpers.BuildString(" 435");

            AType result = this.engineUni.Execute<AType>("E.% 435");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatIntegerList()
        {
            AType expected = Helpers.BuildString(" 42 23 65 353");

            AType result = this.engine.Execute<AType>("form 42 23 65 353");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatIntegerListUni()
        {
            AType expected = Helpers.BuildString(" 42 23 65 353");

            AType result = this.engineUni.Execute<AType>("E.% 42 23 65 353");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatIntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("   3   4  54"),
                Helpers.BuildString(" 332   2   4"),
                Helpers.BuildString("   3   4  54")
            );

            AType result = this.engine.Execute<AType>("form 3 3 rho 3 4 54 332 2 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatIntegerMatrixUni()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("   3   4  54"),
                Helpers.BuildString(" 332   2   4"),
                Helpers.BuildString("   3   4  54")
            );

            AType result = this.engineUni.Execute<AType>("E.% 3 3 S.? 3 4 54 332 2 4");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFloat()
        {
            AType expected = Helpers.BuildString(" 4e+01");

            this.engine.Execute<AType>("$pp 1");
            AType result = this.engine.Execute<AType>("form 43.434");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFloatList1()
        {
            AType expected = Helpers.BuildString(" 4.23 4.66 8.66");

            this.engine.Execute<AType>("$pp 3");
            AType result = this.engine.Execute<AType>("form 4.234 4.6567 8.6556");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFloatMatrix1()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("    2        42.1   "),
                Helpers.BuildString(" 4333.44      2.8743"),
                Helpers.BuildString("   24.7674    2     ")
            );

            AType result = this.engine.Execute<AType>("form 3 2 rho 2 42.1 4333.44 2.8743 24.7674");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFloatMatrix2()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString(" 6 6"),
                Helpers.BuildString(" 6 6")
            );

            AType result = this.engine.Execute<AType>("form 2 2 rho 6.0");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFloatMatrix3()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("   0.5   43.23  -4.3 "),
                Helpers.BuildString("  -0.3  224      0.5 ")
            );

            AType result = this.engine.Execute<AType>("form 2 3 rho 0.5 43.23 -4.3 -0.3 224");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatSymbolVector()
        {
            AType expected = Helpers.BuildString(" `a `bc `abc `");

            AType result = this.engine.Execute<AType>("form `a `bc `abc `");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatSymbolMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString(" `a    `abcd `bc  "),
                Helpers.BuildString(" `abc  `     `a   "),
                Helpers.BuildString(" `abcd `bc   `abc ")
            );

            AType result = this.engine.Execute<AType>("form 3 3 rho `a `abcd `bc `abc `");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFloatList2()
        {
            AType expected = Helpers.BuildString(" 1.06 1.16 1.26 1.36 1.46 1.56 1.66 1.76 1.86 1.96");

            this.engine.Execute<AType>("$pp 3");
            AType result = this.engine.Execute<AType>("form 1.055 1.155 1.255 1.355 1.455 1.555 1.655 1.755 1.855 1.955");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFunctionScalar1()
        {
            AType expected = Helpers.BuildString("+");

            AType result = this.engine.Execute<AType>("form{<{+}}");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("DefaultFormat"), TestMethod]
        public void DefaultFormatFunctionScalar2()
        {
            AType expected = Helpers.BuildString("*derived*");

            AType result = this.engine.Execute<AType>("form{<{(+ each) each}}");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("DefaultFormat"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DefaultFormatTypeError()
        {
            AType result = this.engine.Execute<AType>("form <6");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("DefaultFormat"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DefaultFormatRankError1()
        {
            AType result = this.engine.Execute<AType>("form (2;4;62)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("DefaultFormat"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DefaultFormatRankError2()
        {
            AType result = this.engine.Execute<AType>("form{`a`b , <{+}}");
        }
    }
}
