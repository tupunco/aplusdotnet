using System.Collections.Generic;

namespace AplusCore.Compiler.Grammar
{
    public class Variables
    {
        #region Properties

        public Dictionary<string, List<AST.Identifier>> LocalAssignment { get; private set; }
        public Dictionary<string, List<AST.Identifier>> GlobalAssignment { get; private set; }
        public Dictionary<string, List<AST.Identifier>> Accessing { get; private set; }

        #endregion

        #region Constructors

        public Variables()
        {
            this.LocalAssignment = new Dictionary<string, List<AST.Identifier>>();
            this.GlobalAssignment = new Dictionary<string, List<AST.Identifier>>();
            this.Accessing = new Dictionary<string, List<AST.Identifier>>();
        }

        #endregion

        #region Grammar Support

        public void AddAccess(AST.Identifier variable)
        {
            if (!this.Accessing.ContainsKey(variable.Name))
            {
                this.Accessing[variable.Name] = new List<AST.Identifier>();
            }

            this.Accessing[variable.Name].Add(variable);
        }

        #endregion
    }
}
