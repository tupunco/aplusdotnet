using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.ControlFlow
{
    [TestClass]
    public class Case : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void SimpleCase()
        {
            AType expected = Helpers.BuildString("found it");

            AType result = this.engine.Execute<AType>("case (1) { 2; 'hello'; 1; 'found it' }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void SimpleMemberTestCase()
        {
            AType expected = Helpers.BuildString("found it");

            AType result = this.engine.Execute<AType>("case (1) { 2 3 ; 'hello'; 1 2; 'found it' }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void DefaultCase()
        {
            AType expected = Helpers.BuildString("default");

            AType result = this.engine.Execute<AType>("case (0) { 1; 'hello'; 1; 'found it'; 'default' }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void NoDefaultCase()
        {
            AType result = this.engine.Execute<AType>("case (0) { 2; 'hello'; 1; 'found it' }");

            Assert.IsTrue(result.Type == ATypes.ANull, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void EmptyCase()
        {
            AType expected = AInteger.Create(3);
            AType result = this.engine.Execute<AType>("case (0) { 3 }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void OnlyDefaultCase()
        {
            AType expected = AInteger.Create(-123);
            AType result = this.engine.Execute<AType>("case (Z:=-124) { Z:=Z + 1 }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void MultipleSameCase()
        {
            AType expected = Helpers.BuildString("hello");

            AType result = this.engine.Execute<AType>("case (1) { 1; 'hello'; 1; 'found it' }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void NoTypeErrorCase()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("case ('error') { 'error'; 0; 1; 'found it' }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorCase()
        {
            this.engine.Execute<AType>("case ('error') { 1; 'hello'; 1; 'found it' }");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void NullTargetCase()
        {
            AType result = this.engine.Execute<AType>("case (()) { 2; 'hello'; 1; 'found it' }");

            Assert.IsTrue(result.Type == ATypes.ANull, "Incorrect result produced");
        }

        [TestCategory("DLR"), TestCategory("ControlFlow"), TestCategory("Case"), TestMethod]
        public void NullTargetWithDefaultCase()
        {
            AType expected = Helpers.BuildString("default");

            AType result = this.engine.Execute<AType>("case (()) { 2; 'hello'; 1; 'found it'; 'default' }");

            Assert.AreEqual<AType>(expected, result, "Incorrect result produced");
        }

    }
}
