using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Type : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void IntegerType()
        {
            AType expected = ASymbol.Create("int");
            AType result = this.engine.Execute<AType>("?1");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void IntegerTypeUni()
        {
            AType expected = ASymbol.Create("int");
            AType result = this.engineUni.Execute<AType>("|1");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void FloatType()
        {
            AType expected = ASymbol.Create("float");
            AType result = this.engine.Execute<AType>("?1.1");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void SymbolType()
        {
            AType expected = ASymbol.Create("sym");
            AType result = this.engine.Execute<AType>("?`test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void BoxType()
        {
            AType expected = ASymbol.Create("box");
            AType result = this.engine.Execute<AType>("?(1;2)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void NullType()
        {
            AType expected = ASymbol.Create("null");
            AType result = this.engine.Execute<AType>("?()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void CharacterType()
        {
            AType expected = ASymbol.Create("char");
            AType result = this.engine.Execute<AType>("?'hello'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Type"), TestMethod]
        public void FunctionType()
        {
            AType expected = ASymbol.Create("func");

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("test{x}: x", scope);

            AType result = this.engine.Execute<AType>("?test", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

    }
}
