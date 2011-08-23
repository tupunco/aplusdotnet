using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;
using System.IO;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.MemoryMappedFile
{
    [TestClass]
    public class Operations : AbstractTest
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
            TestUtils.DeleteMemoryMappedFiles(ref this.engine);
        }

        #endregion

        #region Case 2: Disclose, Enclose, Raze

        #region Disclose

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void DiscloseMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(1),
                    AInteger.Create(6),
                    AInteger.Create(7)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(3),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := > a", scope);

            this.engine.Execute<AType>("(0 0 flip b) := 1 3", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void EncloseAndDiscloseMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(5),
                    AInteger.Create(45),
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := < a", scope);
            this.engine.Execute<AType>("c := > b", scope);

            this.engine.Execute<AType>("((0;1) # c) := 45", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void StrandWithMappedIntegerArray1()
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
                    AInteger.Create(56)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := 1 beam `Integer23.m", scope);

            this.engine.Execute<AType>("c := (a; iota 2 3; b)", scope);
            this.engine.Execute<AType>("d := > c[2]", scope);
            this.engine.Execute<AType>("d[1;2] := 56", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void StrandWithMappedIntegerArray2()
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);

            this.engine.Execute<AType>("c := (a; iota 2 3)", scope);
            this.engine.Execute<AType>("d := > c", scope);

            this.engine.Execute<AType>("d[1;0] := 56", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Rake

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void RakeMappedIntegerArray()
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
                    AInteger.Create(4),
                    AInteger.Create(2),
                    AInteger.Create(8)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := in a", scope);

            this.engine.Execute<AType>("c := > b[0]", scope);

            this.engine.Execute<AType>("(0 1 / c) := 1 3 rho 4 2 8", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Raze

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void RazeMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(1),
                    AInteger.Create(6),
                    AInteger.Create(7)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(3),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := pick a", scope);

            this.engine.Execute<AType>("(0 0 flip b):=1 3", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void RazeNestedMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(4),
                    AInteger.Create(4),
                    AInteger.Create(4)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(8),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := < a", scope);
            this.engine.Execute<AType>("c := pick b", scope);

            this.engine.Execute<AType>("(1 take c) := 4", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void RazeBoxVectorWithMappedIntegerArray()
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := (a;iota 2 3)", scope);
            this.engine.Execute<AType>("c := pick b", scope);

            this.engine.Execute<AType>("c[1;0] := 14", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #endregion

        #region Case 3: Right, Identity

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void RightMappedIntegerArray()
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := rtack a", scope);

            this.engine.Execute<AType>("b[1;1] := 5", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void RightEnclosedMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(8),
                    AInteger.Create(2),
                    AInteger.Create(1)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(5),
                    AInteger.Create(4),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := rtack < a", scope);
            this.engine.Execute<AType>("c := > b", scope);

            this.engine.Execute<AType>("(!c) := 8 2 1 5 4 2", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void IdentityMappedIntegerArray()
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := + a", scope);
            this.engine.Execute<AType>("b[] := 34", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void IdentityMappedFloat()
        {
            AType expected = AFloat.Create(2.3);

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `FloatScalar.m", scope);
            this.engine.Execute<AType>("b := + a", scope);
            this.engine.Execute<AType>("b[] := 3.4", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Case 4: Bracket indexing, Choose, Pick

        #region Bracket indexing

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void BracketIndexingWithNullMappedIntegerArray()
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
                    AInteger.Create(12),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := a[]", scope);

            this.engine.Execute<AType>("b[1;1] := 12", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Choose

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void ChooseNull2MappedIntegerArray()
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
                    AInteger.Create(5),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := () # a", scope);

            this.engine.Execute<AType>("b[1;1] := 5", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void ChooseInteger2MappedIntegerArray()
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := 1 # a", scope);

            this.engine.Execute<AType>("b[1] := 12", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void ChooseBoxVector2MappedIntegerArray()
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            AType g = this.engine.Execute<AType>("b := (0;0 2) # a", scope);

            this.engine.Execute<AType>("b[1] := 65", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Pick

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void PickWithNull2MappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(23),
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := () pick a", scope);

            this.engine.Execute<AType>("b[0;0] := 23", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void PickWithNull2NestedMappedIntegerArray()
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := () pick < a", scope);
            this.engine.Execute<AType>("c := > b", scope);

            this.engine.Execute<AType>("((0;0;0) # c) := 8", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void PickWithNull2StrandWithMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(6),
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
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("b := () pick (2;a;8)", scope);
            this.engine.Execute<AType>("c := > b[1]", scope);

            this.engine.Execute<AType>("((0;0) # c) := 6", scope);

            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #endregion

        #region Case 5: ReplaceAll

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void ReplaceAllMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(3),
                    AInteger.Create(9),
                    AInteger.Create(1)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(8),
                    AInteger.Create(6),
                    AInteger.Create(5)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("a[] := 2 3 rho 3 9 1 8 6 5", scope);

            AType result = this.engine.Execute<AType>("1 beam `Integer23.m", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void ReplaceAllMappedFloatArray()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(6.4),
                    AFloat.Create(6.4)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(6.4),
                    AFloat.Create(6.4)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `Float22.m", scope);
            this.engine.Execute<AType>("a[] := 6.4", scope);

            AType result = this.engine.Execute<AType>("1 beam `Float22.m", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Operations"), TestMethod]
        public void ReplaceAllMappedCharacter()
        {
            AType expected = AChar.Create('B');

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 1 beam `CharScalar", scope);
            this.engine.Execute<AType>("a[] := 'B'", scope);

            AType result = this.engine.Execute<AType>("1 beam `CharScalar", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }


        #endregion

        #region Case 6: Append

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void CreateAndAppendMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(10),
                    AInteger.Create(20),
                    AInteger.Create(30),
                    AInteger.Create(40)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(1),
                    AInteger.Create(2),
                    AInteger.Create(3),
                    AInteger.Create(4)
                )
            );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("'test.m' beam 0 4 rho 0", scope);

            this.engine.Execute<AType>("_items{10;'test.m'}", scope);

            this.engine.Execute<AType>("t := 1 beam 'test.m'", scope);

            this.engine.Execute<AType>("t[,] := 10 20 30 40", scope);
            this.engine.Execute<AType>("t[,] := 1 2 3 4", scope);

            AType result = scope.GetVariable<AType>(".t");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            scope.RemoveVariable(".");

            result = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete("test.m");
        }

        #endregion

        #region Case 7: UserDefined function

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void InvokeUserDefinedFunctionWithMappedIntegerArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(7),
                    AInteger.Create(4),
                    AInteger.Create(1)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(8),
                    AInteger.Create(2)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{a} : {(1 take a) := 1 3 rho 7 4 1 }", scope);

            this.engine.Execute<AType>("a := 1 beam `Integer23.m", scope);
            this.engine.Execute<AType>("f{a}", scope);

            AType result = this.engine.Execute<AType>("1 beam `Integer23.m", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Operations"), TestMethod]
        public void InvokeUserDefinedFunctionWithMappedFloatArray()
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
                    AFloat.Create(34)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{} : { 1 beam `Float22.m }", scope);

            this.engine.Execute<AType>("a := f{}", scope);
            this.engine.Execute<AType>("a[1;1] := 34", scope);

            AType result = this.engine.Execute<AType>("1 beam `Float22.m", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion
    }
}
