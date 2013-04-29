using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;

namespace AplusCoreUnitTests.Dlr.Function.Monadic.NonScalar
{
    [TestClass]
    public class Enclose : AbstractTest
    {
        AType udf = AFunc.Create(
                "a",
                (Func<Scope, AType, AType>)((scope, x) =>
                {
                    return x.Clone();
                }),
                2,
                "identity"
            );

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Enclose"), TestMethod]
        public void SimpleEnclose()
        {
            AType expected = ABox.Create(AInteger.Create(1));

            AType result = this.engine.Execute<AType>("< 1");

            Assert.AreEqual<AType>(expected, result, "Incorrect boxed value was created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Enclose"), TestMethod]
        public void SimpleEncloseUni()
        {
            AType expected = ABox.Create(AInteger.Create(1));

            AType result = this.engineUni.Execute<AType>("< 1");

            Assert.AreEqual<AType>(expected, result, "Incorrect boxed value was created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Enclose"), TestMethod]
        public void VectorEnclose()
        {
            AType expected = ABox.Create(AArray.Create(ATypes.AInteger, AInteger.Create(1), AInteger.Create(2)));

            AType result = this.engine.Execute<AType>("< 1 2");

            Assert.AreEqual<AType>(expected, result, "Incorrect boxed value was created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Enclose"), TestMethod]
        public void MultipleFloatEnclose()
        {
            AType expected = ABox.Create(ABox.Create(AFloat.Create(2.2)));

            AType result = this.engine.Execute<AType>("< < 2.2");

            Assert.AreEqual<AType>(expected, result, "Incorrect boxed value was created");
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Enclose"), TestMethod]
        public void EncloseUserDefinedFunction()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("a", udf);

            AType expected = ABox.Create(udf, ATypes.AFunc);

            AType result = this.engine.Execute<AType>("<{a}", scriptscope);

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }

        [TestCategory("DLR"), TestCategory("Monadic"), TestCategory("Enclose"), TestMethod]
        public void EncloseTwiceUserDefinedFunction()
        {
            ScriptScope scriptscope = this.engine.CreateScope();
            scriptscope.SetVariable("a", udf);

            AType expected = ABox.Create(ABox.Create(udf,ATypes.AFunc));

            AType result = this.engine.Execute<AType>("< <{a}", scriptscope);

            Assert.AreEqual<AType>(expected, result);
            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
        }
    }
}
