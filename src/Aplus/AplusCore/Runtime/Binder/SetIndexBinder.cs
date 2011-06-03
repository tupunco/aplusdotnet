using System;
using System.Collections.Generic;
using System.Reflection;

using AplusCore.Compiler;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Binder
{
    class SetIndexBinder : DYN.SetIndexBinder
    {
        private static readonly PropertyInfo AArrayIndexerProperty = typeof(Types.AType).GetIndexerProperty(typeof(List<Types.AType>));

        public SetIndexBinder(DYN.CallInfo callinfo)
            : base(callinfo)
        {
        }

        public override DYN.DynamicMetaObject FallbackSetIndex(DYN.DynamicMetaObject target, DYN.DynamicMetaObject[] indexes, DYN.DynamicMetaObject value, DYN.DynamicMetaObject errorSuggestion)
        {
            DLR.Expression expression;
            DYN.BindingRestrictions restriction;

            // TODO: Fix this: add restirction to AType interface
            //// check if the target implements the AType interface
            //if (target.RuntimeType.GetInterface("AType") == null)
            //{
            //    dynobj = new DYN.DynamicMetaObject(
            //        DLR.Expression.Block(
            //            DLR.Expression.Throw(
            //                DLR.Expression.New(
            //                    typeof(Error.Mismatch).GetConstructor(new Type[] { typeof(string) }),
            //                    DLR.Expression.Constant(
            //                        String.Format("No indexing defined on '{0}'", target.RuntimeType.ToString())
            //                    )
            //                )
            //            ),
            //            value.Expression
            //        ),
            //        restriction
            //    );

            //    return dynobj;
            //}

            if (indexes.Length == 1 && indexes[0].HasValue && indexes[0].Value == null)
            {
                restriction = DYN.BindingRestrictions.GetExpressionRestriction(
                        DLR.Expression.Equal(indexes[0].Expression, DLR.Expression.Constant(null))
                );

                BindingFlags searchFlags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

                expression = DLR.Expression.Block(
                    DLR.Expression.Call(
                        null,
                        typeof(Utils).GetMethod("PerformAssign", searchFlags),
                        DLR.Expression.Convert(target.Expression, typeof(AType)),
                        DLR.Expression.Convert(value.Expression, typeof(AType))
                    ),
                    value.Expression
                );
            }
            else
            {
                // $target[$indexes] = $value;
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
                    typeof(Types.AType),
                    rankCheck,
                    DLR.Expression.Assign(
                        DLR.Expression.MakeIndex(
                            DLR.Expression.Convert(Tools.CloneMemoryMappedFile(target.Expression), typeof(Types.AType)),
                            SetIndexBinder.AArrayIndexerProperty,
                            new DLR.Expression[] { indexer }
                        ),
                        value.Expression
                    ),
                    value.Expression
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
