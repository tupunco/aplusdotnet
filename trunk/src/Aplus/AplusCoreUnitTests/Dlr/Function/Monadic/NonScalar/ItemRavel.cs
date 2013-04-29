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
    public class ItemRavel : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ItemRavel"), TestMethod]
        public void ItemRavelIntegerVector()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engine.Execute<AType>("! iota 2 2");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ItemRavel"), TestMethod]
        public void ItemRavelIntegerVectorUni()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(0),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(3)
            );
            AType result = this.engineUni.Execute<AType>("S.! I.# 2 2");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ItemRavel"), TestMethod]
        public void ItemRavelIntegerMatrixWithFrame()
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
                    AInteger.Create(3),
                    AInteger.Create(4),
                    AInteger.Create(5)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(6),
                    AInteger.Create(7),
                    AInteger.Create(8)
                ),
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(9),
                    AInteger.Create(10),
                    AInteger.Create(11)
                )
            );
            AType result = this.engine.Execute<AType>("! iota 2 2 3");
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ItemRavel"), TestMethod]
        public void ItemRavelMixedMatrix()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType a = this.engine.Execute<AType>("a := <{+}", scope);
            AType b = this.engine.Execute<AType>("b := <{=}", scope);

            AType expected = AArray.Create(
                ATypes.AFunc,
                a,
                ABox.Create(ASymbol.Create("d")),
                b,
                ASymbol.Create("c")
            );

            AType result = this.engine.Execute<AType>("! 2 2 rho a , (<`d) , b, `c", scope);
            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ItemRavel"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ItemRavelRankError1()
        {
            this.engine.Execute<AType>("! 3.5");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ItemRavel"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ItemRavelRankError2()
        {
            this.engine.Execute<AType>("! 'test'");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("ItemRavel"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ItemRavelRankError3()
        {
            this.engine.Execute<AType>("! ()");
        }
    }
}
