using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Scripting.Utils;

using AplusCore.Compiler;
using AplusCore.Runtime.Callback;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Binder
{
    internal class CallbackBinder : DYN.DynamicMetaObjectBinder
    {
        public override T BindDelegate<T>(System.Runtime.CompilerServices.CallSite<T> site, object[] args)
        {
            return base.BindDelegate<T>(site, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The <see cref="CallbackItem"/> to use.</param>
        /// <param name="args"></param>
        /// <remarks>
        /// Order of items in the <see cref="args"/> array:
        /// <list type="number">
        ///  <item><see cref="Aplus"/></item>
        ///  <item>New value for the global variable</item>
        /// </list>
        /// </remarks>
        /// <returns></returns>
        public override DYN.DynamicMetaObject Bind(DYN.DynamicMetaObject target, DYN.DynamicMetaObject[] args)
        {
            ContractUtils.RequiresNotNull(target, "target");
            ContractUtils.RequiresNotNullItems(args, "args");

            if (target.HasValue && target.Value is CallbackItem)
            {
                CallbackItem callbackItem = (CallbackItem)target.Value;
                AFunc callbackAFunction = (AFunc)callbackItem.CallbackFunction.Data;

                DLR.Expression callbackItemExpression = DLR.Expression.Convert(
                    target.Expression, typeof(CallbackItem)
                );


                DYN.BindingRestrictions baseRestriction = DYN.BindingRestrictions.GetTypeRestriction(
                        target.Expression, target.LimitType
                    );

                DLR.Expression callbackAFunctionExpression = DLR.Expression.Convert(
                    DLR.Expression.Convert(target.Expression, typeof(CallbackItem))
                        .Property("CallbackFunction").Property("Data"),
                    typeof(AFunc)
                );

                DYN.BindingRestrictions valenceRestriction =
                    DYN.BindingRestrictions.GetExpressionRestriction(
                        DLR.Expression.Equal(
                            callbackAFunctionExpression.Property("Valence"),
                            DLR.Expression.Constant(callbackAFunction.Valence)
                        )
                    );

                // TODO: refactor the argument generation, something similar is in the InvokeBinder

                IEnumerable<DLR.Expression> callbackBaseArguments = new DLR.Expression[] {
                    callbackItemExpression.Property("StaticData"), // static data
                    DLR.Expression.Convert(args[1].Expression, typeof(AType)), // new value
                    DLR.Expression.Constant(Utils.ANull(), typeof(AType)), // index/set of indices
                    DLR.Expression.Constant(Utils.ANull(), typeof(AType)), // path (left argument of pick)
                    DLR.Expression.Constant(Utils.ANull(), typeof(AType)), // context of the global variable
                    DLR.Expression.Constant(Utils.ANull(), typeof(AType)) // name of the global variable  
                }.Where((item, i) => i < callbackAFunction.Valence - 1).Reverse();
                List<DLR.Expression> callbackArguments = new List<DLR.Expression>();
                
                // Aplus
                callbackArguments.Add(DLR.Expression.Convert(args[0].Expression, args[0].RuntimeType));
                callbackArguments.AddRange(callbackBaseArguments);


                Type[] callTypes = new Type[callbackAFunction.Valence + 1];
                callTypes[0] = typeof(Aplus);

                for (int i = 1; i < callbackAFunction.Valence; i++)
                {
                    callTypes[i] = typeof(AType);
                }

                // return type
                callTypes[callbackAFunction.Valence] = typeof(AType);


                DLR.Expression codeBlock = DLR.Expression.Invoke(
                    DLR.Expression.Convert(
                        callbackAFunctionExpression.Property("Method"),
                        DLR.Expression.GetFuncType(callTypes)
                    ),
                    callbackArguments
                );

                DYN.DynamicMetaObject dynobj =
                    new DYN.DynamicMetaObject(codeBlock, baseRestriction.Merge(valenceRestriction));

                return dynobj;
            }

            // Throw a non-function error
            return new DYN.DynamicMetaObject(
                DLR.Expression.Throw(
                    DLR.Expression.New(
                        typeof(Error.NonFunction).GetConstructor(new Type[] { typeof(string) }),
                        DLR.Expression.Call(
                            target.Expression,
                            typeof(object).GetMethod("ToString")
                        )
                    ),
                    typeof(Error.NonFunction)
                ),
                DYN.BindingRestrictions.GetTypeRestriction(target.Expression, target.RuntimeType)
            );
        }
    }
}
