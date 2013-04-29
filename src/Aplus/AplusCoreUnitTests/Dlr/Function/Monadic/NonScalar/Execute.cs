using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Execute : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void GlobalAssignExecute()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected = AInteger.Create(10);
            AType result = this.engine.Execute<AType>("eval 'b:=10'", scope);

            Assert.IsTrue(scope.ContainsVariable(".b"), "No variable found in global scope");
            Assert.AreEqual<AType>(expected, scope.GetVariable<AType>(".b"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void GlobalAssignExecuteUni()
        {
            ScriptScope scope = this.engineUni.CreateScope();
            AType expected = AInteger.Create(10);
            AType result = this.engineUni.Execute<AType>("E.* 'b:=10'", scope);

            Assert.IsTrue(scope.ContainsVariable(".b"), "No variable found in global scope");
            Assert.AreEqual<AType>(expected, scope.GetVariable<AType>(".b"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void ExecuteReturn()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected_return = AInteger.Create(10);
            AType expected_a = AInteger.Create(20);

            AType result = this.engine.Execute<AType>("eval 'a:=20; :=10; b:=30'", scope);

            Assert.IsFalse(scope.ContainsVariable(".b"), "Variable '.b' found in global scope");
            Assert.AreEqual<AType>(expected_a, scope.GetVariable<AType>(".a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected_return, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void ExecuteReturnUni()
        {
            ScriptScope scope = this.engineUni.CreateScope();
            AType expected_return = AInteger.Create(10);
            AType expected_a = AInteger.Create(20);

            AType result = this.engineUni.Execute<AType>("E.* 'a:=20; :=10; b:=30'", scope);

            Assert.IsFalse(scope.ContainsVariable(".b"), "Variable '.b' found in global scope");
            Assert.AreEqual<AType>(expected_a, scope.GetVariable<AType>(".a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected_return, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void ExecuteReturnInFunction()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected_return = AInteger.Create(11);
            AType expected_a = AInteger.Create(20);

            this.engine.Execute<AType>("f{}: { 1 + eval 'a:=20; :=10; b:=30' }", scope);
            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.IsFalse(scope.ContainsVariable(".b"), "Variable '.b' found in global scope");
            Assert.AreEqual<AType>(expected_a, scope.GetVariable<AType>(".a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected_return, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void FunctionAssignExecute()
        {
            AType expected = AInteger.Create(20);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", AInteger.Create(1));

            this.engine.Execute<AType>("f{}: { a:=20; eval 'a' }", scope);

            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>(".a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void FunctionCombinedLookupsExecute()
        {
            AType expected = AInteger.Create(21);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", AInteger.Create(1));

            this.engine.Execute<AType>("f{}: { a:=20; eval 'a + .a' }", scope);

            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>(".a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }


        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Execute"), TestMethod]
        public void FunctionDyadicDoExecute()
        {
            AType expected = AInteger.Create(2);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", AInteger.Create(500));

            this.engine.Execute<AType>("f{}: { a:=10; eval ' (a:=2) do { drop a }'; a }", scope);

            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(500), scope.GetVariable<AType>(".a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

    }
}
