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
        private Node defaultCase;

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

        /// <summary>
        /// Gets the default case of the <see cref="Case"/> node.
        /// </summary>
        public Node DefaultCase
        {
            get { return this.defaultCase; }
        }

        #endregion

        #region Constructor

        public Case(Node expression, ExpressionList caseList)
        {
            this.expression = expression;
            this.caseList = caseList;

            NormalizeCases();
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;
            // Target condition
            DLR.Expression target = this.expression.Generate(scope);
            DLR.Expression defaultNode = this.defaultCase.Generate(scope);

            if (this.caseList.Length > 0)
            {
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

                result = DLR.Expression.Switch(
                    typeof(AType),
                    target,
                    defaultNode,
                    comparisonMethod,
                    cases
                );
            }
            else
            {
                result = DLR.Expression.Block(target, defaultNode);
            }

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

        #region Utility

        /// <summary>
        /// Normalizes the cases. Sets the default case based on the number of cases.
        /// </summary>
        private void NormalizeCases()
        {
            if (this.caseList.Length % 2 == 1)
            {
                // odd number of cases, the last one is the default case
                this.defaultCase = this.caseList.Items.Last.Value;
                this.caseList.Items.RemoveLast();
            }
            else
            {
                // there is no default case, add a Null (by definition) as default
                this.defaultCase = Node.NullConstant();
            }
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
            return this.expression.GetHashCode() ^ this.caseList.GetHashCode() ^ this.defaultCase.GetHashCode();
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
