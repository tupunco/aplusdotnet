using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.Hosting;
using System.Threading;
using System.Globalization;

namespace AplusCoreUnitTests.Dlr
{

    [TestClass]
    public abstract class AbstractTest
    {
        protected ScriptEngine engine;
        protected ScriptEngine engineUni;

        [TestInitialize]
        public void Setup()
        {
            ScriptRuntimeSetup setup = new ScriptRuntimeSetup();
            setup.LanguageSetups.Add(AplusCore.Runtime.AplusLanguageContext.LanguageSetup);

            ScriptRuntime dlrRuntime = new ScriptRuntime(setup);
            this.engine = dlrRuntime.GetEngine("A+");

            ScriptRuntimeSetup setupUni = new ScriptRuntimeSetup();
            setupUni.LanguageSetups.Add(AplusCore.Runtime.AplusLanguageContext.LanguageSetup);
            setupUni.Options.Add("LexerMode", AplusCore.Compiler.LexerMode.UNI);

            ScriptRuntime dlrRuntimeUni = new ScriptRuntime(setupUni);
            this.engineUni = dlrRuntimeUni.GetEngine("A+");
        }
    }
}
