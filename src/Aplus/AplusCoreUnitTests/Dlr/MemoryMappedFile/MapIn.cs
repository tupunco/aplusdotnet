using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.MemoryMappedFile
{
    [TestClass]
    public class MapIn : AbstractTest
    {
        #region Create and Remove MemoryMappedFiles

        [TestInitialize]
        public void InitMemoryMappedFile()
        {
            TestUtils.CreateMemoryMappedFiles(this.engine);
        }

        [TestCleanup]
        public void CleanUpMemoryMappedFile()
        {
            TestUtils.DeleteMemoryMappedFiles();
        }

        #endregion

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Monadic"), TestCategory("MapIn"), TestMethod]
        public void ReadChar()
        {
            AType expected = AChar.Create('A');

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := beam 'CharScalar.m'", scope);
            this.engine.Execute<AType>("a[] := 'b'", scope);

            AType result = this.engine.Execute<AType>("beam 'CharScalar.m'", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Monadic"), TestCategory("MapIn"), TestMethod]
        public void ReadIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(5),
                    AInteger.Create(6),
                    AInteger.Create(7)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(8),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := beam 'Integer23.m'", scope);
            this.engine.Execute<AType>("(1 take a) := 1 3 rho 9 2 4", scope);

            AType result = this.engine.Execute<AType>("beam 'Integer23.m'", scope);
            
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            scope.RemoveVariable(".a");
        }
    }
}
