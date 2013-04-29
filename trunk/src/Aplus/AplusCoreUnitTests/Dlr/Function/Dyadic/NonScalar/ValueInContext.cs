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
    public class ValueInContext : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Value In Context"), TestMethod]
        public void ValueInContextUnqualified()
        {
            AType x = AInteger.Create(100);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable(".x", x);

            AType result = this.engine.Execute<AType>("`. ref `x", scope);

            Assert.AreEqual<AType>(x, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Value In Context"), TestMethod]
        public void ValueInContextUnqualifiedUni()
        {
            AType x = AInteger.Create(100);

            ScriptScope scope = this.engineUni.CreateScope();
            scope.SetVariable(".x", x);

            AType result = this.engineUni.Execute<AType>("`. ^ `x", scope);

            Assert.AreEqual<AType>(x, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Value In Context"), TestMethod]
        public void ValueInContextQualified()
        {
            AType x = AInteger.Create(100);

            ScriptScope scope = this.engine.CreateScope();
            scope.SetVariable("Q.x", x);

            AType result = this.engine.Execute<AType>("`. ref `Q.x", scope);

            Assert.AreEqual<AType>(x, result);
        }

        [TestCategory("DLR"), TestCategory("Callback"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void ValueInContextSymbolVectorAssign()
        {
            ScriptScope scope = this.engine.CreateScope();

            this.engine.Execute("(`ctx `ctx2 ref `b) := 3", scope);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Value In Context"), TestMethod]
        [ExpectedException(typeof(Error.Value))]
        public void ValueInContextErrorValue()
        {
            // The requested 'x' does not exists
            this.engine.Execute<AType>("`a ref `x");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Value In Context"), TestMethod]
        [ExpectedException(typeof(Error.Value))]
        public void ValueInContextErrorQualified()
        {
            // The requested 'a.b.c' is not a qualified name
            this.engine.Execute<AType>("`X ref `a.b.c");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Value In Context"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ValueInContextErrorTypeRight()
        {
            this.engine.Execute<AType>("`a ref 100");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Value In Context"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void ValueInContextErrorTypeLeft()
        {
            this.engine.Execute<AType>("100 ref `a");
        }

    }
}
