using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting.Shell;
using System.IO;
using Microsoft.Scripting;
using System.Threading;
using Microsoft.Scripting.Hosting;
using AplusCore.Runtime;
using AplusCore.Compiler;
using AplusCore.Types;
using Microsoft.Scripting.Runtime;
using System.Diagnostics;

namespace AplusCore.Hosting
{
    public class AplusCommandLine : CommandLine
    {
        #region Properties

        public AplusLanguageContext AplusLanguageContext { get { return (AplusLanguageContext)this.Language; } }

        #endregion

        #region Constructor

        public AplusCommandLine()
            : base()
        {
        }

        #endregion // CallWithContext

        #region CommandLine Property overrides

        protected override string Logo
        {
            get
            {
                return String.Format(".Net A+ Interpreter codename: Eskim-o ({0} build) {1}\n\n",
                    AplusCore.Runtime.AplusLanguageContext.Version,
                    this.AplusLanguageContext.Runtime.LexerMode);
            }
        }


        protected override string Prompt
        {
            get { return "   "; }
        }

        public override string PromptContinuation
        {
            get { return "*  "; }
        }

        #endregion

        #region Methods

        private ScriptCodeParseResult GetCommandPropertiesAndParse(string code, out ScriptSource source)
        {
            ScriptCodeParseResult result = ScriptCodeParseResult.IncompleteStatement;
            try
            {
                ScriptSource command = this.Engine.CreateScriptSourceFromString(code, SourceCodeKind.InteractiveCode);
                result = command.GetCodeProperties(this.Engine.GetCompilerOptions(this.ScriptScope));
                source = command;

            }
            catch (ParseException error)
            {
                if (error.CanContinue)
                {
                    result = ScriptCodeParseResult.IncompleteStatement;
                }
                else
                {
                    result = ScriptCodeParseResult.Invalid;
                    Console.ErrorOutput.WriteLine("//[parse]: {0}", error.Message);
                }

                source = null;
            }
            //catch (Exception e)
            //{
            //    source = null;
            //    Console.ErrorOutput.WriteLine("**->{0}", e);
            //}


            return result;
        }

        private ScriptSource GetStatement(out bool continueInteraction)
        {
            ScriptSource source = null;
            StringBuilder codeBuilder = new StringBuilder();
            int autoIndentSize = 0;

            this.Console.Write(Prompt, Style.Prompt);

            while (true)
            {
                string line = ReadLine(autoIndentSize);
                continueInteraction = true;

                if (line == null)
                {
                    continueInteraction = false;
                    return source;
                }

                if (line == "$")
                {
                    return null;
                }

                bool allowIncompleteStatement = false;
                codeBuilder.Append(line);
                codeBuilder.Append('\n');

                string code = codeBuilder.ToString();

                var props = GetCommandPropertiesAndParse(code, out source);
                if (SourceCodePropertiesUtils.IsCompleteOrInvalid(props, allowIncompleteStatement))
                {
                    return props != ScriptCodeParseResult.Empty ? source : null;
                }

                // Keep on reading input
                this.Console.Write(PromptContinuation, Style.Prompt);
            }
        }


        protected void LoadFile(string fileName)
        {
            LoadFile(this.Engine.CreateScriptSourceFromFile(fileName));
        }

        protected void LoadFile(ScriptSource source)
        {
            AType result;
            try
            {
                result = source.Execute<AType>(this.ScriptScope);
            }
            catch (Exception e)
            {
                UnhandledException(e);
            }
        }

        #endregion

        #region CommandLine Method overrides

        static AType Time(AplusEnvironment scope)
        {
            Process process = Process.GetCurrentProcess();

            TimeSpan elpasedTime = DateTime.Now - process.StartTime;

            AType result = AArray.Create(ATypes.AInteger,
                AFloat.Create(process.PrivilegedProcessorTime.TotalMilliseconds),
                AFloat.Create(process.TotalProcessorTime.TotalMilliseconds),
                AFloat.Create(elpasedTime.TotalMilliseconds)
            );

            return result;
        }

        protected override int RunInteractive()
        {
            this.ScriptScope = this.Engine.CreateScope();

            this.ScriptScope.SetVariable(
                "time",
                AFunc.Create(
                    "time",
                    (Func<AplusEnvironment, AType>)Time,
                    1,
                    "returns the user time of the current process"
                )
            );

            return base.RunInteractive();
        }

        protected override int RunFile(ScriptSource source)
        {
            if (this.ScriptScope == null)
            {
                this.ScriptScope = this.Engine.CreateScope();

                this.ScriptScope.SetVariable(
                    "time",
                    AFunc.Create(
                        "time",
                        (Func<AplusEnvironment, AType>)Time,
                        1,
                        "returns the user time of the current process"
                    )
                );
            }

            try
            {
                AType result = source.Execute<AType>(this.ScriptScope);
                Console.WriteLine(result.ToString(), Style.Out);
            }
            catch (Error error)
            {
                Console.ErrorOutput.Write(error);
                return 1;
            }
            catch (Exception)
            {

                throw;
            }
            return 0;
        }

        protected override int? TryInteractiveAction()
        {
            int? result = null;

            try
            {
                result = RunOneInteraction();
            }
            catch (ThreadAbortException tae)
            {
                KeyboardInterruptException pki = tae.ExceptionState as KeyboardInterruptException;
                if (pki != null)
                {
                    UnhandledException(tae);
                    Thread.ResetAbort();
                }
                else
                {
                    throw;
                }

            }

            return result;
        }

        /// <summary>
        /// Parses a single interactive command or a set of statements and executes it.  
        /// </summary>
        /// <returns>null or the error code to exit with</returns>
        private int? RunOneInteraction()
        {
            bool continueInteraction;
            ScriptSource source = this.GetStatement(out continueInteraction);

            if (continueInteraction == false)
            {
                return this.ExitCode;
            }

            if (source == null)
            {
                // Is it an empty line?
                //this.Console.Write(String.Empty, Style.Out);
                return null;
            }

            try
            {
                object result = ExecuteCommand(source);
                // Don't print if the result is an ANull
                if (result is AType && ((AType)result).Type != ATypes.ANull)
                {
                    Console.WriteLine(AplusLanguageContext.Write((AType)result), Style.Out);
                }
            }
            catch (Error error)
            {
                Console.ErrorOutput.WriteLine(error);
            }
            return null;
        }



        #endregion
    }
}
