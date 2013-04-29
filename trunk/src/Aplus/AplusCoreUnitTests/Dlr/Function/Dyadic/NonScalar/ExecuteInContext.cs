using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class ExecuteInContext : AbstractTest
    {
        #region Execute In Context

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Execute In Context"), TestMethod]
        public void GlobalAssignExecute()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected = AInteger.Create(10);
            AType result = this.engine.Execute<AType>("`X eval 'b:=10'", scope);

            Assert.IsTrue(scope.ContainsVariable("X.b"), "No variable found in global scope");
            Assert.AreEqual<AType>(expected, scope.GetVariable<AType>("X.b"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Execute In Context"), TestMethod]
        public void GlobalAssignExecuteUni()
        {
            ScriptScope scope = this.engineUni.CreateScope();
            AType expected = AInteger.Create(10);
            AType result = this.engineUni.Execute<AType>("`X E.* 'b:=10'", scope);

            Assert.IsTrue(scope.ContainsVariable("X.b"), "No variable found in global scope");
            Assert.AreEqual<AType>(expected, scope.GetVariable<AType>("X.b"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Execute In Context"), TestMethod]
        public void FunctionAssignExecute()
        {
            AType expected = AInteger.Create(20);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable("X.a", AInteger.Create(1));

            this.engine.Execute<AType>("f{}: { a:=20; `X eval 'a' }", scope);

            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>("X.a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Execute In Context"), TestMethod]
        public void FunctionCombinedLookupsExecute()
        {
            AType expected = AInteger.Create(21);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable("X.a", AInteger.Create(1));
            scope.SetVariable("Y.a", AInteger.Create(1));

            this.engine.Execute<AType>("f{}: { a:=20; `X eval 'a + Y.a' }", scope);

            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>("Y.a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>("X.a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Execute In Context"), TestMethod]
        public void FunctionDyadicDoExecute()
        {
            AType expected = AInteger.Create(2);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", AInteger.Create(500));

            this.engine.Execute<AType>("f{}: { a:=10; `X eval ' (a:=2) do { drop a }'; a }", scope);

            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.IsFalse(scope.ContainsVariable("X.a"), "Variable is defined in context");
            Assert.AreEqual<AType>(AInteger.Create(500), scope.GetVariable<AType>(".a"), "Incorrect result in global variable");
            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        #endregion

        #region Protected Execute

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Protected Execute"), TestMethod]
        public void ProtectedExecute()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected = ABox.Create(AInteger.Create(20));
            AType result = this.engine.Execute<AType>("1 eval '10 + 10'", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect result returned");
        }

        #endregion
    }
}
