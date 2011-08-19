using System;
using System.IO;
using System.Text;

using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class SystemCommand : AbstractTest
    {
        #region Constants

        private const string TEST_FILE = "testFile.a+";

        #endregion

        #region Test setup and cleanup

        [TestInitialize]
        public void SystemCommandSetup()
        {
            using (StreamWriter fileStream = new StreamWriter(TEST_FILE))
            {
                fileStream.WriteLine("$cx testContext");
                fileStream.WriteLine("a := 0");
            }
        }

        [TestCleanup]
        public void SystemCommandCleanUp()
        {
            File.Delete(TEST_FILE);
        }

        #endregion

        #region $load

        [TestCategory("DLR"), TestCategory("System Command"), TestMethod]
        public void ContextAfterLoadTextRootContext()
        {
            this.engine.Execute("$load " + TEST_FILE);

            Aplus runtime = this.engine.GetService<Aplus>();

            Assert.AreEqual(".", runtime.CurrentContext);
        }
        
        [TestCategory("DLR"), TestCategory("System Command"), TestMethod]
        public void ContextAfterLoadTextNotRootContext()
        {
            this.engine.Execute("$cx startingContext");
            this.engine.Execute("$load " + TEST_FILE);

            Aplus runtime = this.engine.GetService<Aplus>();

            Assert.AreEqual("startingContext", runtime.CurrentContext);
        }

        #endregion

        #region $cx

        [TestCategory("DLR"), TestCategory("System Command"), TestMethod]
        public void ContextTest()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a:=5\n$cx X\na:=6", scope);

            Assert.IsTrue(scope.ContainsVariable(".a"), "Variable not found");
            Assert.AreEqual(AInteger.Create(5), scope.GetVariable<AType>(".a"));

            Assert.IsTrue(scope.ContainsVariable("X.a"), "Variable not found");
            Assert.AreEqual(AInteger.Create(6), scope.GetVariable<AType>("X.a"));
        }

        #endregion
    }
}
