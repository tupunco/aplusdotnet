using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using AplusCore.Compiler;

namespace AplusCoreUnitTests.Dlr.Assignments
{
    [TestClass]
    public class Strand : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        public void SimpleStrandAssign()
        {
            var scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(a;b) := (1;2)", scope);

            Assert.IsTrue(scope.ContainsVariable(".a"), "No variable found");
            Assert.IsTrue(scope.ContainsVariable(".b"), "No variable found");

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>(".a"), "Incorrect assignment performed");
            Assert.AreEqual<AType>(AInteger.Create(2), scope.GetVariable<AType>(".b"), "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        public void SimpleStrandAssign2()
        {
            var scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(a;b) := 1 2", scope);

            Assert.IsTrue(scope.ContainsVariable(".a"), "No variable found");
            Assert.IsTrue(scope.ContainsVariable(".b"), "No variable found");

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>(".a"), "Incorrect assignment performed");
            Assert.AreEqual<AType>(AInteger.Create(2), scope.GetVariable<AType>(".b"), "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        public void ComplexStrandAssign()
        {
            var scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(a;b) := (c;d) := (1;2)", scope);

            Assert.IsTrue(scope.ContainsVariable(".a"), "No variable found");
            Assert.IsTrue(scope.ContainsVariable(".b"), "No variable found");
            Assert.IsTrue(scope.ContainsVariable(".c"), "No variable found");
            Assert.IsTrue(scope.ContainsVariable(".d"), "No variable found");

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>(".a"), "Incorrect assignment performed");
            Assert.AreEqual<AType>(AInteger.Create(2), scope.GetVariable<AType>(".b"), "Incorrect assignment performed");

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>(".c"), "Incorrect assignment performed");
            Assert.AreEqual<AType>(AInteger.Create(2), scope.GetVariable<AType>(".d"), "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        [Description("Test ensures that changing context does not change the target context.")]
        public void StrandAssignTargetCorrection()
        {
            var scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(a;b) := ( {eval '$cx A'; 5 }; 7 )", scope);

            Assert.IsTrue(scope.ContainsVariable(".a"), "No variable found");
            Assert.IsTrue(scope.ContainsVariable(".b"), "No variable found");

            Assert.IsFalse(scope.ContainsVariable("A.a"), "Variable found! Shouldn't be.");
            Assert.IsFalse(scope.ContainsVariable("A.b"), "Variable found! Shouldn't be.");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void StrandAssignParseException()
        {
            this.engine.Execute<AType>("(a;a[0]) := (1;2)");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void StrandAssignParseException2()
        {
            this.engine.Execute<AType>("(a;2) := (1;2)");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void StrandAssignLengthError1()
        {
            this.engine.Execute<AType>("(a;b) := (1;2;3)");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("Strand"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void StrandAssignLengthError2()
        {
            this.engine.Execute<AType>("(a;b) := 1 2 3");
        }
    }
}
