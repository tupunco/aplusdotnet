using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;
using AplusCore.Runtime;
using System.Dynamic;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class UserDefinedFunction : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionDefinition()
        {
            AType result = this.engine.Execute<AType>("a{c}: c+1");

            Assert.IsTrue(result is AReference, "Function definition returned invalid type");

            Func<AplusEnvironment, AType, AType> function = (Func<AplusEnvironment, AType, AType>)((AFunc)result.Data).Method;
            Aplus aplusRuntime = this.engine.GetService<Aplus>();
            AType value = function(new AplusEnvironment(aplusRuntime, this.engine.CreateScope()), AInteger.Create(1));

            Assert.AreEqual<AType>(AInteger.Create(2), value, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{c}: c+1", scope);

            AType result = this.engine.Execute<AType>("a{1}", scope);

            Assert.AreEqual<AType>(AInteger.Create(2), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionCallVariableLeak()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{c}: { d:=1; c+1 }", scope);

            AType result = this.engine.Execute<AType>("a{1}", scope);

            Assert.AreEqual<AType>(AInteger.Create(2), result, "Function call made incorrect calculation");
            Assert.IsFalse(scope.ContainsVariable(".d"), "Variable should NOT exist");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        [ExpectedException(typeof(Error.Value))]
        public void Function2FunctionVariableLeak()
        {
            // variables defined in each function should not be visible for each other!

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{c}: { d:=1; e{1} }", scope);
            this.engine.Execute<AType>("e{f}: { d + f }", scope);

            this.engine.Execute<AType>("a{1}", scope);
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionCallGlobalVariableUnmodified()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("d:=10", scope);
            this.engine.Execute<AType>("a{c}: { d:=1; c+1 }", scope);

            AType result = this.engine.Execute<AType>("a{1}", scope);

            Assert.AreEqual<AType>(AInteger.Create(2), result, "Function call made incorrect calculation");
            Assert.AreEqual<AType>(AInteger.Create(10), scope.GetVariable<AType>(".d"), "Function modified global varibale");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionCallGlobalVariableModified()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("d:=10", scope);
            this.engine.Execute<AType>("a{c}: { .d:=1; c+1 }", scope);

            AType result = this.engine.Execute<AType>("a{1}", scope);

            Assert.AreEqual<AType>(AInteger.Create(2), result, "Function call made incorrect calculation");
            Assert.AreEqual<AType>(AInteger.Create(1), scope.GetVariable<AType>(".d"), "Function did NOT modified global varibale");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionCallWithDyadicDoLocalVar()
        {
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", AInteger.Create(10));
            scope.SetVariable(".counter", AInteger.Create(0));

            this.engine.Execute<AType>("method{}: { (a:=3) do { .counter := .counter + a } }", scope);

            AType result = this.engine.Execute<AType>("method{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(3), result, "Function call made incorrect calculation");
            Assert.AreEqual<AType>(AInteger.Create(3), scope.GetVariable<AType>(".counter"), "Function did NOT modified global variabale");
            Assert.AreEqual<AType>(AInteger.Create(10), scope.GetVariable<AType>(".a"), "Function modified global variabale");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionCallChain()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{x}: x+1", scope);
            this.engine.Execute<AType>("b{x}: 1 + a{x}", scope);

            AType result = this.engine.Execute<AType>("b{1}", scope);

            Assert.AreEqual<AType>(AInteger.Create(3), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionCallAndAdd()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{c}: c+1", scope);

            AType result = this.engine.Execute<AType>("a{1} + 1", scope);

            Assert.AreEqual<AType>(AInteger.Create(3), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionReturn()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{c}: { :=c; 7 }", scope);

            AType result = this.engine.Execute<AType>("a{1}", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionAssign()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType function_a = this.engine.Execute<AType>("a{c}: c+1", scope);
            AType function_b = this.engine.Execute<AType>("b:=a", scope);

            Assert.AreEqual<AType>(function_a, function_a);

            AType result = this.engine.Execute<AType>("b{1}", scope);

            Assert.AreEqual<AType>(AInteger.Create(2), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void FunctionValenceError()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType function_a = this.engine.Execute<AType>("a{c;d}: c+d", scope);
            AType function_b = this.engine.Execute<AType>("a{2}", scope);
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        [ExpectedException(typeof(Error.Value))]
        public void FunctionScopeTestSimple()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("z:=20", scope);
            this.engine.Execute<AType>("g{}: { z:=z }", scope);

            this.engine.Execute<AType>("g{}", scope);
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        [ExpectedException(typeof(Error.Value))]
        [Description("Test local assignment inside function and variable lookup.")]
        public void FunctionScopeTest()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("q:=20", scope);
            this.engine.Execute<AType>("g{}: { q; q:=2 }", scope);

            this.engine.Execute<AType>("g{}", scope);
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        [Description("Test global assignment inside function and variable lookup.")]
        public void FunctionGlobalScopeTest()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("q:=20", scope);
            this.engine.Execute<AType>("g{}: { q; (q):=2 }", scope);
            AType result = this.engine.Execute<AType>("g{}", scope);

            Assert.AreEqual<AType>(
                AInteger.Create(2),
                scope.GetVariable<AType>(".q"),
                "Incorrect value was assigned to the '.q' global variable"
            );
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        [Description("Test eval inside function and variable lookup.")]
        public void FunctionEvalScopeTest()
        {
            // inside an eval: there are global assignments
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("g{}: { eval 'q:=-2'; q }", scope);
            AType result = this.engine.Execute<AType>("g{}", scope);

            Assert.AreEqual<AType>(
                AInteger.Create(-2),
                scope.GetVariable<AType>(".q"),
                "Incorrect value was assigned to the '.q' global variable"
            );
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionGlobalScopeFallback()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("var := 10", scope);
            this.engine.Execute<AType>("method{}: { var + 1 }", scope);

            AType result = this.engine.Execute<AType>("method{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(11), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionLocalScope()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("var := 10", scope);
            this.engine.Execute<AType>("method{}: { var:= 500; var + 1 }", scope);

            AType result = this.engine.Execute<AType>("method{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(501), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestMethod]
        public void FunctionLocalAndGlobalScope()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("var := 10", scope);
            this.engine.Execute<AType>("method{}: { var:= 500; var + .var }", scope);

            AType result = this.engine.Execute<AType>("method{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(510), result, "Function call made incorrect calculation");
        }


        [TestCategory("DLR"), TestCategory("UserDefinedFunction"), TestCategory("Stack Refernce"), TestMethod]
        public void FunctionStackReference()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("$mode apl", scope);
            this.engine.Execute<AType>("a{c}: { if(c=0) { 1 } else { &{c - 1} + 1 } }", scope);

            AType result = this.engine.Execute<AType>("a{2}", scope);

            Assert.AreEqual<AType>(AInteger.Create(3), result, "Function call made incorrect calculation");
        }
    }
}
