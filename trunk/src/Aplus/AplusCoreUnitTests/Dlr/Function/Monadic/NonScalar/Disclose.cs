using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Disclose : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedScalar()
        {
            AType expected = AInteger.Create(6);

            AType result = this.engine.Execute<AType>("> < 6");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedScalarUni()
        {
            AType expected = AInteger.Create(6);

            AType result = this.engineUni.Execute<AType>("> < 6");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedUniformIntegerVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(4),
                    AInteger.Create(2),
                    AInteger.Create(1)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(4),
                    AInteger.Create(5),
                    AInteger.Create(6)
                )
            );

            AType result = this.engine.Execute<AType>(">(4 2 1; 4 5 6)");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedUniformIntegerVectorUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(4),
                    AInteger.Create(2),
                    AInteger.Create(1)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(4),
                    AInteger.Create(5),
                    AInteger.Create(6)
                )
            );

            AType result = this.engineUni.Execute<AType>(">(4 2 1; 4 5 6)");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedUniformFloatVector1()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(4.5),
                    AFloat.Create(7)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(4),
                    AFloat.Create(6)
                )
            );

            AType result = this.engine.Execute<AType>(">(4.5 7; 4 6)");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedUniformFloatVector2()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(4),
                    AFloat.Create(7)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(4),
                    AFloat.Create(6)
                ),
                AArray.Create(
                    ATypes.AFloat,
                    AFloat.Create(4.3),
                    AFloat.Create(0)
                )
            );

            AType result = this.engine.Execute<AType>(">(4 7; 4 6; 4.3 0)");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedUniformSymbolConstantVector()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("s1"),
                ASymbol.Create("s2")
            );

            AType result = this.engine.Execute<AType>(">(`s1; `s2)");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedUniformIntegerMarix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(5)
                    ),
                     AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(5)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(5)
                    )
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(5)
                    ),
                     AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(5)
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(3),
                        AInteger.Create(5)
                    )
                )
            );

            AType result = this.engine.Execute<AType>("> 2 3 rho < 3 5");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedArray()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(5), AInteger.Create(3)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(5), AInteger.Create(2))
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(7), AInteger.Create(8), AInteger.Create(5)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(4), AInteger.Create(6))
                )
            );

            AType result = this.engine.Execute<AType>("> 2 2 rho (< 2 5 3) , ( < 1 5 2) , (< 7 8 5), < 2 4 6");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNestedNull()
        {
            AType expected = AArray.Create(ATypes.ANull);

            AType result = this.engine.Execute<AType>("> <()");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseSimpleFloatVector()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(4),
                AFloat.Create(2.3),
                AFloat.Create(6.5)
            );

            AType result = this.engine.Execute<AType>("> 4 2.3 6.5");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseNull()
        {
            AType expected = AArray.Create(ATypes.ANull);

            AType result = this.engine.Execute<AType>("> ()");

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseFunctionScalar()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected = this.engine.Execute<AType>("f := <{+}", scope);

            AType result = this.engine.Execute<AType>("> f",scope);

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseEnclosedFunctionScalar()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected = this.engine.Execute<AType>("f := <{+}", scope);
            this.engine.Execute<AType>("f := < f", scope);

            AType result = this.engine.Execute<AType>("> f", scope);

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseTwiceEnclosedFunctionScalar()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType expected = this.engine.Execute<AType>("f := <<{*}", scope);
            this.engine.Execute<AType>("f := < f", scope);

            AType result = this.engine.Execute<AType>("> f", scope);

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        public void DiscloseMixedNestedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType a = this.engine.Execute<AType>("a := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("a"),
                a,
                ABox.Create(AInteger.Create(4))
            );

            AType result = this.engine.Execute<AType>(">(`a;a;<4)", scope);

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DiscloseTypeError1()
        {
            AType result = this.engine.Execute<AType>(">(3 2; 'abc')");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DiscloseRankError1()
        {
            AType result = this.engine.Execute<AType>(">(iota 3 2; 67 5)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void DiscloseRankError2()
        {
            AType result = this.engine.Execute<AType>(">(3.5; 67 5)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        [ExpectedException(typeof(Error.Mismatch))]
        public void DiscloseMismatchError1()
        {
            AType result = this.engine.Execute<AType>(">(7 3 2; 67 5)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DiscloseDomainError1()
        {
            AType result = this.engine.Execute<AType>(">{(<{+}) , < 4}");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Disclose"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DiscloseDomainError2()
        {
            AType result = this.engine.Execute<AType>(">{(< 5) , `a}");
        }
    }
}
