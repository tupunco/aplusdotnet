using Microsoft.Scripting.Hosting.Shell;
using Microsoft.Scripting.Utils;

using AplusCore.Compiler;

namespace AplusCore.Hosting
{
    public class AplusOptionsParser : OptionsParser<AplusConsoleOptions>
    {

        public AplusOptionsParser()
            : base()
        {
        }

        protected override void ParseArgument(string arg)
        {
            switch (arg)
            {
                case "--mode":
                    string value = this.PopNextArg();
                    if (value == "apl")
                    {
                        this.LanguageSetup.Options["LexerMode"] = LexerMode.APL;
                    }
                    else
                    {
                        this.LanguageSetup.Options["LexerMode"] = LexerMode.ASCII;
                    }
                    break;

                case "--apl":
                    this.LanguageSetup.Options["LexerMode"] = LexerMode.APL;
                    break;

                case "--ascii":
                    this.LanguageSetup.Options["LexerMode"] = LexerMode.ASCII;
                    break;


                default:
                    base.ParseArgument(arg);
                    break;
            }
        }

        public override void GetHelp(out string commandLine, out string[,] options, out string[,] environmentVariables, out string comments)
        {
            string[,] standardOptions;
            base.GetHelp(out commandLine, out standardOptions, out environmentVariables, out comments);

            string[,] aplusoptions = new string[,] {
                {"--apl",               "APL input mode"},
                {"--ascii",             "ASCII input mode"},
                {"--mode [ascii|apl]",  "Choose input mode"},
            };

            options = ArrayUtils.Concatenate<string>(standardOptions, aplusoptions);
        }

    }
}
