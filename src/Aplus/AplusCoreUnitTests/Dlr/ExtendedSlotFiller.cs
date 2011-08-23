using System;

using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class ExtendedSlotFiller : AbstractTest
    {
        AType issf = AFunc.Create(
            "issf",
            (Func<Aplus, AType, AType>)((scope, x) =>
            {
                if (x.IsArray)
                {
                    return AInteger.Create(x.IsSlotFiller(true) ? 1 : 0);
                }
                return AInteger.Create(0);
            }),
            2,
            "checks if x is a extended slotfiller"
        );

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void ExtendedSlotFiller1()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("issf", issf);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("issf{(`this`was`this`that;(10;20;30;40))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void ExtendedSlotFiller2()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("issf", issf);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("issf{(2 2 rho `a`b`c`d;(10;20;30;40))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void ExtendedSlotFiller3()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("issf", issf);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("issf{(`a`b;1 2 rho (4;2))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void ExtendedSlotFiller4()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("issf", issf);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("issf{(2 2 rho `a`b`c`d;2 2 rho (6;7;2;9))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void ExtendedSlotFiller5()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("issf", issf);
            this.engine.Execute<AType>("a{b}: b+b", scriptscope);
            this.engine.Execute<AType>("b{a}: a*a", scriptscope);

            AType expected = AInteger.Create(1);
            AType result = this.engine.Execute<AType>("issf{(`a`c`b;(a;+;b))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        //The keys and values lenght different.
        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void ExtendedSlotFillerDifferentLength1()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("issf", issf);

            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("issf{(2 3 rho `a`b`c`d;(10;20;30;40))}", scriptscope);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("SystemFunction"), TestCategory("SlotFiller"), TestMethod]
        public void ExtendedSlotFillerDifferentLength2()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("issf", issf);

            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("issf{(2 2 rho `a`b`c`d; 2 3 rho (3;4))}", scriptscope);

            Assert.AreEqual(expected, result);
        }
    }
}
