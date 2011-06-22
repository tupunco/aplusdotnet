using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.System
{
    [TestClass]
    public class PermissiveIndexing : AbstractTest
    {
        #region Correct Cases

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        public void CorrectCase1()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                                           AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)),
                                           AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1))
                                           );

            AType result = this.engine.Execute<AType>("_index{2 2 rho 1; 1 1; 1}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        public void ReshapeDefaultItemCase()
        {
            AType expected = 
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger,
                        AInteger.Create(40), AInteger.Create(41), AInteger.Create(42), AInteger.Create(43)),
                    AArray.Create(ATypes.AInteger,
                        AInteger.Create(-1), AInteger.Create(-1), AInteger.Create(-1), AInteger.Create(-1))
                );

            AType result = this.engine.Execute<AType>("_index{10 11;iota 11 4; -1}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        public void SimpleExample()
        {
            AType expected = AArray.Create(ATypes.ABox,
                                           ABox.Create(Helpers.BuildString("May 15")),
                                           ABox.Create(Helpers.BuildString("n.a.")),
                                           ABox.Create(Helpers.BuildString("n.a.")),
                                           ABox.Create(Helpers.BuildString("Jul 18"))
                                           );

            AType result = this.engine.Execute<AType>("_index{2 19 14 1;('Mar 06';'Jul 18';'May 15';'Nov 26');<'n.a.'}");
            
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        public void NullExample()
        {
            AType expected = AArray.Create(ATypes.ANull);

            AType result = this.engine.Execute<AType>("_index{();('Mar 06';'Jul 18';'May 15';'Nov 26');<'n.a.'}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        public void FloatIndex()
        {
            AType expected = AArray.Create(ATypes.ABox,
                                           ABox.Create(Helpers.BuildString("May 15")),
                                           ABox.Create(Helpers.BuildString("n.a.")),
                                           ABox.Create(Helpers.BuildString("n.a.")),
                                           ABox.Create(Helpers.BuildString("Jul 18"))
                                           );

            AType result = this.engine.Execute<AType>("_index{2.0 19.0 14.0 1.0;('Mar 06';'Jul 18';'May 15';'Nov 26');<'n.a.'}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        public void NegativeIndex()
        {
            AType expected = AArray.Create(ATypes.ABox,
                                           ABox.Create(Helpers.BuildString("May 15")),
                                           ABox.Create(Helpers.BuildString("n.a.")),
                                           ABox.Create(Helpers.BuildString("n.a.")),
                                           ABox.Create(Helpers.BuildString("Jul 18"))
                                           );

            AType result = this.engine.Execute<AType>("_index{2 -2 -14 1;('Mar 06';'Jul 18';'May 15';'Nov 26');<'n.a.'}");

            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Error Cases

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RankError()
        {
            this.engine.Execute<AType>("_index{1;1;1}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void WrongDefaultItemRankError()
        {
            this.engine.Execute<AType>("_index{1 2 3;2 3 3 rho 1;3 2 rho 1}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void WrongIndexTypeError()
        {
            this.engine.Execute<AType>("_index{`a `b `c;1;1}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void WrongDefaultItemTypeError()
        {
            this.engine.Execute<AType>("_index{1 2 3;`a;1}");
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("PermissiveIndexing"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void WrongIndexTypeErrorFloat()
        {
            this.engine.Execute<AType>("_index{1.1;1;1}");
        }

        #endregion
    }
}