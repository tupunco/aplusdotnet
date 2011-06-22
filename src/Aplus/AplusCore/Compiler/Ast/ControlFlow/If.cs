using System;
using System.Text;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class If : Node
    {
        #region Variables

        private Node expression;
        private Node trueCase;
        private Node falseCase;

        #endregion

        #region Properties

        public Node Expression { get { return this.expression; } }
        public Node TrueCase { get { return this.trueCase; } }
        public Node FalseCase { get { return this.falseCase; } }
        public bool HaveFalseCase { get { return this.falseCase != Node.NullConstant(); } }

        #endregion

        #region Constructor

        public If(Node expression, Node trueCase, Node falseCase)
        {
            this.expression = expression;
            this.trueCase = trueCase;
            this.falseCase = falseCase;
        }

        #endregion

        #region Methods

        public void AddFalseCase(Node falseCase)
        {
            this.falseCase = falseCase;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            // Create a temp variable to store the result of the codeblocks
            DLR.ParameterExpression tempVariable = DLR.Expression.Parameter(typeof(AType), "TEMP_RESULT");

            DLR.Expression result = DLR.Expression.Block(
                new DLR.ParameterExpression[] { tempVariable },
                DLR.Expression.IfThenElse(
                       DLR.Expression.IsTrue(
                            this.expression.Generate(scope),
                            typeof(Helpers).GetMethod("BooleanTest")
                        ),
                        DLR.Expression.Assign(tempVariable, this.trueCase.Generate(scope)),
                        DLR.Expression.Assign(tempVariable,
                            this.HaveFalseCase ? this.falseCase.Generate(scope) : DLR.Expression.Constant(Utils.ANull())
                        )
                ),
                tempVariable
            );

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            if (this.HaveFalseCase)
            {
                return String.Format("IF({0} {1}) ELSE({2})", this.expression, this.trueCase, this.falseCase);
            }
            else
            {
                return String.Format("IF({0} {1})", this.expression, this.trueCase);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is If))
            {
                return false;
            }
            If other = (If)obj;

            bool conditionOk = this.expression.Equals(other.expression);
            bool trueCaseOk = this.trueCase.Equals(other.trueCase);
            bool falseCaseOk = this.falseCase.Equals(other.falseCase);

            return conditionOk && trueCaseOk && falseCaseOk;

        }

        public override int GetHashCode()
        {
            int hash = this.expression.GetHashCode() ^ this.trueCase.GetHashCode();
            if (this.HaveFalseCase)
            {
                hash ^= this.falseCase.GetHashCode();
            }

            return hash;
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static If IfElse(Node expression, Node trueCase, Node falseCase)
        {
            return new If(expression, trueCase, falseCase);
        }

        public static If If(Node expression, Node trueCase)
        {
            return new If(expression, trueCase, Node.NullConstant());
        }

    }

    #endregion
}
