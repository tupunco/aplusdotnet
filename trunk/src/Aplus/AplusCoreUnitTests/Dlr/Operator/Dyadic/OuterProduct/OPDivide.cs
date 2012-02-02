using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic.OuterProduct
{
    [TestClass]
    public class OPDivide : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Outer Product"), TestCategory("OP Divide"), TestMethod]
        public void Divide2Arrays()
        {
            AType expected = this.engine.Execute<AType>("3 4 rho 1 0.5 0.2 0.1 10 5 2 1 100 50 20 10");

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".y", this.engine.Execute<AType>("1 10 100"));
            scope.SetVariable(".x", this.engine.Execute<AType>("1 2 5 10"));

            AType result = this.engine.Execute<AType>("y %. x", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Outer Product"), TestCategory("OP Divide"), TestMethod]
        public void NullTypeTest()
        {
            AType expected = this.engine.Execute<AType>("0 3 rho 1.1");
            AType result = this.engine.Execute<AType>("() %. 1 2 3 ");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }
    }
}
