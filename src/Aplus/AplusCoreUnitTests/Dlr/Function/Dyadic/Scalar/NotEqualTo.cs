using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.Scalar
{
    [TestClass]
    public class NotEqualTo : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToInt2Float()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("1 ~= 1.2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToFloat2Int()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("1.1 ~= 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToCharachterList2CharacterList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("' ' ~= 'this is it'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToBox2Box()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("(1; 3) ~= (1; 'test')");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToIntList2CharachterList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("1 2 3 ~= '123'");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToFloat2FloatList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(0),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("2.3 ~= 3.1 42.4 2.3000000000001 8.7");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToSymbolConstant2SymbolConstant()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("`abcd ~= `abcd");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToSymbolConstant2SymbolConstant2()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("`abcd ~= `abc");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToSymbolConstant2IntList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(1)
            );
            AType result = this.engine.Execute<AType>("`aba ~= 4 2 2 4");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToIntNull2Int()
        {
            AType result = this.engine.Execute<AType>("(`int?()) ~= 1");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToNull2Null()
        {
            AType result = this.engine.Execute<AType>("() ~= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToIntNull2IntNull()
        {
            AType result = this.engine.Execute<AType>("(`int?()) ~= (`int?())");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToIntNull2Null()
        {
            AType result = this.engine.Execute<AType>("(`int?()) ~= ()");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Not Equal to"), TestMethod]
        public void NotEqualToNull2IntNull()
        {
            AType result = this.engine.Execute<AType>("() ~= (`int?())");

            Assert.AreEqual<ATypes>(ATypes.AInteger, result.Type, "Incorrect type");
        }
    }
}
