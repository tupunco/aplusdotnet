using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Operator.Dyadic
{
    [TestClass]
    public class Each : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void ShapeContaining0Rank()
        {
            AType expected = Utils.ANull();
            AType result = this.engine.Execute<AType>("3 + each 0 rho ()");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseDyadicScalarFunctionScalar2Strand()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]
                {
                    AInteger.Create(5),
                    AInteger.Create(8),
                    AInteger.Create(6)
                }
            );

            AType result = this.engine.Execute<AType>("3 + each (3;5;2)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseDyadicScalarFunctionStrand2Scalar()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]
                {
                    AInteger.Create(5),
                    AInteger.Create(3),
                    AInteger.Create(5),
                    AInteger.Create(5)
                }
            );

            AType result = this.engine.Execute<AType>("min each{(5;6;3;7);<5}");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseDyadicScalarFunctionScalar2OneElementArray()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(AInteger.Create(12))
            );

            AType result = this.engine.Execute<AType>(" 3 ((* each)) 1 rho 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseDyadicNonScalarFunctionStrand2Strand()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1))
                ),
                ABox.Create(
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(
                            ATypes.AInteger,
                            AInteger.Create(8),
                            AInteger.Create(9),
                            AInteger.Create(10),
                            AInteger.Create(11)
                        )
                    )
                )
            );

            AType result = this.engine.Execute<AType>("(2;-1) take each (iota 4; iota 3 4)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseDyadicScalarFunctionScalard2Strand()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(ABox.Create(AInteger.Create(5))),
                ABox.Create(
                    AArray.Create(
                        ATypes.ABox,
                        ABox.Create(AInteger.Create(7)),
                        ABox.Create(AInteger.Create(8))
                    )
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := ((+each)each)", scope);
            AType result = this.engine.Execute<AType>("a{2;(3;(5;6))}",scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseUserDefinedFunctionScalard2IntegerMatrix()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                Helpers.BuildStrand(
                    new AType[]{
                        AFloat.Create(3),
                        AFloat.Create(2.5),
                        AFloat.Create(2)
                    }
                ),
                Helpers.BuildStrand(
                    new AType[]{
                        AFloat.Create(4.5),
                        AFloat.Create(4),
                        AFloat.Create(3.5)
                    }
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b;c} : (b+c)%2", scope);
            AType result = this.engine.Execute<AType>("4 a each iota 2 3", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseDyadicScalarFunctionScalar2Null()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("() * each ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachUseDyadicScalarFunctionNullArray2Scalar()
        {
            AType expected = AArray.Create(ATypes.ANull);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 3};
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>("(+each){0 2 3 rho 4;6}");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        public void EachComplexTest()
        {
            AType expected = AArray.Create(
                ATypes.AChar,
                Helpers.BuildString("ab   "),
                Helpers.BuildString("-----"),
                Helpers.BuildString("cde  ")
            );

            AType result = this.engine.Execute<AType>("> (max/ > # each vs) take each vs := ('ab';'-----';'cde')");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void EachRankError1()
        {
            this.engine.Execute<AType>("(iota 2 3) + each 3 45 5 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void EachRankError2()
        {
            this.engine.Execute<AType>("() + each iota 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void EachLengthError1()
        {
            this.engine.Execute<AType>("(iota 2 3) + each iota 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void EachLengthError2()
        {
            this.engine.Execute<AType>("() + each 7 4 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.NonFunction))]
        public void EachNonFunctionError()
        {
            this.engine.Execute<AType>("(iota 2 3) `sym each 3 45 5 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void EachValenceError1()
        {
            this.engine.Execute<AType>("3 rtack each 6");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void EachValenceError2()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b} : b+b", scope);
            this.engine.Execute<AType>("3 a each 7", scope);
        }
    }
}
