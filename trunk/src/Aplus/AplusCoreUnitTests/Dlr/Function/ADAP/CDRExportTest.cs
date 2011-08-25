using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.ADAP
{
    [TestClass]
    [DeploymentItem("DLR")]
    public class CDRExportTest : AbstractTest
    {
        #region Normal cases

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void StrandTest()
        {
            AType item = this.engine.Execute<AType>("(`eval;(1;2);'+')");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("strand.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }
        
        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void IntegerTest()
        {
            AType item = this.engine.Execute<AType>("1");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("number.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void IntegervectorTest()
        {
            AType item = this.engine.Execute<AType>("5 rho 1");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("numbervector.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void IntegermatrixTest()
        {
            AType item = this.engine.Execute<AType>("5 5 5 rho 1");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("numbermatrix.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void NullTest()
        {
            AType item = this.engine.Execute<AType>("()");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("null.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void NullvectorTest()
        {
            AType item = this.engine.Execute<AType>("5 rho ()");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("nullvector.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void NullmatrixTest()
        {
            AType item = this.engine.Execute<AType>("5 5 5 rho ()");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("nullmatrix.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void FloatTest()
        {
            AType item = this.engine.Execute<AType>("1.1");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("float.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void FloatvectorTest()
        {
            AType item = this.engine.Execute<AType>("5 rho 1.1");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("floatvector.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void FloatMatrixTest()
        {
            AType item = this.engine.Execute<AType>("5 5 5 rho 1.1");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("floatmatrix.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void SymbolTest()
        {
            AType item = this.engine.Execute<AType>("`symbol");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("symbol.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void SymbolvectorTest()
        {
            AType item = this.engine.Execute<AType>("5 rho `symbol");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("symbolvector.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void SymbolmatrixTest()
        {
            AType item = this.engine.Execute<AType>("5 5 5 rho `symbol");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("symbolmatrix.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void StringTest()
        {
            AType item = this.engine.Execute<AType>("'string'");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("string.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void StringmatrixTest()
        {
            AType item = this.engine.Execute<AType>("5 5 5 rho 'string'");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("stringmatrix.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void BoxTest()
        {
            AType item = this.engine.Execute<AType>("<3");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("box.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void BoxvectorTest()
        {
            AType item = this.engine.Execute<AType>("(1;2;3;4)");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("boxvector.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        public void BoxmatrixTest()
        {
            AType item = this.engine.Execute<AType>("5 5 rho <1");

            byte[] result = SysExp.Instance.Format(item);
            byte[] expected = TestUtils.FileToByteArray("boxmatrix.dat");

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        #endregion

        #region Error cases

        [TestCategory("DLR"), TestCategory("ADAP"), TestCategory("Export"), TestMethod]
        [ExpectedException(typeof(Error.Nonce))]
        public void NonceErrorCase()
        {
            AType item = this.engine.Execute<AType>("+");

            SysExp.Instance.Format(item);
        }

        #endregion
    }
}
