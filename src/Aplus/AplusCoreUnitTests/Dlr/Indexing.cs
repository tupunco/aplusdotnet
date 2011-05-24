using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class Indexing : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void SimpleIndexing()
        {
            AType expected = AInteger.Create(1);

            AType result = this.engine.Execute<AType>("(0 1 2 3)[1]");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void NullIndexing()
        {
            this.engine.Execute<AType>("(100)[()]");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MultipleSimpleIndexing()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("(0 1 2 3)[1 2]");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void DimensionSimpleIndexing()
        {
            AType expected = AInteger.Create(5);

            AType result = this.engine.Execute<AType>("(iota 4 4)[1;1]");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void DimensionComplexIndexing()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(6), AInteger.Create(7))
            );

            AType result = this.engine.Execute<AType>("(iota 4 4 4)[0;0 1; 2 3]");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void DimensionComplex2Indexing()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
            );

            AType result = this.engine.Execute<AType>("(iota 2 2 2)[0;0 1]");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }


        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MultipleIndexingWithNull()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(2)
            );

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a",
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(2)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4))
                )
            );

            AType result = this.engine.Execute<AType>("a[0;]", scope);

            Assert.AreEqual(expected, result, "Incorrect indexing occured");
        }


        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void Null2Indexing()
        {
            //  a := iota 3 3 3
            //  a[0;()] == a[0; iota (rho a)[1]]
            //
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a",
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(2)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4))
                )
            );
            AType expected = this.engine.Execute<AType>("a[0; iota (rho a)[1]]", scope);
            AType result = this.engine.Execute<AType>("a[0;]", scope);

            Assert.AreEqual(expected, result, "Incorrect indexing occured");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MultiDimIndexing()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(2), AInteger.Create(2)
            );

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a", 
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(2)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4))
                )
            );

            this.engine.Execute<AType>("b:=a[0 0;1]", scope);

            Assert.IsTrue(scope.ContainsVariable(".b"), "Variable not found");
            Assert.AreEqual(expected, scope.GetVariable<AType>(".b"), "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MatrixIndexing()
        {
            AType expected = AArray.Create(ATypes.AChar,
                Helpers.BuildString("hello"),
                Helpers.BuildString("world")
            );

            AType result = this.engine.Execute<AType>("'lowrdhe'[2 5 rho 5 6 0 0 1 2 1 3 0 4]");

            Assert.AreEqual(expected, result, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void MultiMatrixIndexing()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(5), AInteger.Create(0), AInteger.Create(5), AInteger.Create(0)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(0), AInteger.Create(5), AInteger.Create(0), AInteger.Create(5))
            );

            AType result = this.engine.Execute<AType>("(iota 5 5 5)[0; 2 5 rho 0 1;0]");

            Assert.AreEqual(expected, result, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RankErrorOverIndexing()
        {
            this.engine.Execute<AType>("(1 2 3 4)[1;1]");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorIndexing()
        {
            this.engine.Execute<AType>("(1 2 3 4)[1.2]");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        [ExpectedException(typeof(Error.Index))]
        public void NegativeIndexing()
        {
            this.engine.Execute<AType>("(1 2 3 4)[-4]");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ScalarIndexingError()
        {
            this.engine.Execute<AType>("1[0]");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ScalarIndexing()
        {
            AType result = this.engine.Execute<AType>("1[]");

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Incorrect value returned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void ToleranceIndexing()
        {
            AType result = this.engine.Execute<AType>("(1 2 3 4)[1.000000000000001]");

            Assert.AreEqual<AType>(AInteger.Create(2), result, "Incorrect result returned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void IndexingOrderA()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1));

            AType result = this.engine.Execute<AType>("a[a:=0 1]");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }

        [TestCategory("DLR"), TestCategory("Indexing"), TestMethod]
        public void IndexingOrderB()
        {
            AType expected = AInteger.Create(3);

            AType result = this.engine.Execute<AType>("(iota 2 2)[b ; b:=1]");

            Assert.AreEqual(expected, result, "Incorrect value assigned");
        }
    }
}
