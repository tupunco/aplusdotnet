using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic.InnerProduct
{
    [TestClass]
    public class IPMaxAdd : AbstractTest
    {
        #region Correct cases

        [TestCategory("DLR"), TestCategory("Inner Product"), TestCategory("IP Max Add"), TestMethod]
        public void Add2Matrix()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AArray.Create(ATypes.AFloat, AFloat.Create(3), AFloat.Create(4)),
                AArray.Create(ATypes.AFloat, AFloat.Create(5), AFloat.Create(6))
            );

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(
                ".a",
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
                )
            );

            AType result = this.engine.Execute<AType>("a max.+ a", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Inner Product"), TestCategory("IP Max Add"), TestMethod]
        public void Add2Arrays()
        {
            AType expected = this.engine.Execute<AType>("`float ? 2 3 4 rho 26 27 28 29 30 31 32 33 34 35 36 37 29 30 31 32 33 34 35 36 37 38 39 40");

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".y", this.engine.Execute<AType>("iota 2 3"));
            scope.SetVariable(".x", this.engine.Execute<AType>("iota 3 3 4"));

            AType result = this.engine.Execute<AType>("y max.+ x", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Inner Product"), TestCategory("IP Max Add"), TestMethod]
        public void IPMaxAddTest()
        {
            AType expected =
                AArray.Create(
                    ATypes.AFloat,
                    AArray.Create(
                        ATypes.AFloat,
                        AFloat.Create(10),
                        AFloat.Create(11),
                        AFloat.Create(12),
                        AFloat.Create(13)
                    ),
                    AArray.Create(
                        ATypes.AFloat,
                        AFloat.Create(13),
                        AFloat.Create(14),
                        AFloat.Create(15),
                        AFloat.Create(16)
                    )
                );

            AType result = this.engine.Execute<AType>("(iota 2 3) max.+ (iota 3 4)");
            Assert.AreEqual(expected.CompareInfos(result), InfoResult.OK);
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Error cases

        [TestCategory("DLR"), TestCategory("Inner Product"), TestCategory("IP Max Add"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RankErrorTest()
        {
            this.engine.Execute("1 max.+ 1");
        }

        [TestCategory("DLR"), TestCategory("Inner Product"), TestCategory("IP Max Add"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LengthErrorTest()
        {
            this.engine.Execute("(2 3 rho 1) max.+ 1 2 rho 1");
        }

        #endregion
    }
}
