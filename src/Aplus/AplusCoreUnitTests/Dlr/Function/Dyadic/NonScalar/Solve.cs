using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic
{
    [TestClass]
    public class Solve : AbstractTest
    {
        #region Correct cases

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        public void Rank0Example()
        {
            AType expected = AFloat.Create(0.2);
            AType result = this.engine.Execute<AType>("1 mdiv 5");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        public void Rank0ExampleUni()
        {
            AType expected = AFloat.Create(0.2);
            AType result = this.engineUni.Execute<AType>("1 M.# 5");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        public void Rank2SquareExample()
        {
            AType expected =
                AArray.Create(ATypes.AFloat,
                              AArray.Create(ATypes.AFloat, AFloat.Create(1), AFloat.Create(0)),
                              AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(1))
                              );

            AType result = this.engine.Execute<AType>("(iota 2 2) mdiv iota 2 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        public void Rank2Example()
        {
            AType expected =
                AArray.Create(ATypes.AFloat,
                              AArray.Create(ATypes.AFloat, AFloat.Create(1), AFloat.Create(0)),
                              AArray.Create(ATypes.AFloat, AFloat.Create(0), AFloat.Create(1))
                              );

            AType result = this.engine.Execute<AType>("(iota 3 2) mdiv iota 3 2");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        public void SquareEquationSystem()
        {
            AType expected = AArray.Create(ATypes.AFloat, AFloat.Create(0.5), AFloat.Create(0.0));
            AType result = this.engine.Execute<AType>("1 0 mdiv 2 2 rho 2 1 0 1");

            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        public void OverDeterminedExample1()
        {
            this.engine.Execute("444 458 478 493 506 516 523 531 543 571 mdiv 10 3 rho 1 1 1 1 2 4 1 3 9 1 4 16 1 5 25 1 6 36 1 7 49 1 8 64 1 9 81 1 10 100");
        }

        #endregion

        #region Error Cases

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Length))]
        public void LengthErrorException()
        {
            this.engine.Execute("(2 4 rho 2) mdiv 1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RankErrorException()
        {
            this.engine.Execute("(3 3 3 rho 1) mdiv 1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RankErrorException2()
        {
            this.engine.Execute("1 mdiv (3 3 3 rho 1)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorException()
        {
            this.engine.Execute("(3 3 3 rho 'a') mdiv 1");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void TypeErrorException2()
        {
            this.engine.Execute("1 mdiv (3 3 3 rho 'a')");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainErrorExceptionMoreColumnThanRow()
        {
            this.engine.Execute("(2 3 rho 1.1 2.2 3.3) mdiv (2 3 rho 1.1 2.2 3.3)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainErrorExceptionBecauseOfUnderdeterminedEquation()
        {
            this.engine.Execute("1 2 3 mdiv 3 3 rho 1 2 3 2 4 6 2 3 4");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainErrorExceptionIllConditionedRight()
        {
            this.engine.Execute("(iota 2 2) mdiv (2 2 rho 0)");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Solve"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DomainErrorInfInInput()
        {
            this.engine.Execute("444 458 478 493 506 516 523 531 543 571 mdiv 10 3 rho 1 1 1 1 2 4 1 3 9 1 4 16 1 5 25 1 6 36 1 7 49 1 8 64 1 9 81 1 Inf 100");
        }

        #endregion
    }
}
