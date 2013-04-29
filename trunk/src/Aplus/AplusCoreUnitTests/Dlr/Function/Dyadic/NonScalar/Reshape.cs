using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Reshape : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        public void IntegerCycleReshape()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(5), AInteger.Create(5)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(5), AInteger.Create(5)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(5), AInteger.Create(5))
            );

            AType result = this.engine.Execute<AType>(" 3 3 rho 5 5");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual<AType>(expected, result, "Incorrect integer matrix generated");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        public void IntegerCycleReshapeUni()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(5), AInteger.Create(5)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(5), AInteger.Create(5)),
                AArray.Create(ATypes.AInteger, AInteger.Create(5), AInteger.Create(5), AInteger.Create(5))
            );

            AType result = this.engineUni.Execute<AType>(" 3 3 S.? 5 5");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual<AType>(expected, result, "Incorrect integer matrix generated");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        public void ZeroShapedReshape()
        {
            AType expected = AArray.Create(ATypes.AInteger);
            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2 };
            expected.Rank = expected.Shape.Count;

            AType result = this.engine.Execute<AType>("0 2 rho 1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual<AType>(expected, result, "Incorrect integer matrix generated");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        public void ZeroShaped2Reshape()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger),
                AArray.Create(ATypes.AInteger)
            );

            expected.Length = 2;
            expected.Shape = new List<int>() { 2, 0, 2 };
            expected.Rank = expected.Shape.Count;

            AType result = this.engine.Execute<AType>("2 0 2 rho 1");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual<AType>(expected, result, "Incorrect integer matrix generated");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        public void StringCycleReshape()
        {
            AType expected = AArray.Create(ATypes.AChar,
                AArray.Create(ATypes.AChar, AChar.Create('h'), AChar.Create('e'), AChar.Create('l')),
                AArray.Create(ATypes.AChar, AChar.Create('l'), AChar.Create('o'), AChar.Create('h')),
                AArray.Create(ATypes.AChar, AChar.Create('e'), AChar.Create('l'), AChar.Create('l'))
            );

            AType result = this.engine.Execute<AType>(" 3 3 rho 'hello'");

            Assert.AreEqual<AType>(expected, result, "Incorrect integer matrix generated");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        public void BoxReshape()
        {
            AType expected = AArray.Create(ATypes.ABox,
                AArray.Create(ATypes.ABox, ABox.Create(AInteger.Create(1)), ABox.Create(AInteger.Create(2)), ABox.Create(AInteger.Create(1))),
                AArray.Create(ATypes.ABox, ABox.Create(AInteger.Create(2)), ABox.Create(AInteger.Create(1)), ABox.Create(AInteger.Create(2))),
                AArray.Create(ATypes.ABox, ABox.Create(AInteger.Create(1)), ABox.Create(AInteger.Create(2)), ABox.Create(AInteger.Create(1)))
            );

            Console.WriteLine(expected);
            AType result = this.engine.Execute<AType>(" 3 3 rho (1;2)");
            Console.WriteLine(result);

            Assert.AreEqual<AType>(expected, result, "Incorrect integer matrix generated");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        public void IntegerRavelReshape()
        {
            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".a",
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
                )
            );

            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3), AInteger.Create(0))
            );

            AType result = this.engine.Execute<AType>(" 3 3 rho .a", scope);

            Assert.AreEqual<AType>(expected, result, "Incorrect integer matrix generated");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorReshape()
        {
            this.engine.Execute<AType>(" 'fail' rho 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainErrorReshape()
        {
            this.engine.Execute<AType>(" 1 2 3 4 5 6 7 8 9 10 11 12 13 rho 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Reshape"), TestMethod]
        [ExpectedException(typeof(Error.MaxRank))]
        public void MaxRankErrorReshape()
        {
            this.engine.Execute<AType>(" 1 2 3 4 5 6 7 8 9 10 rho 2 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void CloneTestReshape()
        {

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("a:=1 2", scope);

            AType result = this.engine.Execute<AType>("b:= 2 2 rho a", scope);

            AType var_a = scope.GetVariable<AType>(".a");
            AType var_b = scope.GetVariable<AType>(".b");
            var_a[1] = AInteger.Create(200);
            var_b[0][0] = AInteger.Create(100);

            bool checkSideEffect = (var_a[0].asInteger == 100) || (var_b[0][0].asInteger == 200);

            Assert.IsFalse(checkSideEffect, "Modifing result modified the arguments!");

        }
    }
}
