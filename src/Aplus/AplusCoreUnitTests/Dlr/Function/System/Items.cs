using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            TestUtils.CreateMemoryMappedFiles(this.engine);
        }

        [TestCleanup]
        public void CleanUpMemoryMappedFile()
        {
            TestUtils.DeleteMemoryMappedFiles();
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

            this.engine.Execute<AType>("_items{3;'Integer23'}");
            this.engine.Execute<AType>("t := 1 beam `Integer23", scope);
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

            this.engine.Execute<AType>("_items{4;'Float22'}");
            this.engine.Execute<AType>("t := 1 beam `Float22", scope);
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

            this.engine.Execute<AType>("_items{1;`Char25.m}");
            AType result = this.engine.Execute<AType>("t := 1 beam `Char25.m", scope);

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

            this.engine.Execute<AType>("_items{0;`Float22.m}");
            AType result = this.engine.Execute<AType>("t := 1 beam `Float22.m", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        #endregion

        #region Information of Memory-mapped file size

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void SizeOfMemoryMappedIntegerArray()
        {
            AType expected = AInteger.Create(2);

            AType result =  this.engine.Execute<AType>("_items{-1;'Integer23'}");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("System"), TestCategory("Items"), TestMethod]
        public void SizeOfMemoryMappedChargerArray()
        {
            AType expected = AInteger.Create(15);

            this.engine.Execute<AType>("_items{15;'Char25'}");
            AType result = this.engine.Execute<AType>("_items{-1;'Char25'}");

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
            this.engine.Execute<AType>("t := 1 beam `Integer23", scope);
            this.engine.Execute<AType>("t[,] := 2 4 1", scope);
        }

        #endregion
    }
}
