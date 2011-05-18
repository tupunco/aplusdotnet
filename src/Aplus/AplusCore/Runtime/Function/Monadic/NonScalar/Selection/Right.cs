using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Selection
{
    class Right : AbstractMonadicFunction
    {
        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            return !String.IsNullOrEmpty(argument.MemoryMappedFile) ?
                argument.Clone() :
                argument;
        }
    }
}
