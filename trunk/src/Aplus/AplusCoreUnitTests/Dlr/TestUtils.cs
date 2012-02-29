using System;
using System.IO;
using System.Linq;

using Microsoft.Scripting.Hosting;

using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCoreUnitTests.Dlr
{
    #region InfoResult

    /// <summary>
    /// Enum for representing errors between two ATypes
    /// </summary>
    public enum InfoResult
    {
        /// <summary>
        /// No information error found while comparing ATypes
        /// </summary>
        OK,

        /// <summary>
        /// ATypes' length does not match
        /// </summary>
        LengthError,

        /// <summary>
        /// ATypes' shape does not match
        /// </summary>
        ShapeError,

        /// <summary>
        /// ATypes' rank does not match
        /// </summary>
        RankError,

        /// <summary>
        /// ATypes' type does not match
        /// </summary>
        TypeError
    }

    #endregion

    public static class TestUtils
    {
        #region Compare

        /// <summary>
        /// Compares the AType's information's to an other AType
        /// </summary>
        /// <param name="other"></param>
        /// <returns>value from InfoResult enum</returns>
        public static InfoResult CompareInfos(this AType actual, AType other)
        {
            if (actual.Length != other.Length)
            {
                return InfoResult.LengthError;
            }

            if (!actual.Shape.SequenceEqual<int>(other.Shape))
            {
                return InfoResult.ShapeError;
            }

            if (actual.Rank != other.Rank)
            {
                return InfoResult.RankError;
            }

            if (actual.Type != other.Type)
            {
                return InfoResult.TypeError;
            }

            if (actual.Rank > 0)
            {
                InfoResult result;

                for (int i = 0; i < actual.Length; i++)
                {
                    result = actual[i].CompareInfos(other[i]);
                    if (result != InfoResult.OK)
                    {
                        return result;
                    }
                }
            }
            else if (actual.Type == ATypes.ABox)
            {
                return actual.NestedItem.CompareInfos(other.NestedItem);
            }

            return InfoResult.OK;
        }

        #endregion

        #region CDR related

        /// <summary>
        /// Function to get byte array from a file
        /// </summary>
        /// <param name="fileName">File name to get byte array</param>
        /// <returns>Byte Array</returns>
        public static byte[] FileToByteArray(string fileName)
        {
            string testCasePath = Path.Combine("Function", "ADAP", "Expected", fileName);

            byte[] buffer;

            using (FileStream fileStream = new FileStream(testCasePath, FileMode.Open))
            {
                long totalBytes = new FileInfo(testCasePath).Length - 1; // /n at the and is not needed
                buffer = new byte[totalBytes];

                fileStream.Read(buffer, 0, (int)totalBytes);
            }

            return buffer;
        }

        #endregion

        #region A+ external functions

        /// <summary>
        /// <see cref="TypeAlternateMethod"/>
        /// </summary>
        public static Func<Aplus, AType, AType> TypeAlternateFunction = TypeAlternateMethod;

        /// <summary>
        /// Tester method which returns different typed results.
        /// </summary>
        /// <param name="env"><see cref="Aplus"/> environemnt.</param>
        /// <param name="number">Input number.</param>
        /// <remarks>
        /// The resulting type of the value is determined by the input number's value.
        /// If the value is:
        /// <list type="bullet">
        ///  <item><description>0: returns 1.1 as a AFloat.</description></item>
        ///  <item><description>1: returns 0 as a AInteger.</description></item>
        ///  <item><description>2: returns `a as a ASymbol.</description></item>
        ///  <item><description>3: returns 'a' as a AFChar.</description></item>
        ///  <item><description>otherwise: returns empty array.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Different typed ATypes.</returns>
        public static AType TypeAlternateMethod(Aplus env, AType number)
        {
            AType result;

            switch (number.asInteger)
            {
                case 0:
                    result = AFloat.Create(1.1);
                    break;
                case 1:
                    result = AInteger.Create(0);
                    break;
                case 2:
                    result = ASymbol.Create("a");
                    break;
                case 3:
                    result = AChar.Create('a');
                    break;
                default:
                    result = Utils.ANull();
                    break;
            }

            return result;
        }

        #endregion
    }
}
