using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.Hosting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator
{
    [TestClass]
    public class UserDefinedOperator : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), Ignore, TestMethod]
        public void OperatorCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(f op)b: 5", scope);

            AType result = this.engine.Execute<AType>("+ op 2", scope);

            Assert.AreEqual<AType>(AInteger.Create(5), result, "Function call made incorrect calculation");
        }
    }
}
