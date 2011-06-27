using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.System
{
    [TestClass]
    public class Exit : AbstractTest
    {
        #region Error Cases

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Exit"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LengthErrorTest()
        {
            this.engine.Execute("_exit{2 2 rho 1}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Exit"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void FloatErrorTest()
        {
            this.engine.Execute("_exit{2 2 rho 1.0}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Exit"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorFloat()
        {
            this.engine.Execute("_exit{1.1}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Exit"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorOther()
        {
            this.engine.Execute("_exit{2 2 rho `a}");
        }

        #endregion
    }
}
