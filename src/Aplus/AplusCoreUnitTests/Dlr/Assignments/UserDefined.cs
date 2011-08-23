using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AplusCore.Types;
using AplusCore.Runtime;

namespace AplusCoreUnitTests.Dlr.Assignments
{
    [TestClass]
    public class UserDefined : AbstractTest
    {

        private static AType TestMethod(Aplus env, AType arg2, AType arg1, AType arg0)
        {
            return AInteger.Create(0);
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("User Defined"), TestMethod]
        public void UserDefined1()
        {
            AType expected = AArray.Create(
                ATypes.AInteger,
                AInteger.Create(-10),
                AInteger.Create(1),
                AInteger.Create(2),
                AInteger.Create(3)
            );

            AType function = AFunc.Create("T", (Func<Aplus, AType, AType, AType, AType>)TestMethod, 4, "Test method");

            var scope = this.engine.CreateScope();
            scope.SetVariable(".T", function);

            scope.SetVariable(
                ".a",
                AArray.Create(
                    ATypes.AInteger,
                    AInteger.Create(0),
                    AInteger.Create(1),
                    AInteger.Create(2),
                    AInteger.Create(3)
                )
            );

            this.engine.Execute<AType>("T{0;1;a} := -10", scope);

            Assert.AreEqual<AType>(expected, scope.GetVariable<AType>(".a"), "Incorrect assignment performed");
        }

        [TestCategory("DLR"), TestCategory("Assign"), TestCategory("User Defined"), TestMethod]
        public void UserDefined2()
        {
            AType expected = AArray.Create(ATypes.ABox,
                    ABox.Create(
                        AArray.Create(
                            ATypes.AInteger,
                            AInteger.Create(-10),
                            AInteger.Create(1),
                            AInteger.Create(2),
                            AInteger.Create(3)
                        )
                    ),
                    ABox.Create(AInteger.Create(0))
                );

            AType function = AFunc.Create("T", (Func<Aplus, AType, AType, AType, AType>)TestMethod, 4, "Test method");

            var scope = this.engine.CreateScope();
            scope.SetVariable(".T", function);
            scope.SetVariable(".a", 
                AArray.Create(ATypes.ABox,
                    ABox.Create(
                        AArray.Create(
                            ATypes.AInteger,
                            AInteger.Create(0),
                            AInteger.Create(1),
                            AInteger.Create(2),
                            AInteger.Create(3)
                        )
                    ),
                    ABox.Create(AInteger.Create(0))
                )
            );

            this.engine.Execute<AType>("T{0;1;0 pick a} := -10", scope);

            Assert.AreEqual<AType>(expected, scope.GetVariable<AType>(".a"), "Incorrect assignment performed");
        }
    }
}
