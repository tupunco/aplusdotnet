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
    public class Depth : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthScalar()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("== 'a'");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthScalarUni()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engineUni.Execute<AType>("== 'a'");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthSimpleVector()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("== 'test'");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthNull()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("== ()");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthNestedNull()
        {
            AType expected = AInteger.Create(2);

            AType result = this.engine.Execute<AType>("== << ()");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthNestedVector()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("== (2;3;<(3;7))");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthUserDefinedFunction()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);

            AType expected = AInteger.Create(-1);

            AType result = this.engine.Execute<AType>("== a", scope);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthFunctionScalar1()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);
            this.engine.Execute<AType>("f := <{a}", scope);

            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("== f", scope);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthPrimitiveFunction()
        {
            AType expected = AInteger.Create(-1);

            AType result = this.engine.Execute<AType>("=={+}");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthFunctionScalar2()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("== f", scope);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthNestedFunctionScalar()
        {

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);

            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("== < < a", scope);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthMixedSimpleArray1()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("=={`a`b , <{+}}");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthMixedSimpleArray2()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("=={(<{+}) , `a`b}");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Depth"), TestMethod]
        public void DepthMixedNestedArray()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("=={(<{+}) , < 4}");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
