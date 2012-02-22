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
    public class Log : AbstractTest
    {

        /*==============================
			           y < 0 :
        ================================
        x = (any value)	|	Domain Error */


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError0()
        {
            AType result = this.engine.Execute<AType>("-2 log 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError1()
        {
            AType result = this.engine.Execute<AType>("-2 log -3.4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError2()
        {
            AType result = this.engine.Execute<AType>("-2 log 8.4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError3()
        {
            AType result = this.engine.Execute<AType>("-1 log 8.4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Power"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError4()
        {
            AType result = this.engine.Execute<AType>("-1 log -8.5");
        }

        /*===============================
		        y = (any value)	:
         ================================
	        x < 0		|	Domain Error*/

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError5()
        {
            AType result = this.engine.Execute<AType>("2 log -8");
        }


        /*==============================
			        y = 0 :
        ================================
	        x = 0		|	Domain Error
        --------------------------------
	        x > 0		|	0
        --------------------------------
	        x = Inf		|	Domain Error*/

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogYEqualTo0AndXGreaterThan0()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("0 log 3");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError6()
        {
            AType result = this.engine.Execute<AType>("0 log 0");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError7()
        {
            AType result = this.engine.Execute<AType>("0 log Inf");
        }

        /*==============================
		        (0 < y < 1) :
        ================================
	        x = 0		|	Inf
        --------------------------------
	        x > 0		|	log_y x
        --------------------------------
	        x = Inf		|	-Inf        */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogYGreaterThan0AndYLessThan1AndXEqualTo0()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("0.4 log 0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogYGreaterThan0AndYLessThan1AndXGreaterThan0()
        {
            AType expected = AFloat.Create(Math.Log(6,.7));
            AType result = this.engine.Execute<AType>("0.7 log 6");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogYGreaterThan0AndYLessThan1AndXEqualToPositiveInf()
        {
            AType expected = AFloat.Create(Double.NegativeInfinity);
            AType result = this.engine.Execute<AType>("0.2 log Inf");

            Assert.AreEqual(expected, result);
        }

        /*==============================
			        y = 1 :
        ================================
	        x > 1		|	Inf
        --------------------------------
	        x = 1		|	Domain Error
        --------------------------------
	        0 <= x < 1	|	-Inf        */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogXGreaterThan0AndYEqualTo1()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("1 log 8");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogXGreaterThanOrEqualTo0AndXLessThan1AndYEqualTo1()
        {
            AType expected = AFloat.Create(Double.NegativeInfinity);
            AType result = this.engine.Execute<AType>("1 log .7");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError8()
        {
            AType result = this.engine.Execute<AType>("1 log 1");
        }

        /*==============================
			        y > 1 :
        ================================
	        x = 0		|	-Inf
        --------------------------------
	        x > 0		|	log_y x
        --------------------------------
	        x = Inf		|	Inf         */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogXEqualTo0AndYGreaterThan1()
        {
            AType expected = AFloat.Create(Double.NegativeInfinity);
            AType result = this.engine.Execute<AType>("6 log 0");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogXGreaterThan0AndYGreaterThan0()
        {
            AType expected = AFloat.Create(3);
            AType result = this.engine.Execute<AType>("2 log 8");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogXEqualToPositiveInf0AndYGreaterThan0()
        {
            AType expected = AFloat.Create(Double.PositiveInfinity);
            AType result = this.engine.Execute<AType>("8 log Inf");

            Assert.AreEqual(expected, result);
        }

        /*==============================
			        y = Inf :
        ================================
	        x = 0		|	Domain Error
        --------------------------------
	        x > 0		|	0
        --------------------------------
	        x = Inf		|	Domain Error */

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError9()
        {
            AType result = this.engine.Execute<AType>("Inf log 0");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void LogDomainError10()
        {
            AType result = this.engine.Execute<AType>("Inf log Inf");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogXGreaterThan0AndYEqualToPositiveInf()
        {
            AType expected = AFloat.Create(0);
            AType result = this.engine.Execute<AType>("Inf log 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogInteger2Null()
        {
            AType result = this.engine.Execute<AType>("1 log ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Log"), TestMethod]
        public void LogNull2Null()
        {
            AType result = this.engine.Execute<AType>("() log ()");

            Assert.AreEqual<ATypes>(ATypes.AFloat, result.Type, "Type mismatch");
        }
    }
}
