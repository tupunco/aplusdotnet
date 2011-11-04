using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Runtime;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class InvokeFunction : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestMethod]
        [ExpectedException(typeof(Error.NonFunction))]
        public void IncorrectTargetCall()
        {
            this.engine.Execute<AType>(" a:=6; a{1}");
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestMethod]
        public void FunctionCallInsideFunction()
        {
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("f{x}: { x }", scope);
            this.engine.Execute<AType>("g{}: { h := f; h{3} }", scope);

            this.engine.Execute<AType>("g{}", scope);
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestMethod]
        public void EmptyFunction()
        {
            AType expected = Utils.ANull();
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("f{}: { }", scope);
            AType result = this.engine.Execute<AType>("f{}", scope);

            Assert.AreEqual<AType>(expected, result);
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestMethod]
        public void FunctionCallInsideEval()
        {
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("f{x}: { x }", scope);
            this.engine.Execute<AType>("g{}: { h := f; eval 'h{3}' }", scope);

            var result = this.engine.Execute<AType>("g{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(3), result);
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestCategory("Infix"), TestMethod]
        public void InfixDyadicFunction()
        {
            AType expected = AInteger.Create(5);
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("f{x;y}: { x + y }", scope);
            scope.SetVariable(".a", AInteger.Create(1));
            scope.SetVariable(".b", AInteger.Create(2)); 
            AType result = this.engine.Execute<AType>("2 f a f b", scope);

            Assert.AreEqual<AType>(expected, result);
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestCategory("Infix"), TestMethod]
        public void InfixFunctionInsideFuncion()
        {
            AType expected = AInteger.Create(5);
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("g{x}: { x + 1 }", scope);
            this.engine.Execute<AType>("f{x}: { g x }", scope);
            AType result = this.engine.Execute<AType>("f 4", scope);

            Assert.AreEqual<AType>(expected, result);
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestCategory("Infix"), TestMethod]
        public void InfixFunctionEvalInsideFuncion()
        {
            AType expected = AInteger.Create(5);
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("g{x}: -x", scope);
            this.engine.Execute<AType>("f{x}: { eval 'g{h}: h+1'; g x }", scope);
            AType result = this.engine.Execute<AType>("f 4", scope);

            Assert.AreEqual<AType>(expected, result);
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestCategory("Infix"), TestMethod]
        [ExpectedException(typeof(AplusCore.Compiler.ParseException))]
        public void InfixInvisibleFunctionEvalInsideFuncion()
        {
            this.engine.Execute<AType>("f{x}: { eval 'g{h}: h+1'; g x }");
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestCategory("Infix"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void InfixValenceErrorDyadicFunction()
        {
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("f{x;y}: { x + y }", scope);
            this.engine.Execute<AType>("f f 2", scope);
        }

        [TestCategory("DLR"), TestCategory("InvokeFunction"), TestCategory("Infix"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void InfixValenceErrorMonadicFunction()
        {
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("f{x}: { x + x }", scope);
            this.engine.Execute<AType>("1 f 2", scope);
        }
    }
}
