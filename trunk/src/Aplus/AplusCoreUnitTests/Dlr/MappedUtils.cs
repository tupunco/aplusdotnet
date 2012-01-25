using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.Scripting.Hosting;

namespace AplusCoreUnitTests.Dlr
{
    internal static class MappedUtils
    {
        #region Variables

        private static Dictionary<MappedFiles, MappedFileAttribute> mappings =
            new Dictionary<MappedFiles, MappedFileAttribute>();

        #endregion

        #region Constructor

        static MappedUtils()
        {
            MappedFiles[] values = Enum.GetValues(typeof(MappedFiles)) as MappedFiles[];

            foreach (MappedFiles item in values)
            {
                MappedFileAttribute attribute = item.GetMappedFileAttribute();

                if (attribute != null)
                {
                    MappedUtils.mappings.Add(item, attribute);
                }
            }
        }

        #endregion

        #region Lifetime related

        public static void CreateMemoryMappedFiles(ScriptEngine engine)
        {
            foreach (KeyValuePair<MappedFiles, MappedFileAttribute> item in mappings)
            {
                item.Value.CreateMappedFile(engine);
            }
        }

        public static void DeleteMemoryMappedFiles(ref ScriptEngine engine)
        {
            engine = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            foreach (KeyValuePair<MappedFiles, MappedFileAttribute> item in mappings)
            {
                item.Value.DeleteMappedfile();
            }
        }

        #endregion

        #region Extensions

        public static string GetFileName(this MappedFiles value)
        {
            return mappings[value].Filename;
        }

        public static string GetFileNameWithoutExtension(this MappedFiles value)
        {
            return Path.ChangeExtension(value.GetFileName(), null);
        }

        private static void CreateMappedFile(this MappedFileAttribute value, ScriptEngine engine)
        {
            engine.Execute(string.Format("`{0} beam {1}", value.Filename, value.Data));
        }

        private static void DeleteMappedfile(this MappedFileAttribute value)
        {
            string filename = value.Filename;

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        private static MappedFileAttribute GetMappedFileAttribute(this MappedFiles value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            MappedFileAttribute[] attributes =
                fieldInfo.GetCustomAttributes(typeof(MappedFileAttribute), false) as MappedFileAttribute[];

            return (attributes.Length != 0) ? attributes[0] : null;
        }

        #endregion
    }

    internal enum MappedFiles
    {
        [MappedFileAttribute("IntegerScalar.m", "67")]
        IntegerScalar,
        [MappedFileAttribute("FloatScalar.m", "2.3")]
        FloatScalar,
        [MappedFileAttribute("CharScalar.m", "'A'")]
        CharScalar,
        [MappedFileAttribute("Integer23.m", "2 3 rho 5 6 7 9 8 2")]
        IntegerMatrix,
        [MappedFileAttribute("Float22.m", "2 2 rho 3.4 1.4 7.6 1.1")]
        FloatMatrix,
        [MappedFileAttribute("Char25.m", "2 5 rho 'HelloWorld'")]
        CharMatrix
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    internal class MappedFileAttribute : Attribute
    {
        #region Variables

        private string filename;
        private string data;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the filename of the memory mapped file.
        /// </summary>
        public string Filename { get { return this.filename; } }


        /// <summary>
        /// Gets the data represented as an A+ statement.
        /// </summary>
        public string Data { get { return this.data; } }

        #endregion

        #region Constructor

        public MappedFileAttribute(string filename, string data)
        {
            this.filename = filename;
            this.data = data;
        }

        #endregion
    }
}
