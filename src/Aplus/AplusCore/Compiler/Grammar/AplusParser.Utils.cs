using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace AplusCore.Compiler.Grammar
{
    partial class AplusParser
    {
        private Dictionary<string, List<AST.Identifier>> localAssignments = null;
        private Dictionary<string, List<AST.Identifier>> globalAssignments = null;


        /// <summary>
        /// Describes if the current parsing state is inside a function.
        /// </summary>
        private bool isfunction = false;
        private AST.Node function;

        /// <summary>
        /// Describes if the current parsing state is inside a function.
        /// </summary>
        private bool isdependency = false;
        private HashSet<AST.Identifier> variableAccessing = null;

        public AST.Node tree;

        public bool ParseOk { get { return NumberOfSyntaxErrors == 0; } }

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

        private void SetupUserDefFunction()
        {
            this.localAssignments = new Dictionary<string, List<AST.Identifier>>();
            this.globalAssignments = new Dictionary<string, List<AST.Identifier>>();
            this.isfunction = true;
            this.function = null;
        }

        private void TearDownUserDefFunction()
        {
            this.localAssignments = null;
            this.globalAssignments = null;
            this.isfunction = false;
            this.function = null;
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
                if (this.localAssignments.ContainsKey(target.Name))
                {
                    // Found the variable already used in a local assignment
                    target.IsEnclosed = false;
                    this.localAssignments[target.Name].Add(target);
                }
                else
                {
                    if (!this.globalAssignments.ContainsKey(target.Name))
                    {
                        // variable did not exists currently as global assignment
                        this.globalAssignments[target.Name] = new List<AST.Identifier>();
                    }
                    // add the target as a global assignment target
                    this.globalAssignments[target.Name].Add(target);
                }
            }
            else
            {
                if (!this.localAssignments.ContainsKey(target.Name))
                {
                    this.localAssignments[target.Name] = new List<AST.Identifier>();
                }

                if (this.globalAssignments.ContainsKey(target.Name))
                {
                    // found the same variable as a global assignment target
                    //  move it to the local assignments
                    foreach (AST.Identifier item in this.globalAssignments[target.Name])
                    {
                        item.IsEnclosed = false;
                        this.localAssignments[target.Name].Add(item);
                    }

                    // remove from the global assignments' list
                    this.globalAssignments.Remove(target.Name);
                }

                this.localAssignments[target.Name].Add(target);
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

    }
}
