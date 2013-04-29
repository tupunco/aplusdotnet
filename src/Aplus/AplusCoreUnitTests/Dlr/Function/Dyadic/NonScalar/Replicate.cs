using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Runtime;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Replicate : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateIntegerList2IntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(4),
                AInteger.Create(4),
                AInteger.Create(4)
            );
            AType result = this.engine.Execute<AType>("2 3 0 / 1 4 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateIntegerList2IntegerListUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(1),
                AInteger.Create(1),
                AInteger.Create(4),
                AInteger.Create(4),
                AInteger.Create(4)
            );
            AType result = this.engineUni.Execute<AType>("2 3 0 / 1 4 8");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateIntegerList2MatrixWithFrame()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(0),
                        AInteger.Create(1),
                        AInteger.Create(2)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(4),
                        AInteger.Create(5)
                    )
                )
            );
            AType result = this.engine.Execute<AType>("1 0 / iota 2 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateIntegerList2Integer()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0),
                AInteger.Create(0)
            );
            AType result = this.engine.Execute<AType>("2 3 / iota 1");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateInteger2Matrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1),
                    AInteger.Create(2)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1),
                    AInteger.Create(2)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(3),
                    AInteger.Create(4),
                    AInteger.Create(5)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(3),
                    AInteger.Create(4),
                    AInteger.Create(5)
                )
            );
            AType result = this.engine.Execute<AType>("2 / iota 2 3");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateNull2Strand()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );

            AType result = this.engine.Execute<AType>("0 / (2;3)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateInteger2Box()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(Helpers.BuildString("test")),
                ABox.Create(Helpers.BuildString("test")),
                ABox.Create(Helpers.BuildString("test"))
            );
            AType result = this.engine.Execute<AType>("3 / < 'test'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateInteger2Null()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );
            AType result = this.engine.Execute<AType>("3 / ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateInteger2Float()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(4.5),
                AFloat.Create(4.5),
                AFloat.Create(4.5)
            );
            AType result = this.engine.Execute<AType>("3 / 4.5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateIntegerList2CharacterConstant2()
        {
            AType expected = Helpers.BuildString("aaaaa");

            AType result = this.engine.Execute<AType>("2 0 3 0 / 'a'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateIntegerList2CharacterConstant()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("ab"),
                Helpers.BuildString("ab"),
                Helpers.BuildString("ab"),
                Helpers.BuildString("ef")
            );
            AType result = this.engine.Execute<AType>("3 0 1 / 3 2 rho 'abcdef'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateCompress1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(4),
                AInteger.Create(3),
                AInteger.Create(0),
                AInteger.Create(8)
            );
            AType result = this.engine.Execute<AType>("(a >= 0) / a := 4 -1 3 0 8 -5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateCompress2()
        {
            AType expected = AArray.Create(ATypes.AInteger);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 4 };
            expected.Rank = 2;

            AType result = this.engine.Execute<AType>("0 0 0 / iota 3 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        public void ReplicateIntegerList2NestedMixedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := {+}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("a"),
                ASymbol.Create("a"),
                ABox.Create(AInteger.Create(4)),
                ABox.Create(f),
                ABox.Create(f)
            );

            AType result = this.engine.Execute<AType>("2 0 1 2 / `a , (3;4;f)", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ReplicateDomainError()
        {
            AType result = this.engine.Execute<AType>("4 -2 2 / 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ReplicateTypeError()
        {
            AType result = this.engine.Execute<AType>("3.0000000000003 4.3 / 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ReplicateTypeError2()
        {
            AType result = this.engine.Execute<AType>("`test / 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ReplicateRankError1()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) / 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ReplicateRankError2()
        {
            AType result = this.engine.Execute<AType>("(iota 2 3) / ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ReplicateLengthError1()
        {
            AType result = this.engine.Execute<AType>("2 3 4 / 6 2 1 1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Replicate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void ReplicateLengthError2()
        {
            AType result = this.engine.Execute<AType>("3 5 / ()");
        }
    }
}
