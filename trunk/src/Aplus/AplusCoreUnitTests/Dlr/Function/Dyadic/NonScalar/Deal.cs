using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Dyadic.NonScalar
{
    [TestClass]
    public class Deal : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        public void DealInteger2Integer1()
        {
            AType result = this.engine.Execute<AType>("10 rand 10");
            TestDuplication(result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        public void DealInteger2Integer1Uni()
        {
            AType result = this.engineUni.Execute<AType>("10 M.? 10");
            TestDuplication(result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        public void DealInteger2Integer2()
        {
            AType result = this.engine.Execute<AType>("5 rand 20");
            TestDuplication(result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        public void DealInteger2Integer3()
        {
            AType expected = Utils.ANull(ATypes.AInteger);

            AType result = this.engine.Execute<AType>("0 rand 20");

            Assert.AreEqual<AType>(expected, result, "Incorrect result created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        public void DealInteger2Integer5()
        {
            AType result = this.engine.Execute<AType>("1000 rand 1000");
            TestDuplication(result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DealTypeError1()
        {
            AType result = this.engine.Execute<AType>("2.3 rand 7");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void DealTypeError2()
        {
            AType result = this.engine.Execute<AType>("5 rand 9.3");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DealDomainError1()
        {
            AType result = this.engine.Execute<AType>("() rand 10");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DealDomainError2()
        {
            AType result = this.engine.Execute<AType>("5 rand ()");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DealDomainError3()
        {
            AType result = this.engine.Execute<AType>("3 5 rand 30");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DealDomainError4()
        {
            AType result = this.engine.Execute<AType>("10 rand -30");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Deal"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void DealDomainError5()
        {
            AType result = this.engine.Execute<AType>("20 rand 10");
        }

        private static void TestDuplication(AType argument)
        {
            HashSet<int> duplicates = new HashSet<int>();

            foreach (AType item in argument)
            {
                int num = item.asInteger;

                if (duplicates.Contains(num))
                {
                    Assert.Fail("Result contains duplicate items!");
                }

                duplicates.Add(num);
            }
        }
    }
}
