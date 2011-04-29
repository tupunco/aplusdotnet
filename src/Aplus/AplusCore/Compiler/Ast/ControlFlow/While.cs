using System;
using System.Collections.Generic;
using System.Text;
using DLR = System.Linq.Expressions;
using AplusCore.Runtime;
using AplusCore.Types;

namespace AplusCore.Compiler.AST
{
    public class While : Node
    {
        #region Variables

        private Node expression;
        private Node codeBlock;

        #endregion

        #region Constructor

        public While(Node expression, Node codeBlock)
        {
            this.expression = expression;
            this.codeBlock = codeBlock;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            // This parameter will store the return value
            DLR.ParameterExpression returnValue = DLR.ParameterExpression.Parameter(typeof(AType), "RETURN_VALUE");
            DLR.LabelTarget breakLabel = DLR.Expression.Label(typeof(AType), "BREAK");

            DLR.Expression result = DLR.Expression.Block(
                new DLR.ParameterExpression[] { returnValue },
                // Assign the default return value: ANull
                DLR.Expression.Assign(returnValue, DLR.Expression.Constant(Utils.ANull())),
                DLR.Expression.Loop(
                    DLR.Expression.Block(
                        DLR.Expression.IfThen(
                        // Test if the condition is equal to 0
                            // Invert the inner test's result 
                            DLR.Expression.Not(
                                // This part will test if the condition is true (this means it is not 0)
                                DLR.Expression.IsTrue(
                                    this.expression.Generate(scope),
                                    typeof(Helpers).GetMethod("BooleanTest")
                                )
                            ),
                            // Break out from the loop with the last computed value
                            DLR.Expression.Break(breakLabel, returnValue)
                        ),
                        // Compute & assign the value
                        DLR.Expression.Assign(returnValue, this.codeBlock.Generate(scope))
                    ),
                    // The label where to jump in case of break
                    breakLabel
                )
            );


            return result;
        }


        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("WHILE({0} {1})", this.expression, this.codeBlock);
        }

        public override bool Equals(object obj)
        {
            if (obj is While)
            {
                While other = (While)obj;
                return (this.expression == other.expression) && (this.codeBlock == other.codeBlock);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.expression.GetHashCode() ^ this.codeBlock.GetHashCode();
        }

        #endregion

        #region GraphViz output (Only in DEBUG)
#if DEBUG
        private static int counter = 0;
        internal override string ToDot(string parent, StringBuilder textBuilder)
        {
            string name = String.Format("While{0}", counter++);
            string exprName = this.expression.ToDot(name, textBuilder);
            string codeBlockName = this.codeBlock.ToDot(name, textBuilder);

            textBuilder.AppendFormat("  {0} [label=\"While\"];\n", name);
            textBuilder.AppendFormat("  {0} -> {1};\n", name, exprName);
            textBuilder.AppendFormat("  {0} -> {1};\n", name, codeBlockName);


            return name;
        }

#endif
        #endregion

    }


    #region Construction helper

    public partial class Node
    {
        public static While While(Node expression, Node codeBlock)
        {
            return new While(expression, codeBlock);
        }
    }

    #endregion
}
