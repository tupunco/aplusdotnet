using System;

using Microsoft.Scripting.Hosting.Shell;

namespace AplusCore.Hosting
{
    public class AplusConsoleHost : ConsoleHost
    {
        #region ConsoleHost Property overrides

        protected override Type Provider
        {
            get { return typeof(AplusCore.Runtime.AplusLanguageContext); }
        }

        #endregion

        #region ConsoleHost Method overrides

        protected override CommandLine CreateCommandLine()
        {
            return new AplusCore.Hosting.AplusCommandLine();
        }

        protected override Microsoft.Scripting.Hosting.ScriptRuntimeSetup CreateRuntimeSetup()
        {
            return base.CreateRuntimeSetup();
        }

        protected override void ParseHostOptions(string[] args)
        {
            base.ParseHostOptions(args);
        }

        protected override void ExecuteInternal()
        {
            base.ExecuteInternal();
        }

        protected override OptionsParser CreateOptionsParser()
        {
            return new AplusOptionsParser();
        }

        #endregion
    }
}
