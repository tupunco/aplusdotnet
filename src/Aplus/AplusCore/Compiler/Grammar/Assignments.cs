using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AplusCore.Compiler.Grammar
{
    public class Assignments
    {
        #region Properties

        public Dictionary<string, List<AST.Identifier>> Local { get; private set; }
        public Dictionary<string, List<AST.Identifier>> Global { get; private set; }

        #endregion

        #region Constructors

        public Assignments()
        {
            this.Local = new Dictionary<string, List<AST.Identifier>>();
            this.Global = new Dictionary<string, List<AST.Identifier>>();
        }

        #endregion
    }
}
