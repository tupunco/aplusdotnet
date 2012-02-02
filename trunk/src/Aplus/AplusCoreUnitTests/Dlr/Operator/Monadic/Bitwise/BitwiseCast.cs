using System;

using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore;
using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic.Bitwise
{
    [TestClass]
    public class BitwiseCast : AbstractTest
    {
        #region CorrectCases

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void SymToInt()
        {
            AType expected = AInteger.Create(ASymbol.Create("valami").GetHashCode());
            AType result = this.engine.Execute<AType>("`int bwor `valami");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void SymToFloat()
        {
            // This test assumes that int to float convertion works good.

            AType hashAsInt = AArray.Create(
                                            ATypes.AInteger,
                                            AInteger.Create(ASymbol.Create("valami").GetHashCode()),
                                            AInteger.Create(ASymbol.Create("semmi").GetHashCode())
                              );
            ScriptScope scope = this.engine.CreateScope();

            scope.SetVariable("x", hashAsInt);
            AType expected = this.engine.Execute<AType>("`float bwor x", scope);
            AType result = this.engine.Execute<AType>("`float bwor `valami `semmi");

            Assert.AreEqual(expected, result);
        }
        
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void SymToChar()
        {
            // This test assumes that int to char convertion works good.

            AType hashAsInt = AInteger.Create(ASymbol.Create("valami").GetHashCode());
            ScriptScope scope = this.engine.CreateScope();

            scope.SetVariable("x", hashAsInt);
            AType expected = this.engine.Execute<AType>("`char bwor x", scope);
            AType result = this.engine.Execute<AType>("`char bwor `valami");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void IntToInt()
        {
            AType expected = this.engine.Execute<AType>("iota 3 3 3");
            AType result = this.engine.Execute<AType>("`int bwor iota 3 3 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void IntToFloat()
        {
            AType expected = AFloat.Create(1.2);
            AType result = this.engine.Execute<AType>("`float bwor 858993459 1072902963");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void IntMatrixToFloat()
        {
            AType expected =
                AArray.Create(
                              ATypes.AArray,
                              AArray.Create(ATypes.AFloat, AFloat.Create(1.1), AFloat.Create(1.1)),
                              AArray.Create(ATypes.AFloat, AFloat.Create(1.1), AFloat.Create(1.1))
                );
            AType result = this.engine.Execute<AType>("`float bwor 2 4 rho -1717986918 1072798105 -1717986918 1072798105 -1717986918 1072798105 -1717986918 1072798105");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void IntToChar()
        {
            AType expected = 
                AArray.Create(
                    ATypes.AArray,
                    Helpers.BuildString("jajjjajjjajj"),
                    Helpers.BuildString("jajjjajjjajj"),
                    Helpers.BuildString("jajjjajjjajj")
                );

            AType result = this.engine.Execute<AType>("`char bwor 3 3 rho 1785356650");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void IntMatrixToChar()
        {
            AType expected = Helpers.BuildString("kiskutya");
            AType result = this.engine.Execute<AType>("`char bwor 1802725739 1635349621");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void FloatToInt()
        {
            AType expected = new int[] { 858993459, 1072902963 }.ToAArray();
            AType result = this.engine.Execute<AType>("`int bwor 1.2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void FloatMatrixToInt()
        {
            AType expected = 
                AArray.Create(
                ATypes.AArray,
                new int[] { 858993459, 1072902963, 858993459, 1072902963 }.ToAArray(),
                new int[] { 858993459, 1072902963, 858993459, 1072902963 }.ToAArray()
            );

            AType result = this.engine.Execute<AType>("`int bwor 2 2 rho 1.2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void FloatToFloat()
        {
            AType expected = AFloat.Create(1.2);
            AType result = this.engine.Execute<AType>("`float bwor 1.2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void FloatToChar()
        {
            string expectedString = String.Concat("333333", (char)243, (char)63);
            AType expected = Helpers.BuildString(expectedString);
            AType result = this.engine.Execute<AType>("`char bwor 1.2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void CharToInt()
        {
            AType expected = AInteger.Create(1918986339);
            AType result = this.engine.Execute<AType>("`int bwor 'char'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void Char2DToInt()
        {
            AType expected =
                AArray.Create(ATypes.AInteger,
                              AArray.Create(ATypes.AInteger, AInteger.Create(1633771873)),
                              AArray.Create(ATypes.AInteger, AInteger.Create(1633771873)),
                              AArray.Create(ATypes.AInteger, AInteger.Create(1633771873)),
                              AArray.Create(ATypes.AInteger, AInteger.Create(1633771873))
                              );

            AType result = this.engine.Execute<AType>("`int bwor 4 4 rho 'a'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void CharToFloat()
        {
            string magicString = String.Concat("333333", (char)243, (char)63);
            AType expected = AFloat.Create(1.2);
            AType result = this.engine.Execute<AType>(String.Format("`float bwor '{0}'", magicString));

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void CharToChar()
        {
            AType expected = Helpers.BuildString("char");
            AType result = this.engine.Execute<AType>("`char bwor 'char'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        public void ShapeContainingNull()
        {
            AType expected = this.engine.Execute<AType>("4 0 1 rho 1");
            AType result = this.engine.Execute<AType>("`int bwor 4 0 4 rho 'a'");

            Assert.AreEqual(expected, result);
        }

        #endregion

        #region ErrorCases

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LeftArgumentArray()
        {
            this.engine.Execute("`int `float bwor 1 2 3 4");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ConvertInvalidToFloat()
        {
            this.engine.Execute("`float bwor ()");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ConvertInvalidToChar()
        {
            this.engine.Execute("`char bwor ()");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ConvertInvalidToInt()
        {
            this.engine.Execute("`int bwor ()");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ConvertCharToSym()
        {
            this.engine.Execute("`sym bwor 'a'");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ConvertIntToSym()
        {
            this.engine.Execute("`sym bwor 5");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ConvertFloatToSym()
        {
            this.engine.Execute("`sym bwor 5.5");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void NullArgument()
        {
            this.engine.Execute("`int bwor ()");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void BoxArgument()
        {
            this.engine.Execute("`int bwor <3");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void InvalidLength()
        {
            this.engine.Execute("`int bwor 'abc'");
        }
        
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ShapeContainingNullLengthError()
        {
            this.engine.Execute("`int bwor 4 0 3 rho 'a'");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Operator"), TestCategory("Bitwise Cast"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ScalarIntToFloat()
        {
            this.engine.Execute("`float bwor 0");
        }

        #endregion
    }
}
