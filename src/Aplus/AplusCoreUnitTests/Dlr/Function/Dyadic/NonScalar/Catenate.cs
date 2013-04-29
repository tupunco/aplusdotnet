using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;
using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Catenate : AbstractTest
    {

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Integer2IntegerCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("1, 2");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Integer2IntegerCatenateUni()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(2)
            );

            AType result = this.engineUni.Execute<AType>("1, 2");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Integer2IntegerDifferentLengthCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1), AInteger.Create(2),
                AInteger.Create(3), AInteger.Create(4),
                AInteger.Create(5)
            );

            AType result = this.engine.Execute<AType>("1 2, 3 4 5");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Integer2NullCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(1)
            );

            AType result = this.engine.Execute<AType>("1, ()");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void NullArray2IntegerArrayCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger, 
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4))
            );

            AType result = this.engine.Execute<AType>("(0 2 rho 0), 3 4");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }


        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Integer2VectorCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)
            );

            AType result = this.engine.Execute<AType>("0 1, 2");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Integer2FloatCatenate()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(1), AFloat.Create(2)
            );

            AType result = this.engine.Execute<AType>("1, 2.0");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void BoxCatenate()
        {
            AType expected = this.engine.Execute<AType>("(1;2;3)");

            AType result = this.engine.Execute<AType>("(<1) , (2;3)");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Integer2MatrixCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
            );

            AType result = this.engine.Execute<AType>("1, (iota 2 2)");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Matrix2IntegerCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(1))
            );

            AType result = this.engine.Execute<AType>("(iota 2 2) , 1");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));

        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void Matrix2MatrixCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))
            );

            AType result = this.engine.Execute<AType>("(iota 2 2), (iota 2 2)");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void MultiDim2MultiDimRankDiffCatenate()
        {
            AType expected = AArray.Create(ATypes.AInteger,
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4), AInteger.Create(5))
                ),
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(6), AInteger.Create(7), AInteger.Create(8)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(9), AInteger.Create(10), AInteger.Create(11))
                ),
                AArray.Create(ATypes.AInteger,
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4), AInteger.Create(5))
                )
            );

            AType result = this.engine.Execute<AType>("(iota 2 2 3), (iota 2 3)");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void Vector2MultiDimArrayCatenate()
        {
            this.engine.Execute<AType>("1 2, (iota 2 2 2)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void Integer2CharacterArrayCatenate()
        {
            this.engine.Execute<AType>("1, 'test'");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LengthErrorCatenate()
        {
            this.engine.Execute<AType>("(iota 2 2 3), (iota 2 2 4)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LengthErrorDifferentRankACatenate()
        {
            this.engine.Execute<AType>("(iota 2 2 3), (iota 2 4)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LengthErrorDifferentRankBCatenate()
        {
            this.engine.Execute<AType>("(iota 2 3), (iota 2 2 4)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Catenate"), TestMethod]
        public void CloneTestCatenate()
        {

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute<AType>("a:=iota 2 2 3", scope);

            AType result = this.engine.Execute<AType>("b:=a, (iota 2 3)", scope);

            AType var_a = scope.GetVariable<AType>(".a");
            AType var_b = scope.GetVariable<AType>(".b");
            var_b[0][0][0] = AInteger.Create(100);

            bool checkSideEffect = (var_a[0][0][0].asInteger == 100);

            Assert.IsFalse(checkSideEffect, "Modifing result modified the arguments!");

        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Catenate"), TestMethod]
        public void CatenateMixedArray()
        {
            AType expected = AArray.Create(ATypes.ASymbol,
                ASymbol.Create("a"),
                ASymbol.Create("b"),
                ABox.Create(AInteger.Create(1)),
                ABox.Create(AInteger.Create(3))
            );

            AType result = this.engine.Execute<AType>("`a`b , (1;3)");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

    }
}
