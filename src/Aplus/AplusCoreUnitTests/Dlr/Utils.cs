using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public static class Utils
    {
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

            return InfoResult.OK;
        }

    }
}
