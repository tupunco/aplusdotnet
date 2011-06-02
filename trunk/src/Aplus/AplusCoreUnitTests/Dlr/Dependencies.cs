using System;

using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore;
using AplusCore.Runtime;
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
        public void ItemwiseDependencyDefinition()
        {
            var scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a[i]: b[i] + 2", scope);
        }

        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void BasicDependencyEvaluation()
        {
            var expected = AInteger.Create(5);
            var scope = this.engine.CreateScope();

            Aplus runtime = this.engine.GetService<Aplus>();

            this.engine.Execute<AType>("a: b + 2", scope);
            this.engine.Execute<AType>("b:=3", scope);

            Assert.IsTrue(runtime.DependencyManager.IsInvalid(".a"), "Dependency '.a' marked valid");

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".a"), "Dependency '.a' marked invalid");
        }

        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void ItemwiseDependencyEvaluation()
        {
            var expected = new int[] { 3, 4, 5 }.ToAArray();
            var scope = this.engine.CreateScope();

            Aplus runtime = this.engine.GetService<Aplus>();

            this.engine.Execute<AType>("a[i]: b[i] + 2", scope);
            this.engine.Execute<AType>("b:=1 2 3", scope);

            Assert.IsTrue(runtime.DependencyManager.IsInvalid(".a"), "Dependency '.a' marked valid");

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".a"), "Dependency '.a' marked invalid");
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

            Aplus runtime = this.engine.GetService<Aplus>();
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".a"), "Dependency '.a' marked invalid");
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".b"), "Dependency '.b' marked invalid");
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".df"), "Dependency '.bf' marked invalid");
        }


        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void CyclicDependencyEvaluation()
        {
            var expected = AFloat.Create(0.085);
            var scope = this.engine.CreateScope();

            this.engine.Execute<AType>("y: u + s", scope);
            this.engine.Execute<AType>("u: y - s", scope);
            this.engine.Execute<AType>("s: y - u", scope);
            this.engine.Execute<AType>("(u;s):= (0.08; 0.005)", scope);

            AType result = this.engine.Execute<AType>("y", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");

            Aplus runtime = this.engine.GetService<Aplus>();
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".y"), "Dependency '.y' marked invalid");
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".u"), "Dependency '.u' marked invalid");
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".s"), "Dependency '.s' marked invalid");
        }

        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void DependencyDependantModificationEvaluation()
        {
            AType expected = AFloat.Create(111);
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("m: { m:=m+n; (n):=10*n; m+n }", scope);
            this.engine.Execute<AType>("m:=100", scope);
            this.engine.Execute<AType>("n:=1", scope);

            AType result = this.engine.Execute<AType>("m", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");
        }

        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void DependencyDependantEvalModificationEvaluation()
        {
            AType expected = AFloat.Create(2);
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("m: { m:=m+n; eval 'm:n'; m+n }", scope);
            this.engine.Execute<AType>("m:=100", scope);
            this.engine.Execute<AType>("n:=1", scope);

            AType result = this.engine.Execute<AType>("m", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");

            Aplus runtime = this.engine.GetService<Aplus>();
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".m"), "Dependency marked invalid");
        }

        [TestCategory("DLR"), TestCategory("Dependencies"), TestMethod]
        public void DependencyDependantCheck()
        {
            AType expected = AFloat.Create(40);
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("m: { a:=20; a + a }", scope);
            AType result = this.engine.Execute<AType>("m", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect dependency evaluation");

            this.engine.Execute<AType>("a := -100", scope);

            Aplus runtime = this.engine.GetService<Aplus>();
            Assert.IsFalse(runtime.DependencyManager.IsInvalid(".m"), "Dependency marked invalid");
        }
    }
}
