using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic.OuterProduct
{
    [TestClass]
    public class OPPower : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Outer Product"), TestCategory("OP Power"), TestMethod]
        public void Power2Arrays()
        {
            AType expected = this.engine.Execute<AType>("3 4 rho 1 1 1 1 10 100 1000 10000 100 10000 1000000 100000000");
            expected.ConvertToFloat(); // because of whole numbers

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".y", this.engine.Execute<AType>("1 10 100"));
            scope.SetVariable(".x", this.engine.Execute<AType>("1 2 3 4"));

            AType result = this.engine.Execute<AType>("y ^. x", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
