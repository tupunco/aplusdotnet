using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using System.IO;
using Microsoft.Scripting.Hosting;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.MemoryMappedFile
{
    [TestClass]
    public class Map : AbstractTest
    {
        [TestInitialize]
        public void InitMemoryMappedFile()
        {
            this.engine.Execute<AType>(CreateMapInCreator("IntegerScalar.m", "67"));
            this.engine.Execute<AType>(CreateMapInCreator("Integer23.m","2 3 rho 5 6 7 9 8 2"));
            this.engine.Execute<AType>(CreateMapInCreator("Float22.m", "2 2 rho 3.4 1.4 7.6 1.1"));
            this.engine.Execute<AType>(CreateMapInCreator("Char25.m", "2 5 rho 'HelloWorld'"));
        }

        [TestCleanup]
        public void CleanUpMemoryMappedFile()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string[] files = new string[] { "IntegerScalar.m", "Integer23.m", "Float22.m", "Char25.m" };

            for (int i = 0; i < files.Length; i++)
            {
                string name = CreatePath(files[i]);

                if (File.Exists(name))
                {
                    File.Delete(name);
                }
            }
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadIntegerScalar()
        {
            AType expected = AInteger.Create(67);

            AType result = this.engine.Execute<AType>(CreateMapIn(0, "IntegerScalar.m"));

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
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
            this.engine.Execute<AType>("a := " + CreateMapIn(1, "Integer23.m"), scope);
            this.engine.Execute<AType>("b := " + CreateMapIn(1, "Integer23.m"), scope);

            this.engine.Execute<AType>("a[1;2] := 55", scope);
            AType result = this.engine.Execute<AType>("b", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            scope.RemoveVariable(".a");
            scope.RemoveVariable(".b");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
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
            this.engine.Execute<AType>("a := " + CreateMapIn(1, "Float22.m"), scope);
            this.engine.Execute<AType>("b := " + CreateMapIn(1, "Float22.m"), scope);

            this.engine.Execute<AType>("((0;1) # b) := 8", scope);
            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            scope.RemoveVariable(".a");
            scope.RemoveVariable(".b");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Map"), TestMethod]
        public void ReadAndWriteCharArray()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("Hello"),
                Helpers.BuildString("City ")
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := " + CreateMapIn(1, "Char25.m"), scope);
            this.engine.Execute<AType>("b := " + CreateMapIn(1, "Char25.m"), scope);

            this.engine.Execute<AType>("(1 drop b) := 1 5 rho 'City '", scope);
            AType result = this.engine.Execute<AType>("a", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);

            scope.RemoveVariable(".a");
            scope.RemoveVariable(".b");
        }

        public static string CreateMapInCreator(string name, string data)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("'");
            builder.Append(CreatePath(name));
            builder.Append("'");
            builder.Append(" beam ");
            builder.Append(data);

            return builder.ToString();
        }

        public static string CreateMapIn(byte mode, string name)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(mode);
            builder.Append(" beam ");
            builder.Append("'");
            builder.Append(CreatePath(name));
            builder.Append("'");

            return builder.ToString();
        }

        public static string CreatePath(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), name);
        }
    }
}
