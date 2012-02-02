using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic.OuterProduct
{
    [TestClass]
    public class OPLessEqual : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Outer Product"), TestCategory("OP Less Equal"), TestMethod]
        public void LTE2Arrays()
        {
            AType expected = this.engine.Execute<AType>("3 4 rho 1 1 1 1 0 0 0 1 0 0 0 0");

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".y", this.engine.Execute<AType>("1 10 100"));
            scope.SetVariable(".x", this.engine.Execute<AType>("1 2 5 10"));

            AType result = this.engine.Execute<AType>("y <=. x", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
