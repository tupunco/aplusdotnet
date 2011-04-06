using System;
using System.Collections.Generic;
using System.Linq;
using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;
using AplusCore.Runtime;
using AplusCore.Types;
using AplusCore.Runtime.Function.Monadic;
using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Runtime.Function.Monadic.NonScalar.Other;
using AplusCore.Runtime.Function.Dyadic.NonScalar.Other;

namespace AplusCore.Compiler.AST
{
    public class Assign : Node
    {
        #region Variables

        private Node target;
        private Node expression;

        #endregion

        #region Properties

        public Node Target { get { return this.target; } }

        #endregion

        #region Constructor
        public Assign(Node target, Node expression)
        {
            this.target = target;
            this.expression = expression;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result = null;
            // Clone the rhs value of the assignment to ensure correct results
            // in case of: a:=b:=[...:=] [rhs]  assignments
            DLR.Expression value = DLR.Expression.Call(
                this.expression.Generate(scope), typeof(AType).GetMethod("Clone")
            );

            if (this.target is Identifier)
            {
                result = GenerateIdentifierAssign(scope, (AST.Identifier)this.target, value);
            }
            else if (this.target is Strand)
            {
                result = GenerateStrandAssign(scope, (Strand)this.target, value);
            }
            else if (this.target is Indexing)
            {
                Indexing target = (Indexing)this.target;

                // in case of:  a[,] := ...
                if (target.IndexExpression[0] is BuiltInFunction)
                {
                    BuiltInFunction function = (BuiltInFunction)target.IndexExpression[0];
                    if (function.Function.Type != Grammar.Tokens.RAVEL)
                    {
                        // incorrect function inside the index expression
                        throw new ParseException("Incorrect function", false);
                    }

                    var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
                    result = DLR.Expression.Call(
                        typeof(Assign).GetMethod("AppendItem", flags),
                        value,
                        target.Item.Generate(scope)
                    );
                }
                else
                {
                    result = GenerateIndexAssign(scope, target, value);
                }
            }
            else if(Node.TestMonadicToken(this.target, Grammar.Tokens.VALUE))
            {
                var method = typeof(Value).GetMethod("Assign");
                var targetDLR = ((MonadicFunction)this.target).Expression.Generate(scope);

                result = DLR.Expression.Call(
                    method,
                    targetDLR,
                    value,
                    scope.GetAplusEnvironment()
                );
            }
            else if(Node.TestDyadicToken(this.target, Grammar.Tokens.VALUEINCONTEXT))
            {
                var targetNode = (DyadicFunction)this.target;
                var method = typeof(ValueInContext).GetMethod("Assign");
                var targetDLR = targetNode.Right.Generate(scope);
                var contextNameDLR = targetNode.Left.Generate(scope);

                result = DLR.Expression.Call(
                    method,
                    targetDLR,
                    contextNameDLR,
                    value,
                    scope.GetAplusEnvironment()
                );
            }
            else if(Node.TestDyadicToken(this.target, Grammar.Tokens.PICK))
            {
                result = BuildPickAssign(scope, value);
            }
            else
            {
                var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
                // switch on Assignment flag
                scope.IsAssignment = true;

                // Find the correct method for assignment
                // for pick we need a different method currently..
                var method = typeof(Assign).GetMethod("AssignHelper", flags);

                result = DLR.Expression.Call(
                    method,
                    value,
                    this.target.Generate(scope)
                );

                // switch off Assignment flag
                scope.IsAssignment = false;
            }

            return result;
        }

        #endregion

        #region DLR assign

        private DLR.Expression BuildPickAssign(AplusScope scope, DLR.Expression value)
        {
            DyadicFunction function = (DyadicFunction)this.target;
            bool isValueinContext = Node.TestDyadicToken(function.Left, Grammar.Tokens.VALUEINCONTEXT);
            bool isValue = Node.TestMonadicToken(function.Right, Grammar.Tokens.VALUE);

            if (!(function.Right is Identifier || isValueinContext || isValue))
            {
                throw new ParseException("assign pick target", false);
            }

            DLR.Expression left = function.Left.Generate(scope);
            DLR.Expression right = function.Right.Generate(scope);

            var method = DyadicFunctionInstance.Pick;
            DLR.Expression pickDLR = DLR.Expression.Call(
                DLR.Expression.Constant(method),
                method.GetType().GetMethod("AssignExecute"),
                right,
                left,
                scope.GetAplusEnvironment()
            );

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;

            DLR.Expression result = DLR.Expression.Call(
                typeof(Assign).GetMethod("PickAssignHelper", flags),
                value,
                pickDLR
            );

            return result;
        }

