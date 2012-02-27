using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Matrixinverse : AbstractTest
    {
        #region CorrectCases

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        public void Rank0Test()
        {
            AType expected = AFloat.Create(0.5);
            AType result = this.engine.Execute<AType>("mdiv 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        public void Rank1Test()
        {
            AType expected = AArray.Create(ATypes.AFloat, AFloat.Create(0.125), AFloat.Create(0.125), AFloat.Create(0.125), AFloat.Create(0.125));
            AType result = this.engine.Execute<AType>("mdiv 2 2 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        public void Rank2Test()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                                           AArray.Create(ATypes.AFloat, AFloat.Create(1), AFloat.Create(-1)),
                                           AArray.Create(ATypes.AFloat, AFloat.Create(-0.5), AFloat.Create(1))
                                           );
            AType result = this.engine.Execute<AType>("mdiv 2 2 rho 2 2 1 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        public void PseudoInverseTest()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                                           AArray.Create(ATypes.AFloat, AFloat.Create(-1.3333), AFloat.Create(-0.3333), AFloat.Create(0.6667)),
                                           AArray.Create(ATypes.AFloat, AFloat.Create(1.0833), AFloat.Create(0.3333), AFloat.Create(-0.4167))
                                           );

            AType result = this.engine.Execute<AType>("mdiv 3 2 rho 1 2 3 4 5 6");

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[i][j] = AFloat.Create(Math.Round(result[i][j].asFloat, 4));
                }
            }
            
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Error Cases

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void WrongTypeExeption()
        {
            this.engine.Execute<AType>("mdiv `a");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void ZeroVector()
        {
            this.engine.Execute<AType>("mdiv 0 0 0 0");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void WrongRankExeption()
        {
            this.engine.Execute<AType>("mdiv 3 3 3 rho 1");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void MoreRowsThanColumnsMatrix()
        {
            this.engine.Execute<AType>("mdiv iota 2 3");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void SingularCase()
        {
            this.engine.Execute<AType>("mdiv 2 2 rho 1");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void SingularNotSquareMatrixCase1()
        {
            this.engine.Execute<AType>("mdiv 5 4 rho 1 0 0 0 0 0 0 4 0 3 0 0 0 0 0 0 2 0 0 0");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("MatrixInverse"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void SingularNotSquareMatrixCase2()
        {
            this.engine.Execute<AType>("mdiv 4 3 rho 2 -4 5 6 0 3 2 -4 5 6 0 3");
        }

        #endregion
    }
}
