using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class Case : Node
    {
        #region Variables

        private Node expression;
        private ExpressionList caseList;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.Case; }
        }

        public Node Expression { get { return this.expression; } }
        public ExpressionList CaseList { get { return this.caseList; } }

        #endregion

        #region Constructor

        public Case(Node expression, ExpressionList caseList)
        {
            this.expression = expression;
            // TODO: correct expression list to have correct number of elements for case node
            this.caseList = caseList;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            // Target condition
            DLR.Expression target = this.expression.Generate(scope);

            DLR.Expression defaultCase;

            if (this.caseList.Length % 2 == 1)
            {
                // odd number of cases, last one is the default case
                defaultCase = this.caseList.Items.Last.Value.Generate(scope);
                // remove the default case from the list
                this.caseList.Items.RemoveLast();
            }
            else
            {
                // No default case, set it to an ANull
                defaultCase = DLR.Expression.Constant(Utils.ANull());
            }

            List<DLR.SwitchCase> cases = new List<DLR.SwitchCase>();

            for (int i = 0; i < this.caseList.Length; i += 2)
            {
                // Add each case
                //  1. (i+1) is the case's codeblock
                //  2. (i) is the case's test value
                cases.Add(
                    DLR.Expression.SwitchCase(
                        this.caseList[i + 1].Generate(scope),
                        this.caseList[i].Generate(scope)
                    )
                );
            }

            DLR.Expression result = DLR.Expression.Switch(
                typeof(AType),
                target,
                defaultCase,
                comparisonMethod,
                cases
            );

            return result;
        }

        /// <summary>
        /// Method to use for comparing target value and case values
        /// </summary>
        private static MethodInfo comparisonMethod =
            typeof(Case).GetMethod("CaseCheck", BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// This is the expected result of the case check if there is a match
        /// </summary>
        private static AType trueValue = AInteger.Create(1);

        /// <summary>
        /// Method used in case checking by the Case AST Node in the generated DLR expression tree
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="caseValue"></param>
        /// <returns></returns>
        internal static bool CaseCheck(AType targetValue, AType caseValue)
        {
            if (targetValue.Type == ATypes.ANull)
            {
                // ANull is always false! this will lead to the default case.
                return false;
            }

            AType result = DyadicFunctionInstance.Member.Execute(caseValue, targetValue);

            bool found = trueValue.Equals(DyadicFunctionInstance.Member.Execute(result, trueValue));
            return found;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Case({0} {1})", this.expression, this.caseList);
        }

        public override bool Equals(object obj)
        {
            if (obj is Case)
            {
                Case other = (Case)obj;
                return (this.expression == other.expression) && (this.caseList == other.caseList);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.expression.GetHashCode() ^ this.caseList.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static Case Case(Node expression, ExpressionList caseList)
        {
            return new Case(expression, caseList);
        }

        public static Case Case(Node expression, Node caseList)
        {
            Debug.Assert(caseList is ExpressionList);
            return new Case(expression, (ExpressionList)caseList);
        }
    }

    #endregion
}
