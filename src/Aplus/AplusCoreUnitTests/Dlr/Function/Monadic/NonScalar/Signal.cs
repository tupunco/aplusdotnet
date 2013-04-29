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
    public class Signal : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory(""), TestMethod]
        [ExpectedException(typeof(Error.Signal))]
        public void SymbolSignal()
        {
            try
            {
                this.engine.Execute<AType>("take `sym");
            }
            catch (Error.Signal signal)
            {
                Assert.AreEqual<string>("sym", signal.Message, "Incorrect message inside the error");
                throw;
            }
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory(""), TestMethod]
        [ExpectedException(typeof(Error.Signal))]
        public void SymbolSignalUni()
        {
            try
            {
                this.engineUni.Execute<AType>("S.+ `sym");
            }
            catch (Error.Signal signal)
            {
                Assert.AreEqual<string>("sym", signal.Message, "Incorrect message inside the error");
                throw;
            }
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory(""), TestMethod]
        [ExpectedException(typeof(Error.Signal))]
        public void SymbolVectorSignal()
        {
            try
            {
                this.engine.Execute<AType>("take `this `and `that");
            }
            catch (Error.Signal signal)
            {
                Assert.AreEqual<string>("this", signal.Message, "Incorrect message inside the error");
                throw;
            }
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory(""), TestMethod]
        [ExpectedException(typeof(Error.Signal))]
        public void SymbolCharacterVectorSignal()
        {
            try
            {
                this.engine.Execute<AType>("take 'this away'");
            }
            catch (Error.Signal signal)
            {
                Assert.AreEqual<string>("this away", signal.Message, "Incorrect message inside the error");
                throw;
            }
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory(""), TestMethod]
        [ExpectedException(typeof(Error.Signal))]
        public void SymbolCharacterMatrixSignal()
        {
            try
            {
                this.engine.Execute<AType>("take 2 5 rho 'this away '");
            }
            catch (Error.Signal signal)
            {
                Assert.AreEqual<string>("this away ", signal.Message, "Incorrect message inside the error");
                throw;
            }
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory(""), TestMethod]
        public void ProtectedSignal()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    Helpers.BuildString("error"),
                    AInteger.Create((int)ErrorType.Signal)
                }
            );
            AType result = this.engine.Execute<AType>("do { take `error }");

            Assert.AreEqual<AType>(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory(""), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void NumberDomainErrorSignal()
        {
            this.engine.Execute<AType>("take 1");   
        }
    }
}
