using System;
using System.Collections.Generic;

using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.System
{
    [TestClass]
    public class Items : AbstractTest
    {
        #region Create and Remove Memory-mapped files

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

        #region Expand

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void EnlargeSizeOfMemoryMappedIntegerArray()
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
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(6),
                    AInteger.Create(2),
                    AInteger.Create(5)
                )
            );

            ScriptScope scope = this.engine.CreateScope();

            string mappedName = MappedFiles.IntegerMatrix.GetFileNameWithoutExtension();

            this.engine.Execute<AType>(string.Format("_items{{3;'{0}'}}", mappedName));
            this.engine.Execute<AType>(string.Format("t := 1 beam `{0}", mappedName), scope);
            this.engine.Execute<AType>("t[,] := 6 2 5", scope);

            AType result = scope.GetVariable<AType>(".t");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void EnlargeSizeOfMemoryMappedFloatArray()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(3.4),
                    AFloat.Create(1.4)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(7.6),
                    AFloat.Create(1.1)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(0),
                    AFloat.Create(1)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(2),
                    AFloat.Create(3)
                )
            );

            ScriptScope scope = this.engine.CreateScope();

            string filename = MappedFiles.FloatMatrix.GetFileNameWithoutExtension();

            this.engine.Execute<AType>(string.Format("_items{{4;'{0}'}}", filename));
            this.engine.Execute<AType>(string.Format("t := 1 beam `{0}", filename), scope);
            this.engine.Execute<AType>("t[,] := iota 2 2", scope);

            AType result = scope.GetVariable<AType>(".t");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        #endregion

        #region Truncate

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void TruncateSizeofMemoryMappedCharArray()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("Hello")
            );

            ScriptScope scope = this.engine.CreateScope();

            string filename = MappedFiles.CharMatrix.GetFileName();
            this.engine.Execute<AType>(string.Format("_items{{1;`{0}}}", filename));
            AType result = this.engine.Execute<AType>(string.Format("t := 1 beam `{0}", filename), scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void TruncateSizeofMemoryMappedFloatArray()
        {
            AType expected = AArray.Create(ATypes.AFloat);
            expected.Shape = new List<int>() { 0, 2 };
            expected.Rank = 2;

            ScriptScope scope = this.engine.CreateScope();

            string filename = MappedFiles.FloatMatrix.GetFileName();
            this.engine.Execute<AType>(string.Format("_items{{0;`{0}}}", filename));
            AType result = this.engine.Execute<AType>(string.Format("t := 1 beam `{0}", filename), scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        #endregion

        #region Information of Memory-mapped file size

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void SizeOfMemoryMappedIntegerArray()
        {
            AType expected = AInteger.Create(2);

            string filename = MappedFiles.IntegerMatrix.GetFileNameWithoutExtension();
            AType result = this.engine.Execute<AType>(string.Format("_items{{-1;'{0}'}}", filename));

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void SizeOfMemoryMappedChargerArray()
        {
            AType expected = AInteger.Create(15);

            string filename = MappedFiles.CharMatrix.GetFileNameWithoutExtension();
            this.engine.Execute<AType>(string.Format("_items{{15;'{0}'}}", filename));
            AType result = this.engine.Execute<AType>(string.Format("_items{{-1;'{0}'}}", filename));

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        #endregion

        #region Errors

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("System"), TestCategory("Items"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ItemsLengthError()
        {
            ScriptScope scope = this.engine.CreateScope();
            string filename = MappedFiles.IntegerMatrix.GetFileNameWithoutExtension();
            this.engine.Execute<AType>(string.Format("t := 1 beam `{0}", filename), scope);
            this.engine.Execute<AType>("t[,] := 2 4 1", scope);
        }

        #endregion
    }
}