        internal static AType PickAssignHelper(AType value, PickAssignmentTarget pickResult)
        {
            if (pickResult.FromBox)
            {
                pickResult.Target.Data = value.Data;
            }
            else
            {
                Utils.PerformAssign(pickResult.Target, value);
            }
            return value;
        }

        internal static AType AssignHelper(AType value, AType target)
        {
            //TODO
            Utils.PerformAssign(target, value);
            return value;
        }

        #endregion

        #region DLR: append

        private static AType AppendItem(AType value, AType target)
        {
            if (target.Rank == 0)
            {
                throw new Error.Rank("assign");
            }

            if (!Utils.DifferentNumberType(target, value) && target.Type != value.Type)
            {
                // The target and value are not numbers and they are of a different type?
                throw new Error.Type("assign");
            }

            if (target.Shape.SequenceEqual<int>(value.Shape))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    target.Add(value[i].Clone());
                }
            }
            else if (target.Shape.GetRange(1, target.Shape.Count - 1).SequenceEqual<int>(value.Shape))
            {
                target.Add(value.Clone());
            }
            else if (value.Rank == 0)
            {
                AType item = DyadicFunctionInstance.Reshape.Execute(
                    value,
                    target.Shape.GetRange(1, target.Shape.Count - 1).ToAArray()
                );
                target.Add(item);
            }
            else if (value.Rank == target.Rank)
            {
                target.AddRange(value);
            }
            else
            {
                throw new Error.Length("assign");
            }

