using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.System
{
    [TestClass]
    public class RemoveDependency : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("System"), TestCategory("Remove dependency"), TestMethod]
        public void UndefDependency()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute("a: b + b", scope);

            AType result = this.engine.Execute<AType>("_undef{`a}", scope);

            Assert.AreEqual<AType>(AInteger.Create(0), result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Remove dependency"), TestMethod]
        public void MulitpleUndefDependency()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute("a: b + b", scope);

            AType firstUndef = this.engine.Execute<AType>("_undef{`a}", scope);
            Assert.AreEqual<AType>(AInteger.Create(0), firstUndef, "First undef failed");

            AType secondUndef = this.engine.Execute<AType>("_undef{`a}", scope);
            Assert.AreEqual<AType>(AInteger.Create(1), secondUndef, "Second undef failed");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Remove dependency"), TestMethod]
        public void InvalidUndefDependency()
        {
            AType result = this.engine.Execute<AType>("_undef{`a}");

            Assert.AreEqual<AType>(AInteger.Create(1), result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Remove dependency"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ErrorUndefDependency()
        {
            this.engine.Execute<AType>("_undef{'woo'}");
        }
    }
}
