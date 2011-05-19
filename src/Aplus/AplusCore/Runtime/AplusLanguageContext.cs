using System;
using System.Collections.Generic;
using System.Configuration;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;

using AplusCore.Compiler;
using AplusCore.Types;

namespace AplusCore.Runtime
{
    public class AplusLanguageContext : LanguageContext
    {
        #region A+ Language Setup

        private static LanguageSetup langsetup = new LanguageSetup(
            typeof(AplusLanguageContext).AssemblyQualifiedName,
            "A+", new[] { "a+", "aplus", "A+" }, new[] { ".a+" }
        );

        private static Version languageversion = new Version(1, 0);

        public static LanguageSetup LanguageSetup
        {
            get { return langsetup; }
        }

        public static string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        #endregion

        #region Variables

        private Aplus aplus;

        #endregion

        #region Properties

        public Aplus Runtime { get { return this.aplus; } }

        #endregion

        #region Constructor

        public AplusLanguageContext(ScriptDomainManager manager, IDictionary<string, object> options)
            : base(manager)
        {
            if (options.ContainsKey("LexerMode"))
            {
                this.aplus = new Aplus(manager.Globals, (LexerMode)options["LexerMode"]);
            }
            else
            {
                LexerMode mode = LexerMode.ASCII;
                // read the default lexer mode from the application's config file
                string modeText = ConfigurationManager.AppSettings["LexerMode"];
                if (modeText != null)
                {
                    mode = Aplus.ConvertToLexerMode(modeText);
                }
               
                this.aplus = new Aplus(manager.Globals, mode);
            }
            
        }

        #endregion

        #region Language Context Property overrides

        public override T ScopeGetVariable<T>(Scope scope, string name)
        {
            string[] contextParts = Util.SplitUserName(name);

            ScopeStorage storage = base.ScopeGetVariable<ScopeStorage>(scope, contextParts[0]);

            return storage.GetValue(contextParts[1], false);
        }

        public override dynamic ScopeGetVariable(Scope scope, string name)
        {
            string[] contextParts = Util.SplitUserName(name);

            ScopeStorage storage = base.ScopeGetVariable<ScopeStorage>(scope, contextParts[0]);

            return storage.GetValue(contextParts[1], false);
        }

        public override bool ScopeTryGetVariable(Scope scope, string name, out dynamic value)
        {
            string[] contextParts = Util.SplitUserName(name);
            dynamic context; // ScopeStorage
            value = null;

            return base.ScopeTryGetVariable(scope, contextParts[0], out context)
                && context.TryGetValue(contextParts[1], false, out value);
        }

        public override void ScopeSetVariable(Scope scope, string name, object value)
        {
            string[] contextParts = Util.SplitUserName(name);

            dynamic storage; // ScopeStorage
            if (!base.ScopeTryGetVariable(scope, contextParts[0], out storage))
            {
                storage = new ScopeStorage();
                base.ScopeSetVariable(scope, contextParts[0], (ScopeStorage)storage);
            }
            ((ScopeStorage)storage).SetValue(contextParts[1], false, value);
        }

        public override Version LanguageVersion
        {
            get { return AplusLanguageContext.languageversion; }
        }

        #endregion

        #region Language Context Method overrides

        public override ScriptCode CompileSourceCode(SourceUnit sourceUnit, CompilerOptions options, ErrorSink errorSink)
        {
            using (SourceCodeReader reader = sourceUnit.GetReader())
            {
                switch (sourceUnit.Kind)
                {
                    case SourceCodeKind.File:
                    case SourceCodeKind.AutoDetect:
                    // TODO: add a different kind here!
                    case SourceCodeKind.Expression:
                    case SourceCodeKind.InteractiveCode:
                    case SourceCodeKind.SingleStatement:
                    case SourceCodeKind.Statements:
                        return new AplusScriptCode(this.aplus,
                            reader.ReadToEnd(),
                            sourceUnit);

                    case SourceCodeKind.Unspecified:
                    default:

                        throw new Exception("SourceKind fail..");
                }
            }
        }

        public override TService GetService<TService>(params object[] args)
        {
            if (typeof(TService) == typeof(Aplus))
            {
                return (TService)(object)this.aplus;
            }
            return base.GetService<TService>(args);
        }

        #endregion

        #region Methods

        public string Write(AType result)
        {
            return AplusLanguageContext.FormatAType(result, this.Runtime);
        }

        #endregion

        #region Static Methods

        // Todo: extend it to correctly return the values like the original a+ interpreter
        public static string FormatAType(AType value, Aplus runtime)
        {
            if (value.IsBox || ((value is AReference) && ((AReference)value).Data is AFunc))
            {
                return value.ToString();
            }

            return Function.Monadic.MonadicFunctionInstance.DefaultFormat.Execute(
                value,
                new AplusEnvironment(runtime, null)
            ).ToString();
        }

        #endregion
    }
}
