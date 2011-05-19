using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Informational
{
    class Depth : AbstractMonadicFunction
    {
        #region Entry point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            return AInteger.Create(Compute(argument));
        }

        #endregion

        #region Computation

        private int Compute(AType argument)
        {
            if (argument.IsArray)
            {
                int max = 0;

                foreach (AType item in argument)
                {
                    max = Math.Max(Compute(item), max);
                }
                return max;
            }
            else
            {
                //Box give one depth to whole depth and call recursively the Compute function to
                //determine the nested element depth.
                if (argument.IsBox)
                {
                    return argument.IsFunctionScalar ? 0 : 1 + Compute(argument.NestedItem);
                }
                else
                {
                    //Simple case.
                    return argument.Type == ATypes.AFunc ? -1 : 0;
                }
            }
        }

        #endregion
    }
}
