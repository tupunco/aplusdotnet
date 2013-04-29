using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class Circle : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainError()
        {
            this.engine.Execute("8 pi 3.14");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainErrorUni()
        {
            this.engineUni.Execute("8 M.^ 3.14");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeCos()
        {
            AType expected = AFloat.Create(Math.Cos(3.14));

            AType result = this.engine.Execute<AType>("`cos pi 3.14");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ComputeArcoshNegativeInfinity()
        {
            AType result = this.engine.Execute<AType>("`arccosh pi -Inf");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ArcSinhNegativeInfinity()
        {
            AType expected = AFloat.Create(Double.NegativeInfinity);

            AType result = this.engine.Execute<AType>("-5 pi -Inf");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void NullWithInt()
        {
            AType result = this.engine.Execute<AType>("() pi 3");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void NullWithNull()
        {
            AType result = this.engine.Execute<AType>("() pi ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinArcCosWithInt()
        {
            AType result = this.engine.Execute<AType>("0 pi .5");

            AType expected = AFloat.Create(Math.Pow(1 - Math.Pow(0.5, 2),0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinArcCosWithFloat()
        {
            AType result = this.engine.Execute<AType>(".1 pi .2");

            AType expected = AFloat.Create(Math.Pow(1 - Math.Pow(0.2, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinArcCosWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`sinarccos pi 1");

            AType expected = AFloat.Create(Math.Pow(1 - Math.Pow(1, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void SinArcCosDomainError()
        {
            AType result = this.engine.Execute<AType>("0.1 pi 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinWithInt()
        {
            AType result = this.engine.Execute<AType>("1 pi 3");

            AType expected = AFloat.Create(Math.Sin(3));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinWithFloat()
        {
            AType result = this.engine.Execute<AType>("1.9 pi 6");

            AType expected = AFloat.Create(Math.Sin(6));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`sin pi 4");

            AType expected = AFloat.Create(Math.Sin(4));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void SinDomainError()
        {
            AType result = this.engine.Execute<AType>("1 pi Inf");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanWithInt()
        {
            AType result = this.engine.Execute<AType>("3 pi 3");

            AType expected = AFloat.Create(Math.Tan(3));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanWithFloat()
        {
            AType result = this.engine.Execute<AType>("3.7 pi 21");

            AType expected = AFloat.Create(Math.Tan(21));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`tan pi 5.6");

            AType expected = AFloat.Create(Math.Tan(5.6));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSecArcTanWithInt()
        {
            AType result = this.engine.Execute<AType>("4 pi 4");

            AType expected = AFloat.Create(Math.Pow(1 + Math.Pow(4, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSecArcTanWithFloat()
        {
            AType result = this.engine.Execute<AType>("4.33333 pi 2");

            AType expected = AFloat.Create(Math.Pow(1 + Math.Pow(2, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSecArcTanWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`secarctan pi 6.6");

            AType expected = AFloat.Create(Math.Pow(1 + Math.Pow(6.6, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinhWithInt()
        {
            AType result = this.engine.Execute<AType>("5 pi 3");

            AType expected = AFloat.Create(Math.Sinh(3));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinhWithFloat()
        {
            AType result = this.engine.Execute<AType>("5.76 pi 2");

            AType expected = AFloat.Create(Math.Sinh(2));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeSinhWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`sinh pi 0.3");

            AType expected = AFloat.Create(Math.Sinh(0.3));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeCoshWithInt()
        {
            AType result = this.engine.Execute<AType>("6 pi 54");

            AType expected = AFloat.Create(Math.Cosh(54));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeCoshWithFloat()
        {
            AType result = this.engine.Execute<AType>("6.3 pi 2");

            AType expected = AFloat.Create(Math.Cosh(2));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeCoshWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`cosh pi 1.1");

            AType expected = AFloat.Create(Math.Cosh(1.1));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanhWithInt()
        {
            AType result = this.engine.Execute<AType>("7 pi Inf");

            AType expected = AFloat.Create(Math.Tanh(Double.PositiveInfinity));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanhWithFloat()
        {
            AType result = this.engine.Execute<AType>("7.9 pi 0.4");

            AType expected = AFloat.Create(Math.Tanh(0.4));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanhWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`tanh pi 8.2");

            AType expected = AFloat.Create(Math.Tanh(8.2));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcSinWithInt()
        {
            AType result = this.engine.Execute<AType>("-1 pi .4");

            AType expected = AFloat.Create(Math.Asin(0.4));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcSinWithFloat()
        {
            AType result = this.engine.Execute<AType>("-1.37 pi -.3");

            AType expected = AFloat.Create(Math.Asin(-0.3));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcSinWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`arcsin pi -.8");

            AType expected = AFloat.Create(Math.Asin(-0.8));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ArcSinDomainError()
        {
            AType result = this.engine.Execute<AType>("`arcsin pi 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcCosWithInt()
        {
            AType result = this.engine.Execute<AType>("-2 pi -.832");

            AType expected = AFloat.Create(Math.Acos(-0.832));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcCosWithFloat()
        {
            AType result = this.engine.Execute<AType>("-2.76 pi -.7654");

            AType expected = AFloat.Create(Math.Acos(-0.7654));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcCosWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`arccos pi .11111");

            AType expected = AFloat.Create(Math.Acos(0.11111));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ArcCosDomainError()
        {
            AType result = this.engine.Execute<AType>("`arcos pi -5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcTanWithInt()
        {
            AType result = this.engine.Execute<AType>("-3 pi -22");

            AType expected = AFloat.Create(Math.Atan(-22));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcTanWithFloat()
        {
            AType result = this.engine.Execute<AType>("-3.44 pi -Inf");

            AType expected = AFloat.Create(Math.Atan(Double.NegativeInfinity));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcTanWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`arctan pi .33333");

            AType expected = AFloat.Create(Math.Atan(0.33333));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanArcSecWithInt()
        {
            AType result = this.engine.Execute<AType>("-4 pi 33");

            AType expected = AFloat.Create(Math.Pow(-1 + Math.Pow(33, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanArcSecWithFloat()
        {
            AType result = this.engine.Execute<AType>("-4.87 pi 2");

            AType expected = AFloat.Create(Math.Pow(-1 + Math.Pow(2, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeTanArcSecWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`tanarcsec pi 2.23");

            AType expected = AFloat.Create(Math.Pow(-1 + Math.Pow(2.23, 2), 0.5));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void TanArcSecDomainError()
        {
            AType result = this.engine.Execute<AType>("-4 pi -0.5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcSinhWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`arcsinh pi 3");

            AType expected = AFloat.Create(Math.Log(3 + Math.Sqrt(1 + Math.Pow(3, 2))));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcSinhWithFloat()
        {
            AType result = this.engine.Execute<AType>("-5 pi 2");

            AType expected = AFloat.Create(Math.Log(2 + Math.Sqrt(1 + Math.Pow(2, 2))));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcCoshWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`arccosh pi 7");

            AType expected = AFloat.Create(Math.Log(7 + Math.Sqrt(8) * Math.Sqrt(6)));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcCoshWithFloat()
        {
            AType result = this.engine.Execute<AType>("-6.53 pi 9");

            AType expected = AFloat.Create(Math.Log(9 + Math.Sqrt(10) * Math.Sqrt(8)));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcTanhWithSymbolConstant()
        {
            AType result = this.engine.Execute<AType>("`arctanh pi -.3");

            AType expected = AFloat.Create(0.5 * (Math.Log(0.7) - Math.Log(1.3)));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Circle"), TestMethod]
        public void ComputeArcTanhWithFloat()
        {
            AType result = this.engine.Execute<AType>("-7.9 pi .5");

            AType expected = AFloat.Create(0.5 * (Math.Log(1.5) - Math.Log(0.5)));

            Assert.AreEqual(expected, result);
        }

    }
}
