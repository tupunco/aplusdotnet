using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.Hosting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Assignments
{
    [TestClass]
    public class Assign: AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void UnqualifiedSimpleAssign()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a:=6", scope);

            Assert.IsTrue(scope.ContainsVariable(".a"), "Variable not found");
            Assert.AreEqual(AInteger.Create(6), scope.GetVariable<AType>(".a"), "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void UnqualifiedMultipleAssign()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("b:=a:=6", scope);

            Assert.IsTrue(scope.ContainsVariable(".a"), "Variable not found");
            Assert.AreEqual(AInteger.Create(6), scope.GetVariable<AType>(".a"), "Incorrect value assigned");

            Assert.IsTrue(scope.ContainsVariable(".b"), "Variable not found");
            Assert.AreEqual(AInteger.Create(6), scope.GetVariable<AType>(".b"), "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void QualifiedSimpleAssign()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("DLR.a:=6", scope);

            Assert.IsTrue(scope.ContainsVariable("DLR.a"), "Variable not found");
            Assert.AreEqual(AInteger.Create(6), scope.GetVariable<AType>("DLR.a"), "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void GlobalVariableAssign()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{}: { (d):=7 }", scope);

            AType result = this.engine.Execute<AType>("a{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(7), scope.GetVariable<AType>(".d"), "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void VariableHandling()
        {
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".d", AInteger.Create(3));
            this.engine.Execute<AType>("a{}: { d:=-3; d + (d) }", scope);

            AType result = this.engine.Execute<AType>("a{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(-6), result, "Function call made incorrect calculation");
            Assert.AreEqual<AType>(AInteger.Create(3), scope.GetVariable<AType>(".d"), "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void VariableHandling2()
        {
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".d", AInteger.Create(3));
            this.engine.Execute<AType>("a{}: { (d):=-3; d + (d) }", scope);

            AType result = this.engine.Execute<AType>("a{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(-6), result, "Function call made incorrect calculation");
            Assert.AreEqual<AType>(AInteger.Create(-3), scope.GetVariable<AType>(".d"), "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void VariableHandling3()
        {
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".d", AInteger.Create(3));
            this.engine.Execute<AType>("a{}: { (d):=-3; d:=1; d + (d) }", scope);

            AType result = this.engine.Execute<AType>("a{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(2), result, "Function call made incorrect calculation");
            Assert.AreEqual<AType>(AInteger.Create(3), scope.GetVariable<AType>(".d"), "Function overwrote the variable");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestMethod]
        public void VariableHandling4()
        {
            AType expected = AInteger.Create(0);

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := b := iota 3", scope);
            this.engine.Execute<AType>("(1 # a) := 5", scope);
            
            AType result = this.engine.Execute<AType>("a == b", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
