using System;

using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler
{
    /// <summary>
    /// Aplus Scipt Code is responsible to invoke the DLR code generation.
    /// </summary>
    public class AplusScriptCode : ScriptCode
    {
        #region Variables

        private Aplus aplus;
        private DLR.Expression<Func<Runtime.AplusEnvironment, AType>> lambda;

        #endregion

        #region Constructor

        public AplusScriptCode(Aplus aplus, string code, SourceUnit sourceunit)
            : base(sourceunit)
        {
            
            this.aplus = aplus;
            this.lambda = ParseToLambda(code.Trim());
        }

        #endregion

        #region Script Run

        public override object Run()
        {
            return Run(new Scope());
        }

        public override object Run(Scope scope)
        {
            this.aplus.AutoloadContext(scope);

            Func<Runtime.AplusEnvironment, AType> compiled = this.lambda.Compile();
            object result = compiled(new AplusEnvironment(this.aplus, scope));

            return result;
        }

        #endregion

        #region Code generator

        public DLR.Expression<System.Func<Runtime.AplusEnvironment, AType>> ParseToLambda(string code)
        {
            AplusScope scope = new AplusScope(null, "__top__", this.aplus,
                DLR.Expression.Parameter(typeof(Runtime.Aplus), "__aplusRuntime__"),
                DLR.Expression.Parameter(typeof(DYN.IDynamicMetaObjectProvider), "__module__"),
                DLR.Expression.Parameter(typeof(AplusEnvironment), "__ENVIRONMENT__")
            );

            DLR.Expression codebody = null;
            AST.Node tree = Compiler.Parse.String(code, this.aplus.LexerMode);

            if (tree == null)
            {
                codebody = DLR.Expression.Constant(null);
            }
            else
            {
                codebody = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { scope.RuntimeExpression, scope.ModuleExpression },
                    DLR.Expression.Assign(
                        scope.RuntimeExpression, DLR.Expression.PropertyOrField(scope.AplusEnvironment, "Runtime")
                    ),
                    DLR.Expression.Assign(
                        scope.ModuleExpression, DLR.Expression.PropertyOrField(scope.AplusEnvironment, "Context")
                    ),
                    tree.Generate(scope)
                );
            }

            DLR.Expression<System.Func<Runtime.AplusEnvironment, AType>> method =
                DLR.Expression.Lambda<Func<Runtime.AplusEnvironment, AType>>(
                    codebody,
                    scope.AplusEnvironment
                );
            return method;
        }

        #endregion
    }
}
