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
    public class Raze : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeStrandWithCharacterConstant()
        {
            AType expected = Helpers.BuildString("abcdef");

            AType result = this.engine.Execute<AType>("pick ('ab';'cde';'f')");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeStrandWithCharacterConstantUni()
        {
            AType expected = Helpers.BuildString("abcdef");

            AType result = this.engineUni.Execute<AType>("I.> ('ab';'cde';'f')");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeStrandWithMatrix()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4), AInteger.Create(5)),
                AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(10), AInteger.Create(20)),
                AArray.Create(ATypes.AInteger, AInteger.Create(30), AInteger.Create(40), AInteger.Create(50))
            );

            AType result = this.engine.Execute<AType>("pick (iota 2 3;10 * iota 2 3)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeOneElementNestedVector()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("test")
            );

            AType result = this.engine.Execute<AType>("pick 1 rho < `test");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeMixedIntegerAndFloat()
        {
            AType expected = AArray.Create(
                ATypes.AFloat,
                AFloat.Create(3),
                AFloat.Create(2),
                AFloat.Create(4.5),
                AFloat.Create(7),
                AFloat.Create(9)
            );

            AType result = this.engine.Execute<AType>("pick (3 2; 4.5; 7 9)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeStrandWithNull()
        {
            AType expected = Helpers.BuildString("test");

            AType result = this.engine.Execute<AType>("pick ('te';;'s';;;'t')");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeStrandWithFloatNullAndIntegerList()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(3),
                AInteger.Create(5),
                AInteger.Create(7)
            );

            AType result = this.engine.Execute<AType>("pick (0 rho 3.5; 3 5; 7)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeStrandWithNulls()
        {
            AType expected = AArray.Create(ATypes.AFloat);

            AType result = this.engine.Execute<AType>("pick (0 rho 3.5; ; ; ; ;)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeStrandFirstFloatNull()
        {
            AType expected = AArray.Create(ATypes.AFloat,
                AFloat.Create(3.3), AFloat.Create(4)
            );

            AType result = this.engine.Execute<AType>("pick (0 rho 3.5;3.3;4;)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeNestedScalar()
        {
            AType expected = Helpers.BuildString("abc");

            AType result = this.engine.Execute<AType>("pick < 'abc'");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeSimpleVector()
        {
            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("s1"),
                ASymbol.Create("s2")
            );

            AType result = this.engine.Execute<AType>("pick `s1`s2");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeSimpleMixedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("a"),
                ASymbol.Create("b"),
                f
            );

            AType result = this.engine.Execute<AType>("pick `a`b , f", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        public void RazeNestedMixedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.AFunc,
                f,
                ASymbol.Create("a"),
                ABox.Create(AInteger.Create(2)),
                ABox.Create(AInteger.Create(6)),
                ABox.Create(AInteger.Create(7))
            );

            AType result = this.engine.Execute<AType>("pick (f;;`a;(2;6);<7)", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void RazeTypeError1()
        {
            AType result = this.engine.Execute<AType>("pick ('abc'; 4 5)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RazeRankError1()
        {
            AType result = this.engine.Execute<AType>("pick (32 7; iota 3 2)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RazeRankError2()
        {
            AType result = this.engine.Execute<AType>("pick (; iota 3 2)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void RazeRankError3()
        {
            AType result = this.engine.Execute<AType>("pick 2 2 rho (2;6;3;7)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        [ExpectedException(typeof(Error.Mismatch))]
        public void RazeMismatchError1()
        {
            AType result = this.engine.Execute<AType>("pick (iota 3 2; iota 2 2)");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void RazeDomainError1()
        {
            AType result = this.engine.Execute<AType>("pick (<4) , <{+}");
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Raze"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void RazeDomainError2()
        {
            AType result = this.engine.Execute<AType>("pick `a , (3;45)");
        }
    }
}
