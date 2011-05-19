using System;

using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Runtime.Binder
{
    internal class InvokeBinder : DYN.InvokeBinder
    {
        public InvokeBinder(DYN.CallInfo callInfo)
            : base(callInfo)
        {
        }

        public override T BindDelegate<T>(System.Runtime.CompilerServices.CallSite<T> site, object[] args)
        {
            return base.BindDelegate<T>(site, args);
        }


        public override DYN.DynamicMetaObject FallbackInvoke(DYN.DynamicMetaObject target, DYN.DynamicMetaObject[] args, DYN.DynamicMetaObject errorSuggestion)
        {

            if (target.HasValue && target.Value is AReference &&
                ((AReference)target.Value).Data is AFunc)
            {
                DLR.Expression dataExpression =
                    DLR.Expression.PropertyOrField(
                        DLR.Expression.Convert(target.Expression, typeof(AType)),
                        "Data"
                    );

                DYN.BindingRestrictions baseRestriction = DYN.BindingRestrictions.GetTypeRestriction(
 	                    target.Expression, target.LimitType
 	                );

                DYN.BindingRestrictions builtinRestriction;
                DYN.BindingRestrictions restriction;

                DLR.Expression targetExpression = DLR.Expression.Convert(dataExpression, typeof(AFunc));
                
                // Func<..> types (argument types + return type)
                Type[] callTypes;

                // Convert every function parameters to AType
                DLR.Expression[] callArguments;

                DLR.Expression codeBlock = null;

                if (((AFunc)((AReference)target.Value).Data).IsBuiltin)
                {
                    switch (this.CallInfo.ArgumentCount)
                    {
                        case 2:
                            callTypes = new Type[args.Length + 2];
                            callArguments = new DLR.Expression[args.Length + 1];
                            BuildArguments(args, callTypes, callArguments);
                            // Add the missing infos:
                            callTypes[2] = typeof(AType);
                            callArguments[2] = DLR.Expression.Constant(null, typeof(AType));
                            // Build the codeblock for the method invoke
                            codeBlock = BuildInvoke(targetExpression, callTypes, callArguments);
                            break;
                        case 3:
                            callTypes = new Type[args.Length + 1];
                            callArguments = new DLR.Expression[args.Length];
                            BuildArguments(args, callTypes, callArguments);
                            // Build the codeblock for the method invoke
                            codeBlock = BuildInvoke(targetExpression, callTypes, callArguments);
                            break;
                        default:
                            // Every other case is invalid...
                            codeBlock = DLR.Expression.Block(
                                DLR.Expression.Throw(
                                    DLR.Expression.New(
                                        typeof(Error.Valence).GetConstructor(new Type[] { typeof(string) }),
                                        DLR.Expression.Property(targetExpression, "Name")
                                    )
                                ),
                                DLR.Expression.Default(typeof(AType))
                            );
                            break;
                    }

                    builtinRestriction = DYN.BindingRestrictions.GetExpressionRestriction(
                        DLR.Expression.IsTrue(DLR.Expression.Property(targetExpression, "IsBuiltin"))
                    );
                }
                else
                {
                    callTypes = new Type[args.Length + 1];
                    callArguments = new DLR.Expression[args.Length];

                    BuildArguments(args, callTypes, callArguments);

                    codeBlock = DLR.Expression.Block(
                        // Check if the function we are calling have the correct valence
                        DLR.Expression.IfThen(
                            DLR.Expression.NotEqual(
                                DLR.Expression.Property(targetExpression, "Valence"),
                                DLR.Expression.Constant(this.CallInfo.ArgumentCount)
                            ),
                    // Function's valence is incorrect throw error
                            DLR.Expression.Throw(
                                DLR.Expression.New(
                                    typeof(Error.Valence).GetConstructor(new Type[] { typeof(string) }),
                                    DLR.Expression.Property(targetExpression, "Name")
                                )
                            )
                        ),
                    // Function's valence is OK
                    // Call the function
                        BuildInvoke(targetExpression, callTypes, callArguments)
                    );

                    builtinRestriction = DYN.BindingRestrictions.GetExpressionRestriction(
                        DLR.Expression.IsFalse(DLR.Expression.Property(targetExpression, "IsBuiltin"))
                    );
                }

                restriction = baseRestriction.Merge(builtinRestriction);

                DYN.DynamicMetaObject dynobj = new DYN.DynamicMetaObject(codeBlock, restriction);

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

        private static DLR.Expression BuildInvoke(DLR.Expression targetExpression, Type[] callTypes, DLR.Expression[] callArguments)
        {
            return DLR.Expression.Invoke(
                    DLR.Expression.Convert(
                        DLR.Expression.Property(targetExpression, "Method"),
                        DLR.Expression.GetFuncType(callTypes)
                    ),
                    callArguments
                );
        }


        private static void BuildArguments(DYN.DynamicMetaObject[] args, Type[] callTypes, DLR.Expression[] callArguments)
        {
            // Convert the scope argument
            callArguments[0] = DLR.Expression.Convert(args[0].Expression, args[0].RuntimeType);
            callTypes[0] = args[0].RuntimeType;

            // Convert the other arguments to ATypes
            for (int i = 1; i < args.Length; i++)
            {
                callArguments[i] = DLR.Expression.Convert(args[i].Expression, typeof(AType));
                callTypes[i] = typeof(AType);
            }

            // Return Type:
            callTypes[callTypes.Length - 1] = typeof(AType);
        }
    }
}
