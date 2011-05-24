using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore;
using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class IndexingAssign : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void SimpleIndexing()
        {
            AType expected = AInteger.Create(10);

            AType result = this.engine.Execute<AType>("(0 1 2 3)[1] := 10");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void SimpleIndexing2()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType var_a = new int[] { 1, 2, 3, 4 }.ToAArray();

            scope.SetVariable(".a", var_a);

            AType expected_a = new int[] { 1, 2, -10, 4 }.ToAArray();
            AType expected = AInteger.Create(-10);

            AType result = this.engine.Execute<AType>("a[2] := -10", scope);
            AType result_a = scope.GetVariable<AType>(".a");

            Assert.AreEqual(expected, result, "Incorrect value returned");
            Assert.AreEqual(expected_a, result_a, "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void TypeConvertIndexing()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType var_a = new int[] { 1, 2, 3, 4 }.ToAArray();

            scope.SetVariable(".a", var_a);

            AType expected_a = new int[] { 1, 2, -10, 4 }.ToAArray();
            AType expected = AInteger.Create(-10);

            AType result = this.engine.Execute<AType>("a[2] := -10.0", scope);
            AType result_a = scope.GetVariable<AType>(".a");

            Assert.AreEqual(expected, result, "Incorrect value returned");
            Assert.AreEqual(expected_a, result_a, "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MultipleWithSingleIndexing()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType var_a = new int[] { 1, 2, 3, 4 }.ToAArray();

            scope.SetVariable(".a", var_a);

            AType expected_a = new int[] { 1, 2, -10, -10 }.ToAArray();
            AType expected = AInteger.Create(-10);

            AType result = this.engine.Execute<AType>("a[2 3] := -10", scope);
            AType result_a = scope.GetVariable<AType>(".a");

            Assert.AreEqual(expected, result, "Incorrect value returned");
            Assert.AreEqual(expected_a, result_a, "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MultipleWithMultipleIndexing()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType var_a = new int[] { 1, 2, 3, 4 }.ToAArray();

            scope.SetVariable(".a", var_a);

            AType expected_a = new int[] { 1, 2, -10, -30 }.ToAArray();
            AType expected = new int[] { -10, -30 }.ToAArray();

            AType result = this.engine.Execute<AType>("a[2 3] := -10 -30", scope);
            AType result_a = scope.GetVariable<AType>(".a");

            Assert.AreEqual(expected, result, "Incorrect value returned");
            Assert.AreEqual(expected_a, result_a, "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MatrixIndexing()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType var_a = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(0), AInteger.Create(0))
            );

            AType expected_a = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(3), AInteger.Create(4), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(5), AInteger.Create(6), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0), AInteger.Create(0), AInteger.Create(0))
            );

            scope.SetVariable(".a", var_a);

            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(6))
            );

            AType result = this.engine.Execute<AType>("a[1 2;1 2] := 2 2 rho 3 4 5 6", scope);
            AType result_a = scope.GetVariable<AType>(".a");

            Assert.AreEqual(expected, result, "Incorrect value returned");
            Assert.AreEqual(expected_a, result_a, "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MatrixIndexing2()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := 2 2 rho 0", scope);

            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(100))
            );

            this.engine.Execute<AType>("a[1;1] := 100", scope);
            AType result_a = scope.GetVariable<AType>(".a");

            Assert.AreEqual(expected, result_a, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ReplaceAllIndexing()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType var_a = new int[] { 1, 2, 3, 4 }.ToAArray();

            scope.SetVariable(".a", var_a);

            AType expected_a = new int[] { 1200, 1200, 1200, 1200 }.ToAArray();
            AType expected = AInteger.Create(1200);

            AType result = this.engine.Execute<AType>("a[] := 1200", scope);
            AType result_a = scope.GetVariable<AType>(".a");

            Assert.AreEqual(expected, result, "Incorrect value returned");
            Assert.AreEqual(expected_a, result_a, "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ReplaceScalarIndexing()
        {
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", AInteger.Create(100));

            this.engine.Execute<AType>("a[] := -500", scope);

            Assert.AreEqual(AInteger.Create(-500), scope.GetVariable<AType>(".a"), "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ReplaceScalarIndexingTypeError()
        {
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", AInteger.Create(100));

            this.engine.Execute<AType>("a[] := 'F'", scope);
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorIndexing()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType var_a = new int[] { 1, 2, 3, 4 }.ToAArray();

            scope.SetVariable(".a", var_a);
            
            this.engine.Execute<AType>("a[2] := 's'", scope);
        }
    }
}
