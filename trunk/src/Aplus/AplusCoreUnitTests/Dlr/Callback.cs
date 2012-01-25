using System;
using Microsoft.Scripting.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    [TestClass]
    public class Callback : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void BasicCallbackDefinition()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; (f;'fuuu')}", scope);

            Aplus runtime = this.engine.GetService<Aplus>();

            Assert.IsTrue(runtime.CallbackManager.Contains(".a"), "Callback not found.");
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void BasicCallbackInvoke()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>(
                "f{static; newvalue}: {" +
                "drop(static; newvalue);" +
                "if(newvalue~=-4) { take `incorrectNewValue };" +
                "if((static=='fuuu') == 0) { take `incorrectStaticData }" +
                "}",
                scope);
            this.engine.Execute<AType>("_scb{`a; (f;'fuuu')}", scope);
            this.engine.Execute<AType>("a:=-4", scope);
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void GetCallbackDefinition()
        {
            AType expected = Helpers.BuildStrand(
                new AType[] { Helpers.BuildString("fuuu"), Helpers.BuildString(".f") }
            );

            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; (f;'fuuu')}", scope);

            AType result = this.engine.Execute<AType>("_gcb{`a}");

            Assert.AreEqual<AType>(expected, result, "Incorrect callback info returned.");
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void CallbackNotFound()
        {
            AType expected = Utils.ANull();
            AType result = this.engine.Execute<AType>("_gcb{`a}");

            Assert.AreEqual<AType>(expected, result);
        }

        #region Callbacks

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void SimpleAssignCallback()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create(""),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(3),
                        Helpers.BuildString("static")
                    }
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("b := 3", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void SimpleIndexAssignCallback()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create(""),
                        Utils.ANull(),
                        AInteger.Create(1),
                        AInteger.Create(200),
                        Helpers.BuildString("static")
                    }
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("b := 10 20 30 40", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("b[1] := 200", scope);

            AType result = scope.GetVariable<AType>(".result");

            Assert.AreEqual(expected.CompareInfos(result), InfoResult.OK);
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void AppendAssignCallback()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create(""),
                        Utils.ANull(),
                        ABox.Create(AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))),
                        AArray.Create(ATypes.AInteger, AInteger.Create(3),AInteger.Create(4)),
                        Helpers.BuildString("static")
                    }
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("b := 1 2", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("b[,] := 3 4", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void PickChooseExample()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        ASymbol.Create("matrix"),
                        Helpers.BuildStrand(new AType[]{ AInteger.Create(0), AInteger.Create(1) }),
                        AInteger.Create(22),
                        Helpers.BuildString("static")
                    }
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx", scope);
            this.engine.Execute("b := (`scalar `vector `matrix; (3.14; 'abcdef'; iota 3 2))", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("((1;0)#`matrix pick b) := 22", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void SelectiveAssignment()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        AInteger.Create(0),
                        ABox.Create(AArray.Create(ATypes.ASymbol, ASymbol.Create("Scalar"), ASymbol.Create("Vector"), ASymbol.Create("Matrix"))),
                        Helpers.BuildString("static")
                    }
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx", scope);
            this.engine.Execute("b := (`scalar `vector `matrix; (3.14; 'abcdef'; iota 3 2))", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("(1 0 /b) := <`Scalar `Vector `Matrix", scope);

            Assert.AreEqual(expected.CompareInfos(scope.GetVariable<AType>(".result")), InfoResult.OK);
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void ValueInExample()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(3),
                        Helpers.BuildString("static")
                    }
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx");
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("(ref `ctx.b) := 3", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void ValueInContextExample()
        {
            AType expected =
               Helpers.BuildStrand(
                   new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(3),
                        Helpers.BuildString("static")
                    }
               );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx");
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("(`ctx ref `b) := 3", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void ValueInContextExample2()
        {
            AType expected =
               Helpers.BuildStrand(
                   new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(3),
                        Helpers.BuildString("static")
                    }
               );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx");
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f)}", scope);
            this.engine.Execute("_scb{`b;(cbf;'static')}", scope);
            this.engine.Execute("(`ctx ref `ctx.b) := 3", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        #endregion

        #region Preset callbacks

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void InterruptAssign()
        {
            AType expected = AInteger.Create(5);

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("b := 5", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{take `signal}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("b := 3", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".b"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void SimpleAssignPresetCallback()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create(""),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(3),
                        Helpers.BuildString("static")
                    }
                );

            AType expectedValue = AInteger.Create(3);

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("b := 3", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
            Assert.AreEqual(expectedValue, scope.GetVariable<AType>(".b"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void SimpleIndexAssignPresetCallback()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create(""),
                        Utils.ANull(),
                        AInteger.Create(1),
                        AInteger.Create(200),
                        Helpers.BuildString("static")
                    }
                );

            AType expectedValue =
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(10),
                    AInteger.Create(200),
                    AInteger.Create(30),
                    AInteger.Create(40)
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("b := 10 20 30 40", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("b[1] := 200", scope);

            AType result = scope.GetVariable<AType>(".result");

            Assert.AreEqual(expected.CompareInfos(result), InfoResult.OK);
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
            Assert.AreEqual(expectedValue, scope.GetVariable<AType>(".b"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void AppendAssignPresetCallback()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create(""),
                        Utils.ANull(),
                        ABox.Create(AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3))),
                        AArray.Create(ATypes.AInteger, AInteger.Create(3),AInteger.Create(4)),
                        Helpers.BuildString("static")
                    }
                );

            AType expectedValue =
                AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(2), AInteger.Create(3), AInteger.Create(4));

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("b := 1 2", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("b[,] := 3 4", scope);

            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
            Assert.AreEqual(expectedValue, scope.GetVariable<AType>(".b"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void PickChoosePresetExample()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        ASymbol.Create("matrix"),
                        Helpers.BuildStrand(new AType[]{ AInteger.Create(0), AInteger.Create(1) }),
                        AInteger.Create(22),
                        Helpers.BuildString("static")
                    }
                );

            AType expectedValue =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        Helpers.BuildStrand(
                            new AType[]
                            {
                                AArray.Create(
                                    ATypes.AInteger,
                                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                                    AArray.Create(ATypes.AInteger, AInteger.Create(22), AInteger.Create(3)),
                                    AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(5))
                                ),
                                Helpers.BuildString("abcdef"),
                                AFloat.Create(3.14)
                            }
                        ),
                        AArray.Create(ATypes.ASymbol, ASymbol.Create("scalar"), ASymbol.Create("vector"), ASymbol.Create("matrix"))
                    }
                );


            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx", scope);
            this.engine.Execute("b := (`scalar `vector `matrix; (3.14; 'abcdef'; iota 3 2))", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("((1;0)#`matrix pick b) := 22", scope);

            Assert.AreEqual(expectedValue, scope.GetVariable<AType>("ctx.b"));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void SelectiveAssignmentPreset()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        AInteger.Create(0),
                        ABox.Create(AArray.Create(ATypes.ASymbol, ASymbol.Create("Scalar"), ASymbol.Create("Vector"), ASymbol.Create("Matrix"))),
                        Helpers.BuildString("static")
                    }
                );

            AType expectedValue =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        Helpers.BuildStrand(
                            new AType[]
                            {
                                AArray.Create(
                                    ATypes.AInteger,
                                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1)),
                                    AArray.Create(ATypes.AInteger, AInteger.Create(2), AInteger.Create(3)),
                                    AArray.Create(ATypes.AInteger, AInteger.Create(4), AInteger.Create(5))
                                ),
                                Helpers.BuildString("abcdef"),
                                AFloat.Create(3.14)
                            }
                        ),
                        AArray.Create(ATypes.ASymbol, ASymbol.Create("Scalar"), ASymbol.Create("Vector"), ASymbol.Create("Matrix"))
                    }
                );

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx", scope);
            this.engine.Execute("b := (`scalar `vector `matrix; (3.14; 'abcdef'; iota 3 2))", scope);
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("(1 0 /b) := <`Scalar `Vector `Matrix", scope);

            Assert.AreEqual(expected.CompareInfos(scope.GetVariable<AType>(".result")), InfoResult.OK);
            Assert.AreEqual(expectedValue, scope.GetVariable<AType>("ctx.b"));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void ValueInPresetExample()
        {
            AType expected =
                Helpers.BuildStrand(
                    new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(3),
                        Helpers.BuildString("static")
                    }
                );

            AType expectedValue = AInteger.Create(3);

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx");
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("(ref `ctx.b) := 3", scope);

            Assert.AreEqual(expectedValue, scope.GetVariable<AType>("ctx.b"));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void ValueInContextPresetExample()
        {
            AType expected =
               Helpers.BuildStrand(
                   new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(3),
                        Helpers.BuildString("static")
                    }
               );

            AType expectedValue = AInteger.Create(3);

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx");
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("(`ctx ref `b) := 3", scope);

            Assert.AreEqual(expectedValue, scope.GetVariable<AType>("ctx.b"));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        public void DependencyPresetCallback()
        {
            AType expected =
               Helpers.BuildStrand(
                   new AType[]
                    {
                        ASymbol.Create("b"),
                        ASymbol.Create("ctx"),
                        Utils.ANull(),
                        Utils.ANull(),
                        AInteger.Create(8),
                        Helpers.BuildString("static")
                    }
               );

            AType expectedValue = AInteger.Create(8);

            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("$cx ctx");
            this.engine.Execute("cbf{a;b;c;d;e;f}:{.result := (a;b;c;d;e;f); := b}", scope);
            this.engine.Execute("_spcb{`b;(cbf;'static')}", scope);
            this.engine.Execute("a := 3", scope);
            this.engine.Execute("b:2*a", scope);
            this.engine.Execute("a := 4", scope);
            this.engine.Execute("b", scope);

            Assert.AreEqual(expectedValue, scope.GetVariable<AType>("ctx.b"));
            Assert.AreEqual(expected, scope.GetVariable<AType>(".result"));
        }

        #endregion

        #region Errors

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void GlobalNameCallbackError()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{'woo'; (f;'fuuu')}", scope);
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void IncorrectTypeCallbackError()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; 'ab'}", scope);
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        [ExpectedException(typeof(Error.NonData))]
        public void IncorrectArgumentCallbackError()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("f{}: drop 'test'", scope);
            this.engine.Execute<AType>("_scb{`a; f}", scope);
        }

        #endregion
    }
}
