using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.ADAP
{
    [TestClass]
    public class CDRExportImportTest : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void StrandTest()
        {
            AType expected = this.engine.Execute<AType>("(`eval;(1;2);'+')");
            
            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void IntegerTest()
        {
            AType expected = this.engine.Execute<AType>("1");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void IntegervectorTest()
        {
            AType expected = this.engine.Execute<AType>("5 rho 1");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void IntegermatrixTest()
        {
            AType expected = this.engine.Execute<AType>("5 5 5 rho 1");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void NullTest()
        {
            AType expected = this.engine.Execute<AType>("()");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void NullvectorTest()
        {
            AType expected = this.engine.Execute<AType>("5 rho ()");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void NullmatrixTest()
        {
            AType expected = this.engine.Execute<AType>("5 5 5 rho ()");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void FloatTest()
        {
            AType expected = this.engine.Execute<AType>("1.1");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void FloatvectorTest()
        {
            AType expected = this.engine.Execute<AType>("5 rho 1.1");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void FloatMatrixTest()
        {
            AType expected = this.engine.Execute<AType>("5 5 5 rho 1.1");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void SymbolTest()
        {
            AType expected = this.engine.Execute<AType>("`symbol");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void SymbolvectorTest()
        {
            AType expected = this.engine.Execute<AType>("5 rho `symbol");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void SymbolmatrixTest()
        {
            AType expected = this.engine.Execute<AType>("5 5 5 rho `symbol");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void StringTest()
        {
            AType expected = this.engine.Execute<AType>("'string'");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void StringmatrixTest()
        {
            AType expected = this.engine.Execute<AType>("5 5 5 rho 'string'");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void BoxTest()
        {
            AType expected = this.engine.Execute<AType>("<3");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void BoxvectorTest()
        {
            AType expected = this.engine.Execute<AType>("(1;2;3;4)");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export-Import"), TestMethod]
        public void BoxmatrixTest()
        {
            AType expected = this.engine.Execute<AType>("5 5 rho <1");

            byte[] item = SysExp.Instance.Format(expected);
            AType result = SysImp.Instance.Import(item);

            Assert.AreEqual(expected, result);
        }
    }
}
