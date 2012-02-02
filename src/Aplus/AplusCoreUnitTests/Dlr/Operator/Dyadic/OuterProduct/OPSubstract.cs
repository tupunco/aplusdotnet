using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic.OuterProduct
{
    [TestClass]
    public class OPSubstract : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Outer Product"), TestCategory("OP Substract"), TestMethod]
        public void Substract2Arrays()
        {
            AType expected = this.engine.Execute<AType>("3 4 rho 0 -1 -4 -9 9 8 5 0 99 98 95 90");

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".y", this.engine.Execute<AType>("1 10 100"));
            scope.SetVariable(".x", this.engine.Execute<AType>("1 2 5 10"));

            AType result = this.engine.Execute<AType>("y -. x", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
