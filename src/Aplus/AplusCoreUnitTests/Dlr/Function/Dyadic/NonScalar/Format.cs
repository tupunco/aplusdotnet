using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Format : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2Float1()
        {
            AType expected = Helpers.BuildString("             1.1");

            AType result = this.engine.Execute<AType>("16.12 form 1.123456789012");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2Float1Uni()
        {
            AType expected = Helpers.BuildString("             1.1");

            AType result = this.engineUni.Execute<AType>("16.12 E.% 1.123456789012");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2Float2()
        {
            AType expected = Helpers.BuildString(" 12345.6789120");

            AType result = this.engine.Execute<AType>("14.7 form 12345.678912");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2Float3()
        {
            AType expected = Utils.ANull(ATypes.AChar);

            AType result = this.engine.Execute<AType>(".7 form 123");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2Float4()
        {
            AType expected = Helpers.BuildString("4343.434");

            AType result = this.engine.Execute<AType>("8.3 form 4343.434367676");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2Float5()
        {
            AType expected = Helpers.BuildString("876.34");

            AType result = this.engine.Execute<AType>("6.6 form 876.34344");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatInteger2Float1()
        {
            AType expected = Helpers.BuildString(" 32");

            AType result = this.engine.Execute<AType>("3 form 32.443");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatInteger2Float2()
        {
            AType expected = Helpers.BuildString("   3516");

            AType result = this.engine.Execute<AType>("7.0 form 3515.5445");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatNegativeFloat2Integer1()
        {
            AType expected = Helpers.BuildString("  5.00e+02   ");

            AType result = this.engine.Execute<AType>("-13.2 form 500");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatNegativeInteger2NegativeInteger1()
        {
            AType expected = Helpers.BuildString(" -4e+03       ");

            AType result = this.engine.Execute<AType>("-14 form -3500");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatInteger2IntegerList()
        {
            AType expected = Helpers.BuildString("   0   2   2   4   4");

            AType result = this.engine.Execute<AType>("4 form .5 1.5 2.5 3.5 4.5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2FloatList()
        {
            AType expected = Helpers.BuildString("  1.06  1.16  1.26  1.36  1.46  1.56  1.66  1.76  1.86  1.96");

            AType result = this.engine.Execute<AType>("6.2 form 1.055 1.155 1.255 1.355 1.455 1.555 1.655 1.755 1.855 1.955");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2FloatMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("  32.430   3.445 300.000"),
                Helpers.BuildString(" -32.130 134.532   8.835")
            );

            AType result = this.engine.Execute<AType>("8.3 form 2 3 rho 32.43 3.4445 3e2 -32.13 134.532 8.83455");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloatMatrix2FloatMatrix1()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString(" 32.43   3.445  300.0"),
                Helpers.BuildString("-32.13 134.532    8.8")
            );

            AType result = this.engine.Execute<AType>("(3 1 rho 6.2 8.3 7.1) form 2 3 rho 32.43 3.4445 3e2 -32.13 134.532 8.83455");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloatList2FloatMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString(" 1   -3.1416  1.23e+05  "),
                Helpers.BuildString(" 2  123.7000 -5.50e+01  ")
            );

            AType result = this.engine.Execute<AType>("2 10.4 -12.2 form 2 3 rho 1 -3.14159 123456 2 123.7 -55");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2Symbol()
        {
            AType expected = Helpers.BuildString("abcd");

            AType result = this.engine.Execute<AType>("4.6 form `abcdefg");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatFloat2SymbolMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString(" ab   abcd   a"),
                Helpers.BuildString("       abc  ab")
            );

            AType result = this.engine.Execute<AType>("3.4 7 4.6 form 2 3 rho `ab `abcd `a ` `abc");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatNegativeFloat2SymbolMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("    ab"),
                Helpers.BuildString(" ab a ")
            );

            AType result = this.engine.Execute<AType>("-3.4 -3 form 2 2 rho ` `ab `abcd `a");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        public void FormatInteger2Null()
        {
            AType expected = Utils.ANull(ATypes.AChar);

            AType result = this.engine.Execute<AType>("4 form ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FormatLengthError1()
        {
            AType result = this.engine.Execute<AType>("14.3 21.4 form 4343.5555");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FormatLengthError2()
        {
            AType result = this.engine.Execute<AType>("14.3 21.4 12.3 form 64.22 4343.5555");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FormatLengthError3()
        {
            AType result = this.engine.Execute<AType>("14.3 21.4 12.3 form iota 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FormatLengthError4()
        {
            AType result = this.engine.Execute<AType>("14.3 21.4 12.3 form ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FormatLengthError5()
        {
            AType result = this.engine.Execute<AType>("() form 5543.5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FormatLengthError6()
        {
            AType result = this.engine.Execute<AType>("() form 5543.5 7.5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void FormatTypeError1()
        {
            AType result = this.engine.Execute<AType>("2.3 form 'abc'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void FormatTypeError2()
        {
            AType result = this.engine.Execute<AType>("`a form 4.5 .73");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Format"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void FormatTypeError3()
        {
            AType result = this.engine.Execute<AType>("3.4 2.3 4.3 form `a`b , <{+}");
        }
    }
}
