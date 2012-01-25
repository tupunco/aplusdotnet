using System;

using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.MemoryMappedFile
{
    [TestClass]
    public class MapIn : AbstractTest
    {
        #region Create and Remove MemoryMappedFiles

        [TestInitialize]
        public void InitMemoryMappedFile()
        {
            MappedUtils.CreateMemoryMappedFiles(this.engine);
        }

        [TestCleanup]
        public void CleanUpMemoryMappedFile()
        {
            MappedUtils.DeleteMemoryMappedFiles(ref this.engine);
        }

        #endregion

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Monadic"), TestCategory("MapIn"), TestMethod]
        public void ReadChar()
        {
            AType expected = AChar.Create('A');

            ScriptScope scope = this.engine.CreateScope();
            string executable = string.Format("a := beam `{0}", MappedFiles.CharScalar.GetFileName());

            this.engine.Execute<AType>(executable, scope);
            this.engine.Execute<AType>("a[] := 'b'", scope);

            executable = string.Format("beam '{0}'", MappedFiles.CharScalar.GetFileName());
            AType result = this.engine.Execute<AType>(executable, scope);

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
            string executable = string.Format("a := beam '{0}'", MappedFiles.IntegerMatrix.GetFileName());
            this.engine.Execute<AType>(executable, scope);
            this.engine.Execute<AType>("(1 take a) := 1 3 rho 9 2 4", scope);

            executable = string.Format("beam '{0}'", MappedFiles.IntegerMatrix.GetFileName());
            AType result = this.engine.Execute<AType>(executable, scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }
    }
}
