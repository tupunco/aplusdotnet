using System;
using System.IO;
using System.Text;

using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.MemoryMappedFile
{
    [TestClass]
    public class Map : AbstractTest
    {
        #region Init and Clean up

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

        #region Create

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void MapSymbolVector2FloatScalar()
        {
            AType expected = AFloat.Create(3.1);

            this.engine.Execute<AType>("`FloatScalar `Nothing beam 3.1");
            string executable = string.Format("0 beam `{0}", MappedFiles.FloatScalar.GetFileName());
            AType result = this.engine.Execute<AType>(executable);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            result = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete("FloatScalar.m");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void MapCharArray2CharArray()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("Welcome"),
                Helpers.BuildString("Szeged!")
            );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("(2 4 rho 'CharAray') beam 2 7 rho 'WelcomeSzeged!'");
            this.engine.Execute<AType>("t := 0 beam 'CharAray.m'", scope);

            AType result = scope.GetVariable<AType>(".t");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            scope.RemoveVariable(".");

            result = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete("CharAray.m");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void MapCharNullChar2IntegerScalar()
        {
            AType expected = AInteger.Create(7);

            this.engine.Execute<AType>("(`char ? ()) beam 7");
            AType result = this.engine.Execute<AType>("0 beam '.m'");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            result = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete(".m");
        }

        #endregion

        #region Read

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadIntegerScalar()
        {
            AType expected = AInteger.Create(67);

            ScriptScope scope = this.engine.CreateScope();
            string executable = string.Format("0 beam `{0}", MappedFiles.IntegerScalar.GetFileName());
            this.engine.Execute<AType>(string.Format("a := {0}", executable), scope);
            this.engine.Execute<AType>("a[] := 55", scope);

            AType result = this.engine.Execute<AType>(executable, scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadCharArray()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("Hello"),
                Helpers.BuildString("World")
            );

            ScriptScope scope = this.engine.CreateScope();

            string executable = string.Format("0 beam `{0}", MappedFiles.CharMatrix.GetFileName());
            this.engine.Execute<AType>(string.Format("a := {0}", executable), scope);
            this.engine.Execute<AType>("((1;0) # a) := 'w'", scope);

            executable = string.Format("0 beam `{0}", MappedFiles.CharMatrix.GetFileName());
            AType result = this.engine.Execute<AType>(executable, scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadFloatScalarUseAPATH()
        {
            AType expected = AFloat.Create(3.4);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

            Directory.CreateDirectory(path);

            string pathWithFileName = "'" + Path.Combine(path, "FloatTest") + "'";

            this.engine.Execute<AType>(pathWithFileName + " beam 3.4");

            string apath = Environment.GetEnvironmentVariable("APATH", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("APATH", path, EnvironmentVariableTarget.User);

            AType result = this.engine.Execute<AType>("0 beam `FloatTest.m");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            Environment.SetEnvironmentVariable("APATH", apath, EnvironmentVariableTarget.User);

            result = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Directory.Delete(path, true);
        }

        #endregion

        #region Read and Write

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadAndWriteIntegerArray()
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
                    AInteger.Create(55)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            string value = string.Format("1 beam `{0}", MappedFiles.IntegerMatrix.GetFileName());
            this.engine.Execute<AType>(string.Format("a := {0}", value), scope);
            this.engine.Execute<AType>(string.Format("b := {0}", value), scope);

            this.engine.Execute<AType>("a[1;2] := 55", scope);
            AType result = this.engine.Execute<AType>("b", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadAndWriteFloatArray()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(3.4),
                    AFloat.Create(8)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(7.6),
                    AFloat.Create(1.1)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            string value = string.Format("1 beam `{0}", MappedFiles.FloatMatrix.GetFileName());
            this.engine.Execute<AType>(string.Format("a := {0}", value), scope);
            this.engine.Execute<AType>(string.Format("b := {0}", value), scope);

            this.engine.Execute<AType>("((0;1) # b) := 8", scope);
            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadAndWriteCharArray()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("Hello"),
                Helpers.BuildString("City ")
            );

            ScriptScope scope = this.engine.CreateScope();
            string value = string.Format("1 beam `{0}", MappedFiles.CharMatrix.GetFileName());
            this.engine.Execute<AType>(string.Format("a := {0}", value), scope);
            this.engine.Execute<AType>(string.Format("b := {0}", value), scope);

            this.engine.Execute<AType>("(1 drop b) := 1 5 rho 'City '", scope);
            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Read and Local Write

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadAndLocalWriteIntegerArray()
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
                    AInteger.Create(8),
                    AInteger.Create(2),
                    AInteger.Create(7)
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            string value = string.Format("1 beam `{0}", MappedFiles.IntegerMatrix.GetFileName());
            this.engine.Execute<AType>(string.Format("a := {0}", value), scope);
            this.engine.Execute<AType>(string.Format("b := {0}", value), scope);

            this.engine.Execute<AType>("(1 drop a) := 1 3 rho 8 2 7", scope);

            Assert.AreEqual(InfoResult.OK, scope.GetVariable<AType>(".a").CompareInfos(expected));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".a"));
            Assert.AreEqual(InfoResult.OK, scope.GetVariable<AType>(".b").CompareInfos(expected));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".b"));

            this.engine.Execute<AType>("((1;0) # b) := 45", scope);

            expected = AArray.Create(
                 ATypes.AInteger,
                 AArray.Create(
                     ATypes.AInteger,
                     AInteger.Create(5),
                     AInteger.Create(6),
                     AInteger.Create(7)
                 ),
                 AArray.Create(
                     ATypes.AInteger,
                     AInteger.Create(45),
                     AInteger.Create(2),
                     AInteger.Create(7)
                 )
             );

            Assert.AreEqual(InfoResult.OK, scope.GetVariable<AType>(".a").CompareInfos(expected));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".a"));

            Assert.AreEqual(InfoResult.OK, scope.GetVariable<AType>(".b").CompareInfos(expected));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".b"));
        }

        #endregion

        #region Errors

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void MapDomainError()
        {
            this.engine.Execute<AType>("(< 4) beam iota 2 2");
        }

        [TestCategory("DLR"), TestCategory("MemoryMappedFiles"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void MapTypeError()
        {
            AType result = this.engine.Execute<AType>("'test' beam `s1");
        }

        #endregion
    }
}
