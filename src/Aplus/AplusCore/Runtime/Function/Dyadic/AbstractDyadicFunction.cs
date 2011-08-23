using AplusCore.Types;

namespace AplusCore.Runtime.Function.Dyadic
{
    abstract class AbstractDyadicFunction
    {
        /// <summary>
        /// Execute the Dyadic function.
        /// The arguments are swaped by design to ensure the correct execution order of the arguments.
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        /// <returns></returns>
        abstract public AType Execute(AType right, AType left, Aplus environment = null);

        #region Properties

        protected virtual string DomainErrorText { get { return this.GetType().Name; } }
        protected virtual string IndexErrorText { get { return this.GetType().Name; } }
        protected virtual string LengthErrorText { get { return this.GetType().Name; } }
        protected virtual string MaxRankErrorText { get { return this.GetType().Name; } }
        protected virtual string NonceErrorText { get { return this.GetType().Name; } }
        protected virtual string RankErrorText { get { return this.GetType().Name; } }
        protected virtual string TypeErrorText { get { return this.GetType().Name; } }
        protected virtual string ValenceErrorText { get { return this.GetType().Name; } }

        #endregion
    }
}
