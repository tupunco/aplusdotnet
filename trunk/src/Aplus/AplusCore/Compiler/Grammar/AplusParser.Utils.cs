using System.Collections.Generic;

using Antlr.Runtime;

namespace AplusCore.Compiler.Grammar
{
    partial class AplusParser
    {
        #region Variables

        /// <summary>
        /// Describes if the current parsing state is inside a function.
        /// </summary>
        private bool isfunction = false;
        private AST.Node function;
        private Variables variables;
        private FunctionInformation functionInfo;

        /// <summary>
        /// Describes if the current parsing state is inside a function.
        /// </summary>
        private bool isdependency = false;

        private AST.Node tree;

        #endregion

        #region Properties

        public AST.Node Tree
        {
            get { return this.tree; }
        }

        public FunctionInformation FunctionInfo
        {
            get { return this.functionInfo; }
            set { this.functionInfo = value; }
        }

        public bool ParseOk
        {
            get { return NumberOfSyntaxErrors == 0; }
        }

        #endregion

        #region Parser utility

        public bool Parse()
        {
            script();
            return ParseOk;
        }

        public override void ReportError(RecognitionException exception)
        {
            // Add error report here
            base.ReportError(exception);

            if (exception is NoViableAltException)
            {
                // Throw an error that we can't continue the execution/parsing of this input
                throw new ParseException("No Viable Alternate", false, exception);
            }
            else if (exception is UnwantedTokenException)
            {
                UnwantedTokenException ex = (UnwantedTokenException)exception;
                // check if we can continue
                bool canContinue = (ex.Token.Text == "{"); // TODO: change this a constant
                // Throw an error that we can't continue the execution/parsing of this input
                throw new ParseException("Unwanted token found:" + ex.Token.Text, canContinue, ex);
            }
            // Throw an error, maybe we can continue the execution?
            throw new ParseException(exception.Message, true, exception);
        }

        #endregion

        #region Parser internals

        private void SetupUserDefFunction()
        {
            this.isfunction = true;
            this.function = null;
            this.variables = new Variables();
        }

        private void TearDownUserDefFunction()
        {
            this.isfunction = false;
            this.function = null;
            this.variables = null;
        }

        private void RegisterUserDefFunction(AST.UserDefFunction node)
        {
            if (this.FunctionInfo == null || node.Parameters.Length == 0)
            {
                return;
            }

            this.FunctionInfo.RegisterGlobalFunction(node.Identifier.Name);
        }

        private bool IsOperator(IToken nextToken)
        {
            return (nextToken != null) && (this.FunctionInfo != null) && this.FunctionInfo.IsOperator(nextToken.Text);
        }

        private bool IsGlobalFunction(IToken nextToken)
        {
            return (nextToken != null) && (this.FunctionInfo != null) && this.FunctionInfo.IsGlobalFunction(nextToken.Text);
        }

        private AST.Node BuildMonadic(AST.Token symbol, AST.Node argument)
        {
            AST.Node node;

            if (symbol.Type == Tokens.DO)
            {
                node = AST.Node.MonadicDo(argument);
            }
            else if (argument is AST.ExpressionList)
            {
                node = AST.Node.BuiltinInvoke(symbol, (AST.ExpressionList)argument);
            }
            else
            {
                node = AST.Node.MonadicFunction(symbol, argument);
            }

            return node;
        }

        private void AssignmentPreprocessor(AST.Node lhs)
        {
            if (!(lhs is AST.Identifier))
            {
                return; // only need to handle simple assignments
            }
            if (!this.isfunction)
            {
                return;
            }
            AST.Identifier target = (AST.Identifier)lhs;

            if (target.Type != AST.IdentifierType.UnQualifiedName)
            {
                return;
            }

            if (target.IsEnclosed)
            {
                if (this.variables.LocalAssignment.ContainsKey(target.Name))
                {
                    // Found the variable already used in a local assignment
                    target.IsEnclosed = false;
                    this.variables.LocalAssignment[target.Name].Add(target);
                }
                else
                {
                    if (!this.variables.GlobalAssignment.ContainsKey(target.Name))
                    {
                        // variable did not exists currently as global assignment
                        this.variables.GlobalAssignment[target.Name] = new List<AST.Identifier>();
                    }
                    // add the target as a global assignment target
                    this.variables.GlobalAssignment[target.Name].Add(target);
                }
            }
            else
            {
                if (!this.variables.LocalAssignment.ContainsKey(target.Name))
                {
                    this.variables.LocalAssignment[target.Name] = new List<AST.Identifier>();
                }

                if (this.variables.GlobalAssignment.ContainsKey(target.Name))
                {
                    // found the same variable as a global assignment target
                    //  move it to the local assignments
                    foreach (AST.Identifier item in this.variables.GlobalAssignment[target.Name])
                    {
                        item.IsEnclosed = false;
                        this.variables.LocalAssignment[target.Name].Add(item);
                    }

                    // remove from the global assignments' list
                    this.variables.GlobalAssignment.Remove(target.Name);
                }

                this.variables.LocalAssignment[target.Name].Add(target);
            }

        }

        private AST.Node BuildDyadic(AST.Token symbol, AST.Node lhs, AST.Node rhs)
        {
            AST.Node node;

            if (lhs is AST.BuiltInFunction)
            {
                /* This will allow the following construct:
                    ((-)) * 5
                */
                node = BuildMonadic(((AST.BuiltInFunction)lhs).Function, BuildMonadic(symbol, rhs));
            }
            else if (lhs is AST.BuiltInOperator)
            {
                /* This will allow the following construct:
                    ((-each)) * 5
                */

                AST.Operator op = ((AST.BuiltInOperator)lhs).Operator;
                op.RightArgument = BuildMonadic(symbol, rhs);

                node = op;
            }
            else
            {
                switch (symbol.Type)
                {
                    case Tokens.DO:
                        node = AST.Node.DyadicDo(lhs, rhs);
                        break;

                    case Tokens.RESULT: // Tokens.Assign
                        AssignmentPreprocessor(lhs);
                        node = AST.Node.Assign(lhs, rhs);
                        break;

                    default:
                        if (rhs is AST.ExpressionList)
                        {
                            throw new ParseException("Incorrect call format", false);
                        }

                        node = AST.Node.DyadicFunction(symbol, lhs, rhs);
                        break;
                }
            }

            return node;
        }

        /// <summary>
        /// Updates the given <see cref="AST.UserDefInvoke"/> node with the given <paramref name="argument"/> information.
        /// </summary>
        /// <param name="function">The <see cref="AST.UserDefInvoke"/> to update.</param>
        /// <param name="argument">The argument(s) to use for the node update.</param>
        /// <returns>The updated <see cref="AST.UserDefInvoke"/> node.</returns>
        private AST.UserDefInvoke UpdateUserDefInvoke(AST.UserDefInvoke function, AST.Node argument)
        {
            if (argument is AST.ExpressionList)
            {
                AST.ExpressionList arguments = (AST.ExpressionList)argument;
                foreach (AST.Node node in arguments.Items)
                {
                    function.Arguments.AddLast(node);
                }
            }
            else
            {
                function.Arguments.AddLast(argument);
            }

            return function;
        }
        #endregion
    }
}
