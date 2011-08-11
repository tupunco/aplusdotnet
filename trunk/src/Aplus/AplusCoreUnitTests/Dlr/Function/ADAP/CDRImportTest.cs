using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.ADAP
{
    [TestClass]
    [DeploymentItem("DLR")]
    public class CDRImportTest : AbstractTest
    {
        #region Normal cases

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void IntegerTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("number.dat"));
            AType expected = this.engine.Execute<AType>("1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(result, expected);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void IntegervectorTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("numbervector.dat"));
            AType expected = this.engine.Execute<AType>("5 rho 1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void IntegermatrixTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("numbermatrix.dat"));
            AType expected = this.engine.Execute<AType>("5 5 5 rho 1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void NullTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("null.dat"));
            AType expected = this.engine.Execute<AType>("()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected,result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void NullvectorTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("nullvector.dat"));
            AType expected = this.engine.Execute<AType>("5 rho ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void NullmatrixTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("nullmatrix.dat"));
            AType expected = this.engine.Execute<AType>("5 5 5 rho ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected,result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void FloatTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("float.dat"));
            AType expected = this.engine.Execute<AType>("1.1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void FloatvectorTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("floatvector.dat"));
            AType expected = this.engine.Execute<AType>("5 rho 1.1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void FloatMatrixTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("floatmatrix.dat"));
            AType expected = this.engine.Execute<AType>("5 5 5 rho 1.1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void SymbolTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("symbol.dat"));
            AType expected = this.engine.Execute<AType>("`symbol");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void SymbolvectorTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("symbolvector.dat"));
            AType expected = this.engine.Execute<AType>("5 rho `symbol");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void SymbolmatrixTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("symbolmatrix.dat"));
            AType expected = this.engine.Execute<AType>("5 5 5 rho `symbol");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void StringTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("string.dat"));
            AType expected = this.engine.Execute<AType>("'string'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void StringmatrixTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("stringmatrix.dat"));
            AType expected = this.engine.Execute<AType>("5 5 5 rho 'string'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void BoxTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("box.dat"));
            AType expected = this.engine.Execute<AType>("<3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void BoxvectorTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("boxvector.dat"));
            AType expected = this.engine.Execute<AType>("(1;2;3;4)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        public void BoxmatrixTest()
        {
            AType result = SysImp.Instance.Import(TestUtils.FileToByteArray("boxmatrix.dat"));
            AType expected = this.engine.Execute<AType>("5 5 rho <1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Error cases

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainErrorCase()
        {
            byte[] toImport = new byte[10];

            SysImp.Instance.Import(toImport);
        }

        #endregion
    }
}