            return value;
        }

        #endregion

        #region DLR: Assing to indexed target

        private static DLR.Expression GenerateIndexAssign(AplusScope scope, Indexing target, DLR.Expression value)
        {
            DLR.Expression result = DLR.Expression.Convert(
                DLR.Expression.Dynamic(
                    scope.GetRuntime().SetIndexBinder(new System.Dynamic.CallInfo(target.IndexExpression.Length)),
                    typeof(object),
                    target.Item.Generate(scope),
                    DLR.Expression.Call(
                        typeof(Helpers).GetMethod("BuildIndexerArray"),
                        DLR.Expression.NewArrayInit(
                            typeof(AType),
                            target.IndexExpression.Items.Reverse().Select(item => { return item.Generate(scope); })
                        )
                    ),
                    value
                ),
                typeof(AType)
            );

            return result;
        }


        #endregion

        #region DLR: Assign to Strand

        /// <summary>Generates a DLR Expression for Strand assignment.</summary>
        /// <remarks>
        ///  Transform this:
        /// (a;b;...) := .....    // don't care what is there :)
        /// 
        /// to this in DLR:
        /// 
        /// $__VALUES__ = (AType) .....; // ..... is a generated DLR expression now!
        /// 
        /// if($__VALUES__.IsArray)     // case A
        /// {
        ///     if($VALUES.length != 2)  // 2 the # of targets
        ///     {
        ///         throw new Error.Length("assign");
        ///     }
        ///     
        ///     a = disclose( ((AArray)$__VALUES__)[0] );
        ///     b = disclose( ((AArray)$__VALUES__)[1] );
        ///     ...
        /// }
        /// else    // case B
        /// {
        ///     a = $VALUES;
        ///     b = $VALUES;
        ///     ...
        /// }
        /// </remarks>
        /// <param name="scope"></param>
        /// <param name="targets">
        /// Strand containing Identifiers.
        /// If a node which in not an Identifier found, ParseException is thrown
        /// </param>
        /// <param name="value">The generated value which will be assigned to the targets</param>
        /// <returns>A generated DLR expression for strand assignment</returns>
        private static DLR.Expression GenerateStrandAssign(AplusScope scope, Strand targets, DLR.Expression value)
        {
            AbstractMonadicFunction disclose = MonadicFunctionInstance.Disclose;
            Aplus runtime = scope.GetRuntime();
            DLR.ParameterExpression environment = scope.GetAplusEnvironment();

            DLR.ParameterExpression valuesParam = DLR.Expression.Parameter(typeof(AType), "__VALUES__");

            // for case A) assigns
            List<DLR.Expression> strand2StrandAssigns = new List<DLR.Expression>();
            // for case B) assigns
            List<DLR.Expression> strand2ValueAssigns = new List<DLR.Expression>();
            int indexCounter = 0;
            foreach (Node node in targets.Items)
            {
                if (!(node is Identifier))
                {
                    throw new ParseException("assign lhs");
                }
                Identifier id = (Identifier)node;

                DLR.Expression strandValue =
                    DLR.Expression.Call(
                        DLR.Expression.Constant(disclose),
                        disclose.GetType().GetMethod("Execute"),
                        DLR.Expression.MakeIndex(
                            valuesParam,
                            typeof(AType).GetIndexerProperty(typeof(int)), // The indexer property which will be used
                            new DLR.Expression[] { DLR.Expression.Constant(indexCounter, typeof(int)) }
                        ),
                        environment
                );

                // case A) $TARGETS[i] = disclose($VALUES[i])
                strand2StrandAssigns.Add(GenerateIdentifierAssign(scope, id, strandValue));

                // case B) $TARGETS[i] = $VALUE
                strand2ValueAssigns.Add(GenerateIdentifierAssign(scope, id, valuesParam));

                indexCounter++;
            }

            // case A)
            DLR.Expression caseStrand2Strand =
                DLR.Expression.Block(
                    typeof(void),
                    DLR.Expression.IfThenElse(
                        DLR.Expression.NotEqual(
                            DLR.Expression.PropertyOrField(valuesParam, "Length"),
                            DLR.Expression.Constant(indexCounter, typeof(int))
                        ),
                        DLR.Expression.Throw(
                            DLR.Expression.New(
                                typeof(Error.Length).GetConstructor(new Type[] { typeof(string) }),
                                DLR.Expression.Constant("assign", typeof(string))
                            )
                        ),
                        DLR.Expression.Block(
                            typeof(void),
                            strand2StrandAssigns
                        )
                    )
            );

            DLR.Expression result = DLR.Expression.Block(
                new DLR.ParameterExpression[] { valuesParam },
                DLR.Expression.Assign(valuesParam, DLR.Expression.Convert(value, typeof(AType))),
                DLR.Expression.Condition(
                    DLR.Expression.IsTrue(DLR.Expression.PropertyOrField(valuesParam, "IsArray")),
                // case A)
                    caseStrand2Strand,
                // case B)
                    DLR.Expression.Block(typeof(void), strand2ValueAssigns),
                    typeof(void)
                ),
                valuesParam
            );

            return result;
        }

        #endregion

        #region DLR: Assign to Identifier

        internal static DLR.Expression GenerateIdentifierAssign(AplusScope scope, Identifier target, DLR.Expression value)
        {
            Aplus runtime = scope.GetRuntime();
            DLR.Expression result = null;

            if (scope.IsMethod)
            {
                // If the generation is inside of a method
                // check if it has the variablename as function parameter
                DLR.Expression functionParameter = scope.FindIdentifier(target.Name);
                if (functionParameter != null)
                {
                    // Found variable as a function parameter, do a simple assign
                    // the variable is already defined in AST.UserDefFunction
                    return DLR.Expression.Assign(functionParameter, value);
                }

                DLR.Expression functionScopeParam = scope.GetModuleExpression();
                DLR.Expression globalScopeParam = scope.Parent.GetModuleExpression();

                switch (target.Type)
                {
                    case IdentifierType.UnQualifiedName:
                        // Need to check if we are inside an eval block:
                        if (scope.IsEval)
                        {
                            // Code is inside an eval block, we behave slightly differently!

                            #region description about what are we doing in this case

                            //
                            // Check if the variable exists in the function's scope
                            //  |-> Exists: set that variable's value
                            //  |-> otherwise: set the global variable's value
                            //
                            // if(((IDictionary<String, Object>)($FunctionScope).ContainsKey($VARIABLE))
                            // {
                            //      $FunctionScope.$VARIABLE = $VALUE;
                            // }
                            // else
                            // {
                            //      $GlobalScope.$VARIABLE = $VALUE;
                            // }
                            //
                            #endregion

                            result = DLR.Expression.Condition(
                                // Check if the varaible exists in the function scope
                                DLR.Expression.Call(
                                    DLR.Expression.Convert(functionScopeParam, typeof(IDictionary<string, object>)),
                                    typeof(IDictionary<string, object>).GetMethod("ContainsKey"),
                                    DLR.Expression.Constant(target.Name)
                                ),
                                // found the variable in function scope, so assign a value to it
                                DLR.Expression.Dynamic(
                                    runtime.SetMemberBinder(target.Name),
                                    typeof(object),
                                    functionScopeParam,
                                    value
                                ),
                                // did NOT found the variable in the function scope
                                // perform the assignment in the global scope
                                VariableHelper.SetVariable(
                                    runtime,
                                    globalScopeParam, // Global scope
                                    target.CreateContextNames(runtime.CurrentContext),
                                    value
                                )
                            );

                        }
                        else if (target.IsEnclosed)
                        {
                            result = VariableHelper.SetVariable(
                                runtime,
                                globalScopeParam,
                                target.CreateContextNames(runtime.CurrentContext),
                                value
                            );
                            break;
                        }
                        else
                        {
                            // Simple case, we are inside a user defined function, but not inside an eval block
                            result = DLR.Expression.Dynamic(
                                runtime.SetMemberBinder(target.Name),
                                typeof(object),
                                functionScopeParam,
                                value
                            );
                        }
                        break;

                    case IdentifierType.QualifiedName:
                    case IdentifierType.SystemName:
                    default:
                        // Do an assignment on the global scope
                        result = VariableHelper.SetVariable(
                            runtime,
                            globalScopeParam,
                            target.CreateContextNames(runtime.CurrentContext),
                            value
                        );
                        break;
                }
            }
            else
            {

                result = VariableHelper.SetVariable(
                    runtime,
                    scope.GetModuleExpression(),
                    target.CreateContextNames(runtime.CurrentContext),
                    value
                );
            }


            return DLR.Expression.Convert(result, typeof(AType));
        }

        #endregion

        #region Assignment DLR Helpers

        /// <summary>
        /// Builds DLR expression for:  (iota rho argument)
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        internal static DLR.Expression BuildIndicesList(AplusScope scope, DLR.Expression argument)
        {
            var execute = typeof(AbstractMonadicFunction).GetMethod("Execute");

            // (iota rho x)
            DLR.Expression indexes = DLR.Expression.Call(
                DLR.Expression.Constant(MonadicFunctionInstance.Interval),
                execute,
                DLR.Expression.Call(
                    DLR.Expression.Constant(MonadicFunctionInstance.Shape),
                    execute,
                    argument,
                    scope.GetAplusEnvironment()
                ),
                scope.GetAplusEnvironment()
            );

            return indexes;
        }

        /// <summary>
        /// Build DLR expression for: (,target)[indexExpression]
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="target"></param>
        /// <param name="indexExpression">The indexer expression containing the expression returned from <see cref="BuildIndicesList"/></param>
        /// <returns></returns>
        internal static DLR.Expression BuildIndexing(AplusScope scope, DLR.Expression target, DLR.Expression indexExpression)
        {
            var execute = typeof(AbstractMonadicFunction).GetMethod("Execute");
            DLR.ParameterExpression indexes = DLR.Expression.Parameter(typeof(AType), "_INDEX_");

            // (,x)
            DLR.Expression raveledRight = DLR.Expression.Call(
                DLR.Expression.Constant(MonadicFunctionInstance.Ravel),
                execute,
                target,
                scope.GetAplusEnvironment()
            );

            // (,x)[indexExpression] := ..
            DLR.Expression result = DLR.Expression.Convert(
                DLR.Expression.Dynamic(
                    scope.GetRuntime().GetIndexBinder(new DYN.CallInfo(1)),
                    typeof(object),
                    raveledRight,
                    DLR.Expression.Call(
                        typeof(Helpers).GetMethod("BuildIndexerArray"),
                        DLR.Expression.NewArrayInit(typeof(AType), indexExpression)
                    )
                ),
                typeof(AType)
            );

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Assign({0} {1})", this.target, this.expression);

        }

        public override bool Equals(object obj)
        {
            if (obj is Assign)
            {
                Assign other = (Assign)obj;
                return this.target.Equals(other.target) && this.expression.Equals(other.expression);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.target.GetHashCode() ^ this.expression.GetHashCode();
        }

        #endregion

        #region GraphViz output (Only in DEBUG)
#if DEBUG
        static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder text)
        {
            string thisID = String.Format("Assign{0}", counter++);
            string targetID = target.ToDot(thisID, text);
            string expressionID = expression.ToDot(thisID, text);

            //text.AppendFormat("{0} -> {1}", parent, thisNode);
            text.AppendFormat("  {0} [label=\":=\"];\n", thisID);
            text.AppendFormat("  {0} -> {1};\n", thisID, targetID);
            text.AppendFormat("  {0} -> {1};\n", thisID, expressionID);

            return thisID;
        }
#endif
        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static Assign Assign(Node target, Node expression)
        {
            return new Assign(target, expression);
        }

    }

    #endregion
}
