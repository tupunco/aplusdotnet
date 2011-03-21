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
        public void FunctionCallInsiedFunction()
        {
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("f{x}: { x }", scope);
            this.engine.Execute<AType>("g{}: { h := f; eval 'h{3}' }", scope);

            var result = this.engine.Execute<AType>("g{}", scope);

            Assert.AreEqual<AType>(AInteger.Create(3), result);
        }
    }
}
