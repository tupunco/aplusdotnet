﻿using System;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Scripting.Hosting;

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

            return InfoResult.OK;
        }

        #endregion

        #region MemoryMappedFiles

        public static void CreateMemoryMappedFiles(ScriptEngine engine)
        {
            engine.Execute<AType>(CreateMapCreator("IntegerScalar.m", "67"));
            engine.Execute<AType>(CreateMapCreator("FloatScalar.m", "2.3"));
            engine.Execute<AType>(CreateMapCreator("CharScalar.m", "'A'"));
            engine.Execute<AType>(CreateMapCreator("Integer23.m", "2 3 rho 5 6 7 9 8 2"));
            engine.Execute<AType>(CreateMapCreator("Float22.m", "2 2 rho 3.4 1.4 7.6 1.1"));
            engine.Execute<AType>(CreateMapCreator("Char25.m", "2 5 rho 'HelloWorld'"));
        }

        public static void DeleteMemoryMappedFiles()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            string[] files = new string[] { "IntegerScalar.m", "CharScalar.m", "FloatScalar.m", "Integer23.m", "Float22.m", "Char25.m" };

            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]))
                {
                    File.Delete(files[i]);
                }
            }
        }

        public static string CreateMapCreator(string name, string data)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("'");
            builder.Append(name);
            builder.Append("'");
            builder.Append(" beam ");
            builder.Append(data);

            return builder.ToString();
        }

        public static string CreateMap(byte mode, string name)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(mode);
            builder.Append(" beam ");
            builder.Append("'");
            builder.Append(name);
            builder.Append("'");

            return builder.ToString();
        }

        #endregion
    }
}