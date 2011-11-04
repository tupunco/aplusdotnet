using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.Hosting;

using AplusCore.Compiler;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr.Operator
{
    [TestClass]
    public class UserDefinedOperator : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), TestMethod]
        public void SimpleMonadicOperatorCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(f op)b: 5", scope);

            AType result = this.engine.Execute<AType>("+ op 2", scope);

            Assert.AreEqual<AType>(AInteger.Create(5), result, "Operator call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), TestMethod]
        public void ComplexMonadicOperatorCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(f op)b: { f{b;3} }", scope);

            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(5), AInteger.Create(6), AInteger.Create(7)
            );
            AType result = this.engine.Execute<AType>("+ op 2 3 4", scope);

            Assert.AreEqual<AType>(expected, result, "Operator call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), TestCategory("Infix"), TestMethod]
        public void InfixMonadicOperatorCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(f op)b: { f b }", scope);

            AType expected = AArray.Create(ATypes.AInteger,
                AInteger.Create(-2), AInteger.Create(-3), AInteger.Create(-4)
            );
            AType result = this.engine.Execute<AType>("- op 2 3 4", scope);

            Assert.AreEqual<AType>(expected, result, "Operator call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), TestMethod]
        public void SimpleDyadicOperatorCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(f op h)b: 5", scope);

            AType result = this.engine.Execute<AType>("+ op - 2", scope);

            Assert.AreEqual<AType>(AInteger.Create(5), result, "Operator call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), TestMethod]
        public void SimpleDyadicOperatorCallWithDyadicFunction()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a (f op h) b: f{a;b} + h", scope);

            AType result = this.engine.Execute<AType>("3 (+ op -1) 2", scope);

            Assert.AreEqual<AType>(AInteger.Create(4), result, "Function call made incorrect calculation");
        }

        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void WrongDyadicOperatorCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("(f op h)b: { f{b;3} }", scope);

            AType result = this.engine.Execute<AType>("+ op 2 3 4 -", scope);
        }

        [TestCategory("DLR"), TestCategory("UserDefinedOperator"), TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void MissedDyadicOperatorArgumentCall()
        {
            ScriptScope scope = this.engine.CreateScope();
            this.engine.Execute<AType>("a(f op h)b: { f{b;3} }", scope);

            AType result = this.engine.Execute<AType>("5 + op 2 3", scope);
        }
    }
}
