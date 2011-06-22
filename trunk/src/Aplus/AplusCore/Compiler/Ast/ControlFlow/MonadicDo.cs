using System;
using System.Text;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class MonadicDo : Node
    {
        #region Variables

        private Node codeblock;

        #endregion

        #region Properties

        public Node Codeblock { get { return this.codeblock; } }

        #endregion

        #region Constructor

        public MonadicDo(Node codeblock)
        {
            this.codeblock = codeblock;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            // TODO: Add usage of Protected Execute Flag

            // Save the previous return target
            DLR.LabelTarget oldTarget = scope.ReturnTarget;
            scope.ReturnTarget = DLR.Expression.Label(typeof(AType), "EXIT");

            DLR.Expression protectedCode = DLR.Expression.Label(
                scope.ReturnTarget,
                this.codeblock.Generate(scope)
            );

            // Restore the return target;
            scope.ReturnTarget = oldTarget;

            // Code block contining the strandard execution's result
            // wrapped in a strand
            DLR.Expression block =
                DLR.Expression.Call(
                    typeof(Runtime.Helpers).GetMethod("BuildStrand"),
                    DLR.Expression.NewArrayInit(
                        typeof(AType),
                // We need to pass in reverse order
                        protectedCode,
                        DLR.Expression.Constant(AInteger.Create(0), typeof(AType))
                    )
                );

            // Parameter for Catch block
            DLR.ParameterExpression errorVariable = DLR.Expression.Parameter(typeof(Error), "error");

            // Catch block, returns the ([errorcode]; [errortext]) strand
            DLR.CatchBlock catchBlock = DLR.Expression.Catch(
                errorVariable,
                DLR.Expression.Call(
                    typeof(Runtime.Helpers).GetMethod("BuildStrand"),
                    DLR.Expression.NewArrayInit(
                        typeof(AType),
                // We need to pass in reverse order
                // Error Text
                        DLR.Expression.Call(
                            typeof(Runtime.Helpers).GetMethod("BuildString"),
                            DLR.Expression.Property(errorVariable, "ErrorText")
                        ),
                // Error Code
                        DLR.Expression.Call(
                            typeof(AInteger).GetMethod("Create", new Type[] { typeof(int) }),
                            DLR.Expression.Convert(
                                DLR.Expression.Property(errorVariable, "ErrorType"),
                                typeof(int)
                            )
                        )
                    )
                )
            );

            DLR.Expression result = DLR.Expression.TryCatch(
                block,
                catchBlock 
            );

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Do({0})", this.codeblock);
        }

        public override bool Equals(object obj)
        {
            if (obj is MonadicDo)
            {
                MonadicDo other = (MonadicDo)obj;
                return this.codeblock == other.codeblock;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.codeblock.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static MonadicDo MonadicDo(Node codeblock)
        {
            return new MonadicDo(codeblock);
        }
    }

    #endregion
}
