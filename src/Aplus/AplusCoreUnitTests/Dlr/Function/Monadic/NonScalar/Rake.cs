using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;
using AplusCore.Runtime;
using Microsoft.Scripting.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Rake : AbstractTest
    {
        AType a = AFunc.Create(
            "a",
            (Func<Scope, AType,AType, AType>)((scope, x,y) =>
            {
                return AInteger.Create(x.asInteger + y.asInteger);
            }),
            2,
            "add 2 number"
        );

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeVector()
        {
            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(
                    AArray.Create(
                        ATypes.AInteger,
                        AInteger.Create(0),
                        AInteger.Create(1),
                        AInteger.Create(2),
                        AInteger.Create(3)
                    )
                )
            );
            AType result = this.engine.Execute<AType>("in iota 4");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeNull()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("in ()");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeStrandWithUDF1()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("a", a);

            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AInteger.Create(5),
                    a
                }
            );

            AType result = this.engine.Execute<AType>("in (a;5)", scriptscope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeStrandWithUDF2()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("a", a);

            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AInteger.Create(8),
                    AInteger.Create(7),
                    AInteger.Create(5),
                    AInteger.Create(2),
                    Helpers.BuildString("abc"),
                    ABox.Create(a)
                }
            );

            AType result = this.engine.Execute<AType>("in (<<<< {a}; (;'abc';2 2 rho (2;5;7;8)))", scriptscope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeStrandDepth3()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);
            AType g = this.engine.Execute<AType>("g := {-}", scope);

            AType expected = AArray.Create(
                ATypes.ASymbol,
                ASymbol.Create("a"),
                ABox.Create(AInteger.Create(3)),
                ABox.Create(AInteger.Create(2)),
                ABox.Create(f),
                ABox.Create(AInteger.Create(6)),
                ABox.Create(g),
                ASymbol.Create("b")
            );

            AType result = this.engine.Execute<AType>("in `a, (3;(2;(f;);6);g;) , `b", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeStrandDepth4()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AArray.Create(ATypes.AInteger, AInteger.Create(7),AInteger.Create(8)),
                    AInteger.Create(6),
                    AInteger.Create(5),
                    AArray.Create(ATypes.AInteger, AInteger.Create(3),AInteger.Create(4)),
                    AArray.Create(ATypes.AInteger, AInteger.Create(1),AInteger.Create(2)),
                    Helpers.BuildString("ac"),
                    AInteger.Create(4)
                }
            );

            AType result = this.engine.Execute<AType>("in (4;'ac';1 2;(3 4;(5;<(););6);7 8)");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeSimpleArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(
                    AArray.Create(
                        ATypes.ASymbol,
                        ASymbol.Create("a"),
                        ASymbol.Create("b"),
                        f
                    )
                )
            );

            AType result = this.engine.Execute<AType>("in `a`b , f", scope);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Rake"), TestMethod]
        public void RakeStrandWithOnlyNull()
        {
            AType expected = Utils.ANull();

            AType result = this.engine.Execute<AType>("in (;<<();;<<())");

            Assert.AreEqual(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

    }
}
