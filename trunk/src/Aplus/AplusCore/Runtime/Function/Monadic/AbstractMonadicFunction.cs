using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic
{
    abstract class AbstractMonadicFunction
    {
        /// <summary>
        /// Execute the Monadic function.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        abstract public AType Execute(AType argument, Aplus environment = null);
        

        #region Properties

        protected virtual string DomainErrorText { get { return this.GetType().Name; } }
        protected virtual string LengthErrorText { get { return this.GetType().Name; } }
        protected virtual string MaxRankErrorText { get { return this.GetType().Name; } }
        protected virtual string MismatchErrorText { get { return this.GetType().Name; } }
        protected virtual string RankErrorText { get { return this.GetType().Name; } }
        protected virtual string TypeErrorText { get { return this.GetType().Name; } }

        #endregion
    }
}
