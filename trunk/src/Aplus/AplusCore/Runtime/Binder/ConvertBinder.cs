using System;

using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Binder
{
    internal class ConvertBinder : DYN.ConvertBinder
    {
        public ConvertBinder(Type type)
            : base(type, /* explicit convert */false)
        {
        }

        public override DYN.DynamicMetaObject FallbackConvert(DYN.DynamicMetaObject target, DYN.DynamicMetaObject errorSuggestion)
        {
            DYN.BindingRestrictions restriction = DYN.BindingRestrictions.GetTypeRestriction(
                target.Expression, target.RuntimeType
            );

            // Found an AType, simply return the expression
            // We check if it implements the AType interface
            if (target.RuntimeType.GetInterface("AType") != null)
            {
                DYN.DynamicMetaObject result = new DYN.DynamicMetaObject(
                    DLR.Expression.Convert(target.Expression, typeof(AType)),
                    restriction
                );

                return result;
            }
            else if(target.RuntimeType.Name == "Int32")
            {
                DYN.DynamicMetaObject result = new DYN.DynamicMetaObject(
                    // Create a new AInteger from the input value (need to convert to int first)
                    DLR.Expression.Call(
                        typeof(AInteger).GetMethod("Create", new Type[] { typeof(int) }),
                        DLR.Expression.Convert(target.Expression, typeof(int))
                    ),
                    restriction
                );

                // .New AplusCore.Types.AInteger((System.Int32)$$arg0)
                return result;
            }
            else if (target.RuntimeType.Name == "String")
            {
                DYN.DynamicMetaObject result = new DYN.DynamicMetaObject(
                    DLR.Expression.Convert(target.Expression, typeof(AType),
                        typeof(Helpers).GetMethod("BuildString")
                    ),
                    restriction
                );

                return result;
            }

            // TODO::
            throw new NotImplementedException();
        }
    }
}
