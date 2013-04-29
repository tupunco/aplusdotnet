using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Member : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void SimpleIntegerMember()
        {
            AType result = this.engine.Execute<AType>("1 in 1");

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(AInteger.Create(1)));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void SimpleIntegerMemberUni()
        {
            AType result = this.engineUni.Execute<AType>("1 I.? 1");

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(AInteger.Create(1)));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void SimpleFloatComparisonMember()
        {
            ScriptScope scope = this.engine.CreateScope();

            scope.SetVariable(".a", AFloat.Create(1.0));
            scope.SetVariable(".b", AFloat.Create(1.0 + 1e-14));

            AType result = this.engine.Execute<AType>("a in b", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(AInteger.Create(1)));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void IntegerVectorMember()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(0)
            );

            AType result = this.engine.Execute<AType>("1 0 in 1");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void Char2IntTypeError()
        {
            this.engine.Execute<AType>(" 'this' in 1 2 ");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void MemberRankError()
        {
            this.engine.Execute<AType>(" 1 in iota 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void MemberLengthError()
        {
            this.engine.Execute<AType>(" 1 2 in (iota 3 3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void IntegerMatrixMember()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("1 2 in iota 2 2");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void MatrixScalarMember()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0))
            );

            AType result = this.engine.Execute<AType>("(iota 2 2) in 1");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void MatrixFloatComparisonMember()
        {
            ScriptScope scope = this.engine.CreateScope();

            scope.SetVariable(".a", AArray.Create(ATypes.AFloat, AFloat.Create(1.0), AFloat.Create(1.0)));
            scope.SetVariable(".b",
                AArray.Create(ATypes.AFloat,
                    AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(0)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(1.0 + 1e-14), AFloat.Create(1.0 + 1e-14))
                )
            );

            AType result = this.engine.Execute<AType>("a in b", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(AInteger.Create(1)));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void MatrixFloatComparisonMember2()
        {
            ScriptScope scope = this.engine.CreateScope();

            scope.SetVariable(".a", AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)));
            scope.SetVariable(".b",
                AArray.Create(ATypes.AFloat,
                    AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(0)),
                    AArray.Create(ATypes.AFloat, AFloat.Create(1.0 + 1e-14), AFloat.Create(1.0 + 1e-14))
                )
            );

            AType result = this.engine.Execute<AType>("a in b", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(AInteger.Create(1)));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void MultiDimensionMatrixScalarMember()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(0))
                ),
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0))
                )
            );

            AType result = this.engine.Execute<AType>("(iota 2 2 2) in 1 2");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void MultiDimensionMatrix2MatrixMember()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0))           
            );

            AType result = this.engine.Execute<AType>("(iota 2 2 2) in (iota 2 2)");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void MultiDimensionMatrix2MatrixComplexMember()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(0))
            );

            AType result = this.engine.Execute<AType>("(iota 2 2 3) in iota 3 3");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void SimpleBoxMember()
        {
            ScriptScope scope = this.engine.CreateScope();

            scope.SetVariable(".a", ABox.Create(AInteger.Create(1)));
            scope.SetVariable(".b", ABox.Create(AInteger.Create(1)));

            AType result = this.engine.Execute<AType>("a in b", scope);

            Assert.AreEqual<AType>(AInteger.Create(1), result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(AInteger.Create(1)));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void BoxofVectorMember()
        {
            ScriptScope scope = this.engine.CreateScope();

            scope.SetVariable(".a", ABox.Create(AInteger.Create(1)));
            scope.SetVariable(".b", ABox.Create(AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(2))));

            AType result = this.engine.Execute<AType>("a in b", scope);

            Assert.AreEqual<AType>(AInteger.Create(0), result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(AInteger.Create(0)));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void IntegerInNullMember()
        {
            AType expected = AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(0));

            AType result = this.engine.Execute<AType>("1 2 in ()");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void ChainedMember()
        {
            AType expected = AInteger.Create(0);

            AType result = this.engine.Execute<AType>("1 in 0 1 3 in iota 3 3");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void NullInNullMember()
        {
            AType result = this.engine.Execute<AType>("() in ()");

            Assert.AreEqual(ATypes.AInteger, result.Type);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Member"), TestMethod]
        public void ScalarInNullMember()
        {
            AType expected = AInteger.Create(0);
            AType result = this.engine.Execute<AType>("1 in ()");

            Assert.AreEqual<AType>(expected, result, "Invalid value produced");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
