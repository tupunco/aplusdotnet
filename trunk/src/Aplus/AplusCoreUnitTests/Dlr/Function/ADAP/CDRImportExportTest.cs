using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.ADAP
{
    [TestClass]
    [DeploymentItem("DLR")]
    public class CDRImportExportTest : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void IntegerTest()
        {
            byte[] expected = TestUtils.FileToByteArray("number.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void IntegervectorTest()
        {
            byte[] expected = TestUtils.FileToByteArray("numbervector.dat");
            
            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void IntegermatrixTest()
        {
            byte[] expected = TestUtils.FileToByteArray("numbermatrix.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void NullTest()
        {
            byte[] expected = TestUtils.FileToByteArray("null.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void NullvectorTest()
        {
            byte[] expected = TestUtils.FileToByteArray("nullvector.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void NullmatrixTest()
        {
            byte[] expected = TestUtils.FileToByteArray("nullmatrix.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void FloatTest()
        {
            byte[] expected = TestUtils.FileToByteArray("float.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void FloatvectorTest()
        {
            byte[] expected = TestUtils.FileToByteArray("floatvector.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void FloatMatrixTest()
        {
            byte[] expected = TestUtils.FileToByteArray("floatmatrix.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void SymbolTest()
        {
            byte[] expected = TestUtils.FileToByteArray("symbol.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void SymbolvectorTest()
        {
            byte[] expected = TestUtils.FileToByteArray("symbolvector.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void SymbolmatrixTest()
        {
            byte[] expected = TestUtils.FileToByteArray("symbolmatrix.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void StringTest()
        {
            byte[] expected = TestUtils.FileToByteArray("string.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void StringmatrixTest()
        {
            byte[] expected = TestUtils.FileToByteArray("stringmatrix.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void BoxTest()
        {
            byte[] expected = TestUtils.FileToByteArray("box.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void BoxvectorTest()
        {
            byte[] expected = TestUtils.FileToByteArray("boxvector.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Import-Export"), TestMethod]
        public void BoxmatrixTest()
        {
            byte[] expected = TestUtils.FileToByteArray("boxmatrix.dat");

            AType item = SysImp.Instance.Import(expected);
            byte[] result = SysExp.Instance.Format(item);

            Assert.IsTrue(expected.SequenceEqual(result));
        }
    }
}
