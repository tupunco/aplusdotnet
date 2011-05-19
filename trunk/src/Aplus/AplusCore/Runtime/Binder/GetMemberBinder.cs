using System;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Binder
{
    internal class GetMemberBinder : DYN.GetMemberBinder
    {
        public GetMemberBinder(string name)
            : base(name, /*ignore case*/false)
        {
        }

        public override T BindDelegate<T>(System.Runtime.CompilerServices.CallSite<T> site, object[] args)
        {
            return base.BindDelegate<T>(site, args);
        }

        public override DYN.DynamicMetaObject FallbackGetMember(DYN.DynamicMetaObject target, DYN.DynamicMetaObject errorSuggestion)
        {
            DYN.BindingRestrictions restriction =
                DYN.BindingRestrictions.GetTypeRestriction(target.Expression, target.RuntimeType);

            string exceptionString = String.Format("GetMemberBinder: cannot bind member '{0}' on object '{1}'",
                                        this.Name, target.Value.ToString());

            DLR.Expression throwBlock =
                DLR.Expression.Throw(
                    DLR.Expression.New(
                        typeof(Error.Value).GetConstructor(new Type[] { typeof(string) }),
                        DLR.Expression.Constant(exceptionString)
                    ),
                    typeof(Error)
                );

            DYN.DynamicMetaObject dynobj =
                new DYN.DynamicMetaObject(
                    throwBlock,
                    restriction
                );

            return dynobj;
        }

    }
}
