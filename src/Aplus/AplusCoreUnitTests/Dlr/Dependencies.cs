using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class Dependencies : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void BasicDependencyDefinition()
        {
            var scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a: b + 2", scope);
        }

        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void BasicDependencyEvaluation()
        {
            var expected = AInteger.Create(5);
            var scope = this.engine.CreateScope();

            this.engine.Execute<AType>("a: b + 2", scope);
            this.engine.Execute<AType>("b:=3", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");
        }

        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void ComplexDependencyEvaluation()
        {
            var expected = AInteger.Create((int)(100 + Math.Pow(100, 2) + 3 + 2000));
            var scope = this.engine.CreateScope();

            this.engine.Execute<AType>("a:=100", scope);
            this.engine.Execute<AType>("b: a ^ 2", scope);
            this.engine.Execute<AType>("f{x}: 3 + x", scope);
            this.engine.Execute<AType>("df: a + b + f{2000}", scope);

            AType result = this.engine.Execute<AType>("df", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");
        }
    }
}
