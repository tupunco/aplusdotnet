using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator
{
    [TestClass]
    public class Apply : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("General"), TestCategory("Apply"), TestMethod]
        public void GeneralApplyInvoke()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{a;b;c}: drop (a;b;c)", scope);
            this.engine.Execute<AType>("boxF:= <{f}", scope);

            AType expected = AArray.Create(ATypes.ABox,
                ABox.Create(AInteger.Create(1)),
                ABox.Create(AInteger.Create(2)),
                ABox.Create(AInteger.Create(3))
            );

            AType result = this.engine.Execute<AType>("(boxF) each {1;2;3}", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
