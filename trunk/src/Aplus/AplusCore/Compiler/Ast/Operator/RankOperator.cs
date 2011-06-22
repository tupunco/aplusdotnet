using System;

using AplusCore.Runtime.Function.Operator.Dyadic;
using AplusCore.Runtime.Function.Operator.Monadic;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class RankOperator : Operator
    {
        #region Variables

        private Node condition;

        #endregion

        #region Properties

        internal Node Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        #endregion

        #region Constructor

        public RankOperator(Node function, Node condition)
            : base(function)
        {
            this.condition = condition;
        }

        #endregion

        #region DLR generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression func;

            if (this.function is Token)
            {
                Node wrappedFunction = new BuiltInFunction((Token)this.function);
                func = wrappedFunction.Generate(scope);
            }
            else
            {
                func = this.function.Generate(scope);
            }

            DLR.Expression result;

            if (isDyadic)
            {
                result = DLR.Expression.Call(
                    DLR.Expression.Constant(DyadicOperatorInstance.Rank),
                    DyadicOperatorInstance.Rank.GetType().GetMethod("Execute"),
                    func,
                    this.condition.Generate(scope),
                    this.rightarg.Generate(scope),
                    this.leftarg.Generate(scope),
                    scope.GetAplusEnvironment()
                );
            }
            else
            {
                result = DLR.Expression.Call(
                DLR.Expression.Constant(MonadicOperatorInstance.Rank),
                MonadicOperatorInstance.Rank.GetType().GetMethod("Execute"),
                func,
                this.condition.Generate(scope),
                this.rightarg.Generate(scope),
                scope.GetAplusEnvironment()
            );
            }

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            if (isDyadic)
            {
                return String.Format("Rank(({0} {1}) {2} {3}", this.function, this.condition, this.leftarg, this.rightarg);
            }
            else
            {
                return String.Format("Rank(({0} {1}) {2}", this.function, this.condition, this.rightarg);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is RankOperator)
            {
                RankOperator other = (RankOperator)obj;
                bool result = (this.function == other.function) && (this.rightarg == other.rightarg) && (this.condition == other.condition);
                if (isDyadic)
                {
                    result = result && (this.leftarg == other.leftarg);
                }
                return result;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int value = this.function.GetHashCode() ^ this.rightarg.GetHashCode() ^ this.condition.GetHashCode();
            if (isDyadic)
            {
                value ^= this.leftarg.GetHashCode();
            }

            return value;
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static RankOperator RankOperator(Node function, Node condition)
        {
            return new RankOperator(function, condition);
        }

        public static RankOperator RankOperator(Token opToken)
        {
            RankOperator op = new RankOperator(null, null);
            op.OperatorToken = opToken;
            return op;
        }
    }

    #endregion
}
