using System;

using System.Collections.Generic;
using System.Reflection;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Binder
{
    class GetIndexBinder : DYN.GetIndexBinder
    {
        private static readonly PropertyInfo AArrayIndexerProperty = typeof(Types.AType).GetIndexerProperty(typeof(List<Types.AType>));
        private static readonly PropertyInfo ListIndexerProperty = typeof(List<Types.AType>).GetIndexerProperty(typeof(int));

        public GetIndexBinder(DYN.CallInfo callinfo)
            : base(callinfo)
        {
        }

        public override DYN.DynamicMetaObject FallbackGetIndex(DYN.DynamicMetaObject target, DYN.DynamicMetaObject[] indexes, DYN.DynamicMetaObject errorSuggestion)
        {
            DYN.BindingRestrictions restriction = DYN.BindingRestrictions.GetTypeRestriction(
                target.Expression, target.RuntimeType
            );

            var indexer = DLR.Expression.Convert(indexes[0].Expression, typeof(List<Types.AType>));

            var rankCheck = DLR.Expression.IfThen(
                DLR.Expression.LessThan(
                    DLR.Expression.Property(target.Expression, "Rank"),
                    DLR.Expression.Property(indexer, "Count")
                ),
                DLR.Expression.Throw(
                    DLR.Expression.New(typeof(Error.Rank).GetConstructor(new Type[] { typeof(string) }), 
                    //DLR.Expression.Constant("[]")
                    // FIXME-LATER: This is just for debug, so remove it later:
                        DLR.Expression.Call(
                            DLR.Expression.Property(indexer, "Count"),
                            typeof(Int32).GetMethod("ToString", new Type[] {})
                        )
                    )
                )
            );        

            DLR.Expression expression = DLR.Expression.Block(
                rankCheck,
                DLR.Expression.MakeIndex(
                    DLR.Expression.Convert(target.Expression, typeof(Types.AType)), 
                    GetIndexBinder.AArrayIndexerProperty,
                    new DLR.Expression[] { indexer }
                )
            );


            var dynobj = new DYN.DynamicMetaObject(
                expression,
                restriction
            );


            return dynobj;

        }
    }
}
