using System.Collections.Generic;

using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Operator.Reduction
{
    abstract class Reduction : AbstractMonadicFunction
    {
        #region Variables

        /// <summary>
        /// Reduce function.
        /// </summary>
        protected AbstractDyadicFunction function;
        /// <summary>
        /// Result type, if right side length equal zero.
        /// </summary>
        protected ATypes type;
        /// <summary>
        /// The identity scalar.
        /// </summary>
        protected AType fillerElement;

        #endregion

        #region Abstract method

        /// <summary>
        /// This method sets the variables.
        /// </summary>
        /// <param name="type"></param>
        protected abstract void SetVariables(ATypes type);

        #endregion

        #region Entry point

        public override AType Execute(AType argument, Aplus environment = null)
        {
            SetVariables(argument.Type);

            AType result;

            //Identity case.
            if (argument.Length == 0)
            {
                result = Fill(argument.Shape.GetRange(1, argument.Rank - 1));
            }
            else
            {
                if (argument.Type != ATypes.AFloat && argument.Type != ATypes.AInteger)
                {
                    throw new Error.Type(TypeErrorText);
                }

                if (argument.IsArray)
                {
                    if (argument.Length == 1)
                    {
                        //Only one-element, we give back it.
                        result = Process(argument[0]);
                    }
                    else
                    {
                        //argumentArray[0] f argumentArray[1] f ... f argumentArray[-1 + #argumentArray]
                        result = argument[0];

                        for (int i = 1; i < argument.Length; i++)
                        {
                            result = function.Execute(result, argument[i]);
                        }
                    }

                }
                else
                {
                    result = Process(argument);
                }
            }

            return result;
        }

        #endregion

        #region Computation

        protected virtual AType Process(AType argument)
        {
            return argument.Clone();
        }

        /// <summary>
        /// If right side length equal zero, then the result
        /// is (1 drop x) rho identity, where identity is scalar
        /// that depends on function.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        private AType Fill(List<int> shape)
        {
            if (shape.Count > 0)
            {
                AType result = AArray.Create(this.type);

                for (int i = 0; i < shape[0]; i++)
                {
                    result.Add(Fill(shape.GetRange(1, shape.Count - 1)));
                }

                return result;
            }
            else
            {
                return fillerElement.Clone();
            }
        }

        #endregion
    }
}
