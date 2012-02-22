using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic.InnerProduct
{
    [TestClass]
    public class IPMinAdd : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Inner Product"), TestCategory("IP Min Add"), TestMethod]
        public void Add2Matrix()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(1)),
                AArray.Create(ATypes.AFloat, AFloat.Create(2), AFloat.Create(3))
            );

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(
                ".a",
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
                )
            );

            AType result = this.engine.Execute<AType>("a min.+ a", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Inner Product"), TestCategory("IP Min Add"), TestMethod]
        public void Add2Arrays()
        {
            AType expected = this.engine.Execute<AType>("`float ? 2 3 4 rho 0 1 2 3 4 5 6 7 8 9 10 11 3 4 5 6 7 8 9 10 11 12 13 14");

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".y", this.engine.Execute<AType>("iota 2 3"));
            scope.SetVariable(".x", this.engine.Execute<AType>("iota 3 3 4"));

            AType result = this.engine.Execute<AType>("y min.+ x", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestMethod, TestCategory("Inner Product"), TestCategory("IP Min Add")]
        public void IPMinAddTest()
        {
            AType expected =
                AArray.Create(
                    ATypes.AFloat,
                    AArray.Create(
                        ATypes.AFloat,
                        AFloat.Create(0),
                        AFloat.Create(1),
                        AFloat.Create(2),
                        AFloat.Create(3)
                    ),
                    AArray.Create(
                        ATypes.AFloat,
                        AFloat.Create(3),
                        AFloat.Create(4),
                        AFloat.Create(5),
                        AFloat.Create(6)
                    )
                );

            AType result = this.engine.Execute<AType>("(iota 2 3) min.+ (iota 3 4)");
            Assert.AreEqual(expected.CompareInfos(result), InfoResult.OK);
            Assert.AreEqual(expected, result);
        }
    }
}
