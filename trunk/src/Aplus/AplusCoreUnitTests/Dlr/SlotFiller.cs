using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class SlotFiller : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFiller1()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("_issf{(`sym;<4)}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFiller2()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("_issf{(`small`medium`large`super;(16;32;64;72))}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFiller3()
        {
            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("_issf{(`a`b`c;(10;(`x`y;(100;200));(`e`f;((`g`h;(1000;2000));'A+'))))}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFiller4()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scriptscope);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("_issf{(`a;<{a})}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFiller5()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scriptscope);
            this.engine.Execute<AType>("b{a}: a*a", scriptscope);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("_issf{(`a`b;(a;b))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFiller6()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scriptscope);
            this.engine.Execute<AType>("b{a}: a*a", scriptscope);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("_issf{(`a`b`c;(a;b;<{+}))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerSimpleArray()
        {
            AType expected = AInteger.Create(0);
            // input is not an array of boxes
            AType result = this.engine.Execute<AType>("_issf{`small`medium`large`super}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerDifferentLength1()
        {
            AType expected = AInteger.Create(0);
            // mismatch in the length of the keys and values
            AType result = this.engine.Execute<AType>("_issf{(`small;(16;32;64;72))}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerDifferentLength2()
        {
            AType expected = AInteger.Create(0);
            // mismatch in the length of the keys and values
            AType result = this.engine.Execute<AType>("_issf{(`small`medium`large`super;(64;72))}");

            Assert.AreEqual(expected, result);
        }

        //The keys are not symbol consant.
        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerKeysNotSymbolConstant()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("_issf{(2 3 1 4;(16;32;64;72))}");

            Assert.AreEqual(expected, result);
        }
        
        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerKeysMatrix()
        {
            AType expected = AInteger.Create(0);
            // the rank of the keys is greater then 1            
            AType result = this.engine.Execute<AType>("_issf{(4 1 rho `small`medium`large`super;(16;32;64;72))}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerSameKeys()
        {
            AType expected = AInteger.Create(0);
            // duplicate elements inside the keys vector
            AType result = this.engine.Execute<AType>("_issf{(`small`medium`large`super`large;(16;32;64;72;64))}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerValueIntegerList()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("_issf{(`x`y`z; 32 64 72)}");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerValueUserDefinedAndPrimitiveFunctions()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scriptscope);
            this.engine.Execute<AType>("b{a}: a*a", scriptscope);

            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("_issf{(`a`c`b;(a;+;b))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void SlotFillerFunctionScalarKey()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("_issf{(`a ,<{+};(3;5))}");

            Assert.AreEqual(expected, result);
        }
    }
}
