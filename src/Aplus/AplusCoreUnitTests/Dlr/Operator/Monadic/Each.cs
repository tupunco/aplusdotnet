using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Operator.Monadic
{
    [TestClass]
    public class Each : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void EachUsePrimitiveFunctionToCharacterMatrix()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                AArray.Create(ATypes.ABox, ABox.Create(ASymbol.Create("a")), ABox.Create(ASymbol.Create("b"))),
                AArray.Create(ATypes.ABox, ABox.Create(ASymbol.Create("c")), ABox.Create(ASymbol.Create("d")))
            );

            AType result = this.engine.Execute<AType>("(pack each) 2 2 rho 'abcd'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void EachUsePrimitiveFunctionToStrand()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AInteger.Create(2),
                    AInteger.Create(-8),
                    AInteger.Create(-5),
                    AInteger.Create(-4),
                }
            );
              
            AType result = this.engine.Execute<AType>("(- each){(4;5;8;-2)}");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void EachUsePrimitiveFunctionToNull()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("| each ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void EachUsePrimitiveFunctionToNullArray()
        {
            AType expected = AArray.Create(ATypes.ANull);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 4 };
            expected.Rank = 3;

            AType result = this.engine.Execute<AType>("* each 0 2 4 rho 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void EachUseUserDefinedFunctionToStrand()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b}: b+b", scope);

            AType expected = Helpers.BuildStrand(
                new AType[]
                {
                    AInteger.Create(14),
                    AInteger.Create(16),
                    AInteger.Create(6)
                }
            );

            AType result = this.engine.Execute<AType>("a each{(3;8;7)}", scope);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void EachPrimitiveFunctionComplexTest()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("pick rho each rho each (2; iota 2; iota 2 3)");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void MultipleEachFromVariable()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    ABox.Create(AInteger.Create(-3)),
                    Helpers.BuildStrand(
                        new AType[]{
                            AInteger.Create(-2),
                            AInteger.Create(-1)
                        }
                    )
                }
            );

            AType result = this.engine.Execute<AType>("a:=(- each); a each ((1;2);3)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void MultipleEach1()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    ABox.Create(AInteger.Create(-3)),
                    Helpers.BuildStrand(
                        new AType[]{
                            AInteger.Create(-2),
                            AInteger.Create(-1)
                        }
                    )
                }
            );

            AType result = this.engine.Execute<AType>("(- each) each ((1;2);3)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void MultipleEach2()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(ABox.Create(ABox.Create(AInteger.Create(-4)))),
                ABox.Create(
                    AArray.Create(
                        ATypes.ABox,
                        ABox.Create(ABox.Create(AInteger.Create(-7))),
                        ABox.Create(
                            AArray.Create(
                                ATypes.ABox,
                                ABox.Create(AInteger.Create(-3)),
                                ABox.Create(AInteger.Create(-7))
                            )
                        )
                    )
                )
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a := (((- each)each)each)", scope);
            AType result = this.engine.Execute<AType>("a{(4;(7;(3;7)))}", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        public void ComplexEach()
        {
            AType expected = ABox.Create(AInteger.Create(-5));

            AType result = this.engine.Execute<AType>("(if 3 (-) else (*)) each 5");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void EachValenceError1()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("/ each (3;7)", scope);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.Valence))]
        public void EachValenceError2()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a{b;c}: b+c", scope);
            this.engine.Execute<AType>("a each (3;7)", scope);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Each"), TestMethod]
        [ExpectedException(typeof(Error.NonFunction))]
        public void EachNonFunctionError()
        {
            this.engine.Execute<AType>("3 each 5 6");
        }
    }
}
