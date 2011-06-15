using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.System
{
    [TestClass]
    public class IsaSlotfiller : AbstractTest
    {
        #region CorrectCases

        [TestCategory("DLR"), TestCategory("System"), TestCategory("IsaSlotfiller"), TestMethod]
        public void RightCase()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("_issf{(`a;<5)}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("IsaSlotfiller"), TestMethod]
        public void WrongCase()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("_issf{(`a;5)}");

            Assert.AreEqual(expected, result);
        }

        #endregion

        #region ErrorCases

        [TestCategory("DLR"), TestCategory("System"), TestCategory("IsaSlotfiller"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void ValenceErrorCase()
        {
            AType result = this.engine.Execute<AType>("_issf{`a;5}");
        }

        #endregion
    }
}
