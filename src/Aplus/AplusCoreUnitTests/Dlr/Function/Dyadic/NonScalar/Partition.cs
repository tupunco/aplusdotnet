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
    public class Partition : AbstractTest
    {
        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionIntegerList2Matrix()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                   AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(9), AInteger.Create(10), AInteger.Create(11)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(12), AInteger.Create(13), AInteger.Create(14))
                    ),
                    AArray.Create(
                        ATypes.AInteger,
                        AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1), AInteger.Create(2)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(3), AInteger.Create(4), AInteger.Create(5)),
                        AArray.Create(ATypes.AInteger, AInteger.Create(6), AInteger.Create(7), AInteger.Create(8))
                    )
                }
            );
            
            AType result = this.engine.Execute<AType>("3 2 bag iota 5 3");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionIntegerList2Vector()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AArray.Create(ATypes.AInteger),
                    AArray.Create(ATypes.AInteger),
                    AArray.Create(ATypes.AInteger, AInteger.Create(0), AInteger.Create(1))
                }
            );

            AType result = this.engine.Execute<AType>("4 2 1 bag iota 2");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionInteger2Strand1()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    Helpers.BuildStrand(
                        new AType[]{
                            AArray.Create(ATypes.ANull),
                            AInteger.Create(8)
                        }
                    ),
                    Helpers.BuildStrand(
                        new AType[]{
                            AInteger.Create(5),
                            AInteger.Create(4),
                            AInteger.Create(3)
                        }
                    )
                }
            );

            AType result = this.engine.Execute<AType>("3 bag (3;4;5;8;)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionInteger2Strand2()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AArray.Create(ATypes.ANull),
                    AArray.Create(ATypes.ANull),
                    Helpers.BuildStrand(
                        new AType[]{
                            AInteger.Create(4),
                            AInteger.Create(3)
                        }
                    )
                }
            );

            AType result = this.engine.Execute<AType>("2 3 1 bag (3;4)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionIntegerList2Null()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                   AArray.Create(ATypes.ANull),
                   AArray.Create(ATypes.ANull),
                   AArray.Create(ATypes.ANull)
                }
            );

            AType result = this.engine.Execute<AType>("3 2 2 bag ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionFloat2Null()
        {
            AType expected = AArray.Create(ATypes.ANull);

            AType result = this.engine.Execute<AType>("3.0000000000000005 bag ()");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionIntegerList2SymbolConstantList()
        {
            AType expected = Helpers.BuildStrand(
                new AType[]{
                    AArray.Create(ATypes.ASymbol, ASymbol.Create("s3"), ASymbol.Create("s4")),
                    AArray.Create(ATypes.ASymbol),
                    AArray.Create(ATypes.ASymbol, ASymbol.Create("s1"), ASymbol.Create("s2"))
                }
            );

            AType result = this.engine.Execute<AType>("2 0 2 bag `s1`s2`s3`s4`s5");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionNull2FloatList()
        {
            AType expected = AArray.Create(ATypes.ANull);

            AType result = this.engine.Execute<AType>("() bag 3.4 5.3 6.7");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionInteger2Strand()
        {
            AType expected = ABox.Create(AArray.Create(ATypes.ANull));

            AType result = this.engine.Execute<AType>("0 bag (4;3;4)");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionInteger2SymbolArray()
        {
            AType expected = AArray.Create(
                ATypes.ANull
            );

            expected.Length = 0;
            expected.Shape = new List<int>() { 0, 2, 2 };
            expected.Rank = 3;

            expected = ABox.Create(expected);

            AType result = this.engine.Execute<AType>("0 bag 3 2 2 rho `a");

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        public void PartitionIntegerList2MixedNestedArray()
        {
            ScriptScope scope = this.engine.CreateScope();
            AType f = this.engine.Execute<AType>("f := <{+}", scope);

            AType expected = AArray.Create(
                ATypes.ABox,
                ABox.Create(
                    AArray.Create(
                        ATypes.ASymbol,
                        ASymbol.Create("a"),
                        ABox.Create(AInteger.Create(4))
                    )
                ),
                ABox.Create(
                    AArray.Create(
                        ATypes.AFunc,
                        f
                    )
                ),
                ABox.Create(Utils.ANull())
            );

            AType result = this.engine.Execute<AType>("2 2 1 bag `a , (< 4) , f", scope);

            Assert.AreEqual(InfoResult.OK, result.CompareInfos(expected));
            Assert.AreEqual(expected, result);
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        [ExpectedException(typeof(Error.Type))]
        public void PartitionTypeError()
        {
            AType result = this.engine.Execute<AType>("`test bag 2 4 5");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        [ExpectedException(typeof(Error.Rank))]
        public void PartitionRankError()
        {
            AType result = this.engine.Execute<AType>("3 2 bag 2");
        }

        [TestCategory("DLR"), TestCategory("Dyadic"), TestCategory("Partition"), TestMethod]
        [ExpectedException(typeof(Error.Domain))]
        public void PartitionDomainError()
        {
            AType result = this.engine.Execute<AType>("3 -6 2 bag 2 8");
        }
    }
}
