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
            DLR.Expression expression;
            DYN.BindingRestrictions restriction;

            if (indexes.Length == 1 && indexes[0].HasValue && indexes[0].Value == null)
            {
                restriction = DYN.BindingRestrictions.GetExpressionRestriction(
                        DLR.Expression.Equal(indexes[0].Expression, DLR.Expression.Constant(null))
                );

                expression = target.Expression;
            }
            else
            {
                restriction = DYN.BindingRestrictions.GetTypeRestriction(
                    indexes[0].Expression, typeof(List<Types.AType>)
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
                                typeof(Int32).GetMethod("ToString", new Type[] { })
                            )
                        )
                    )
                );

                expression = DLR.Expression.Block(
                    rankCheck,
                    DLR.Expression.MakeIndex(
                        DLR.Expression.Convert(target.Expression, typeof(Types.AType)),
                        GetIndexBinder.AArrayIndexerProperty,
                        new DLR.Expression[] { indexer }
                    )
                );
            }

            DYN.DynamicMetaObject dynobj = new DYN.DynamicMetaObject(
                expression,
                DYN.BindingRestrictions.GetTypeRestriction(target.Expression, target.RuntimeType).Merge(restriction)
            );

            return dynobj;
        }
    }
}
