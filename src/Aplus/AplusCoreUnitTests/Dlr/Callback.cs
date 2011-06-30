using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class Callback : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void BasicCallbackDefinition()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; (f;'fuuu')}", scope);

            Aplus runtime = this.engine.GetService<Aplus>();

            Assert.IsTrue(runtime.CallbackManager.Contains(".a"), "Callback not found.");
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void GetCallbackDefinition()
        {
            AType expected = Helpers.BuildStrand(
                new AType[] { Helpers.BuildString("fuuu"), Helpers.BuildString(".f") }
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; (f;'fuuu')}", scope);

            AType result = this.engine.Execute<AType>("_gcb{`a}");

            Assert.AreEqual<AType>(expected, result, "Incorrect callback info returned.");
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void CallbackNotFound()
        {
            AType expected = Utils.ANull();
            AType result = this.engine.Execute<AType>("_gcb{`a}");

            Assert.AreEqual<AType>(expected, result);
        }

        #region Errors

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void GlobalNameCallbackError()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{'woo'; (f;'fuuu')}", scope);
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void IncorrectTypeCallbackError()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; 'ab'}", scope);
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        [ExpectedException(typeof(Error.NonData))]
        public void IncorrectArgumentCallbackError()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; f}", scope);
        }

        #endregion
    }
}
