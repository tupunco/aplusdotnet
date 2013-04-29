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
    public class Power : AbstractTest
    {
        /*==============================
		        x = (any value) :
        ================================
	        y = 0	|		1           */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToAnyValueAndYEqualTo0()
        {
            AType expected = AFloat.Create(1);
            AType result = this.engine.Execute<AType>("6 ^ 0");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToAnyValueAndYEqualTo0Uni()
        {
            AType expected = AFloat.Create(1);
            AType result = this.engineUni.Execute<AType>("6 M.* 0");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        /*================================
			        x = -Inf :
        ================================
	        y < 0		|	0
        --------------------------------
	        y > 0 and	|	Inf
	        y is even	|
        --------------------------------
	        y > 0 and	|	-Inf
	        y is odd	|
        --------------------------------
	        y = Inf		|	Inf
        --------------------------------
	        y = -Inf    	|	0          */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegativeInfAndYLessThan0()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("-Inf ^ -4");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegativeInfAndYGreaterThan0AndYisOdd()
        {
            AType expected = AFloat.Create(Double.NegativeInfinity);
            AType result = this.engine.Execute<AType>("-Inf ^ 3");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegativeInfAndYGreaterThan0AndYisEven()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("-Inf ^ 8");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegativeInfAndYEqualToInf()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("-Inf ^ -Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegativeInfAndYEqualToNegativeInf()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("-Inf ^ Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        /*==============================
			        x < -1 :
        ================================
	        y < 0 and 	|	x ^ y
	        y is whole	|
        --------------------------------
	        y > 0 and 	|	x ^ y
	        y is whole	|
        --------------------------------
	        y = Inf		|	Inf
        --------------------------------
	        y = -Inf	    |	0           */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXLessThanNegative1AndYLessThan0AndYisWhole()
        {
            AType expected = AFloat.Create(Math.Pow(-2,-4));
            AType result = this.engine.Execute<AType>("-2 ^ -4");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXLessThanNegative1AndYGreaterThanThan0AndYisWhole()
        {
            AType expected = AFloat.Create(Math.Pow(-3, 9));
            AType result = this.engine.Execute<AType>("-3 ^ 9");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXLessThanNegative1AndYEqualToInf()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("-4 ^ -Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXLessThanNegative1AndYEqualToNegativeInf()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("-8 ^ Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PowerDomainErrorNaN()
        {
            AType result = this.engine.Execute<AType>("-2 ^ -5.2");
        }

        /*================================
			        x = -1 :
        ================================
        y < 0 and 		|	-1
        y is whole and	|
        y is odd		|
        --------------------------------
        y < 0 and 		|	1
        y is whole and	|
        y is even		|
        --------------------------------
        y > 0 and 		|	1
        y is whole and	|
        y is even		|
        --------------------------------
        y > 0 and 		|	-1
        y is whole and	|
        y is odd		|
        --------------------------------
	        y = Inf		|	1
        --------------------------------
	        y = -Inf    	|	1           */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegative1AndYLessThan0AndYisWholeAndYisOdd()
        {
            AType expected = AFloat.Create(Math.Pow(-1, -5));
            AType result = this.engine.Execute<AType>("-1 ^ -5");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegative1AndYLessThan0AndYisWholeAndYisEven()
        {
            AType expected = AFloat.Create(Math.Pow(-1, -6));
            AType result = this.engine.Execute<AType>("-1 ^ -6");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegative1AndYGreaterThan0AndYisWholeAndYisOdd()
        {
            AType expected = AFloat.Create(Math.Pow(1, 7));
            AType result = this.engine.Execute<AType>("1 ^ 7");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegative1AndYGreaterThan0AndYisWholeAndYisEven()
        {
            AType expected = AFloat.Create(Math.Pow(1, 8));
            AType result = this.engine.Execute<AType>("1 ^ 8");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegative1AndYEqualToInf()
        {
            AType expected = AFloat.Create(1);
            AType result = this.engine.Execute<AType>("-1 ^ -Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToNegative1AndYEqualToNegativeInf()
        {
            AType expected = AFloat.Create(1);
            AType result = this.engine.Execute<AType>("-1 ^ Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        /*==============================
			        -1 < x < 0:
        ================================
        y < 0 and 		|	x ^ y
        y is whole		|
        --------------------------------
        y > 0 and 		|	x ^ y
        y is whole		|
        --------------------------------
	        y = Inf		|	Inf
        --------------------------------
	        y = -Inf    	|	0           */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThanNegative1AndXLessThan0AndYLessThan0AndYisWhole()
        {
            AType expected = AFloat.Create(Math.Pow(-.4, -5));
            AType result = this.engine.Execute<AType>("-.4 ^ -5");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThanNegative1AndXLessThan0AndYGreaterThan0AndYisWhole()
        {
            AType expected = AFloat.Create(Math.Pow(-.3, 8));
            AType result = this.engine.Execute<AType>("-.3 ^ 8");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        /*==============================
			        x = 0 :
        ================================
	        y < 0	 	|	Inf
        --------------------------------
	        y > 0	 	|	0
        --------------------------------
	        y = Inf		|	0
        --------------------------------
	        y = -Inf	    |	Inf         */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualTo0AndYLessThan0()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("0 ^ -5.7");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualTo0AndYGreaterThan0()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("0 ^ 8.7");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        /*================================
			        0 < x < 1:
        ================================
	        y < 0	 	|	x ^ y
        --------------------------------
	        y > 0	 	|	x ^ y
        --------------------------------
	        y = Inf		|	0
        --------------------------------
	        y = -Inf    	|	Inf        */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThan0AndLEssThan1AndYLessThan0()
        {
            AType expected = AFloat.Create(Math.Pow(.4,-5.7));
            AType result = this.engine.Execute<AType>(".4 ^ -5.7");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThan0AndLEssThan1AndYGreaterThan0()
        {
            AType expected = AFloat.Create(Math.Pow(.4, 8.8));
            AType result = this.engine.Execute<AType>(".4 ^ 8.8");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        /*==============================
			        x = 1 :
        ================================
	        y = Inf		|	Domain Error
        --------------------------------
	        y = -Inf	    |	Domain Error
        --------------------------------
        y = (any value)	|	1           */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualTo1AndYisAnyValue()
        {
            AType expected = AFloat.Create(1);
            AType result = this.engine.Execute<AType>("1 ^ 6.6");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PowerDomainError5()
        {
            AType result = this.engine.Execute<AType>("1 ^ Inf");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PowerDomainError6()
        {
            AType result = this.engine.Execute<AType>("1 ^ -Inf");
        }

        /*==============================
			        x > 1 :
        ================================
	        y < 0	 	|	x ^ y
        --------------------------------
	        y > 0	 	|	x ^ y
        --------------------------------
	        y = Inf		|	Inf
        --------------------------------
	        y = -Inf    	|	0           */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThan1AndYLessThan0()
        {
            AType expected = AFloat.Create(Math.Pow(2,-8.4));
            AType result = this.engine.Execute<AType>("2 ^ -8.4");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThan1AndYGreaterThan0()
        {
            AType expected = AFloat.Create(Math.Pow(6,3.2));
            AType result = this.engine.Execute<AType>("6 ^ 3.2");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThan1AndYEqualToNegativeInf()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("3 ^ -Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXGreaterThan1AndYEqualToInf()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("5 ^ Inf");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        /*==============================
			        x = Inf :
        ================================
	        y < 0	 	|	0
        --------------------------------
	        y > 0	 	|	Inf
        --------------------------------
	        y = Inf		|	Inf
        --------------------------------
	        y = -Inf    	|	0           */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToInfAndYLessThan0()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("Inf ^ -7.4");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerXEqualToInfAndYGreaterThan0()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("Inf ^ 3.2");

            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerInteger2Vector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(1),
                AFloat.Create(Math.Pow(2,0.5)),
                AFloat.Create(2),
                AFloat.Create(4),
                AFloat.Create(8),
                AFloat.Create(16),
                AFloat.Create(32),
                AFloat.Create(64),
                AFloat.Create(128),
                AFloat.Create(256),
                AFloat.Create(Double.PositiveInfinity)
            );
            AType result = this.engine.Execute<AType>("2 ^ 0 .5 1 2 3 4 5 6 7 8 1025");
            Assert.AreEqual(expected, result);
            Assert.IsTrue(result.Type == ATypes.AFloat);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        public void PowerNull2Null()
        {
            AType result = this.engine.Execute<AType>("() ^ ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Incorrect type");
        }
    }
}
