using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AplusCore.Runtime;
using AplusCore.Runtime.Callback;
using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Runtime.Function.Dyadic.NonScalar.Other;
using AplusCore.Runtime.Function.Monadic;
using AplusCore.Runtime.Function.Monadic.NonScalar.Other;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents an Assignment in an A+ AST.
    /// </summary>
    public class Assign : Node
    {
        #region Variables

        private Node target;
        private Node expression;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="NodeTypes">type</see> of the Node.
        /// </summary>
        public override NodeTypes NodeType
        {
            get { return NodeTypes.Assign; }
        }

        /// <summary>
        /// Gets the target of the assignment.
        /// </summary>
        public Node Target
        {
            get { return this.target; }
        }

        /// <summary>
        /// Gets the value of the assignment.
        /// </summary>
        public Node Expression
        {
            get { return this.expression; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Assign"/> AST node.
        /// </summary>
        /// <param name="target">The target of the assignment.</param>
        /// <param name="expression">The value of the assignment.</param>
        public Assign(Node target, Node expression)
        {
            this.target = target;
            this.expression = expression;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.ParameterExpression value = DLR.Expression.Parameter(typeof(AType), "__VALUE__");
            DLR.ParameterExpression targetParameter = DLR.Expression.Parameter(typeof(AType), "__TARGET__");
            DLR.ParameterExpression assignDone = DLR.Expression.Parameter(typeof(bool), "__ASSIGNDONE__");
            DLR.ParameterExpression prevAssignDone = scope.AssignDone;

            scope.AssignDone = assignDone;
            CallbackInfoStorage savedCallbackInfos = scope.CallbackInfo;

            CallbackInfoStorage callbackInfos =
                new CallbackInfoStorage()
                {
                    Index = DLR.Expression.Parameter(typeof(AType), "__CALLBACKINDEX__"),
                    QualifiedName = DLR.Expression.Parameter(typeof(string), "__QUALIFIEDNAME__"),
                    Path = DLR.Expression.Parameter(typeof(AType), "__PATH__"),
                    NonPresetValue = DLR.Expression.Parameter(typeof(AType), "__NONPRESETVALUE__")
                };

            scope.CallbackInfo = callbackInfos;

            // Clone the rhs value of the assignment to ensure correct results
            // in case of: a:=b:=[...:=] [rhs]  assignments
            DLR.ParameterExpression environment = scope.GetRuntimeExpression();
            DLR.Expression result = null;

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
                if (target.IndexExpression != null && target.IndexExpression[0] is BuiltInFunction)
                {
                    BuiltInFunction function = (BuiltInFunction)target.IndexExpression[0];
                    if (function.Function.Type != Grammar.Tokens.RAVEL)
                    {
                        // incorrect function inside the index expression
                        throw new ParseException("Incorrect function", false);
                    }

                    var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;

                    scope.IsAssignment = true;
                    DLR.Expression item = target.Item.Generate(scope);
                    scope.IsAssignment = false;

                    DLR.ParameterExpression param = DLR.Expression.Parameter(typeof(AType));
                    DLR.ParameterExpression errorParam = DLR.Expression.Parameter(typeof(Error.Signal));
                    DLR.Expression callback = BuildCallbackCall(scope, value);
                    DLR.Expression presetCallback = BuildPresetCallbackCall(scope, value);

                    result =
                        DLR.Expression.Block(
                            new DLR.ParameterExpression[] { param },
                            DLR.Expression.Assign(param, item),
                            DLR.Expression.Call(
                                typeof(Assign).GetMethod("CalculateIndexes", flags),
                                value,
                                param,
                                environment,
                                scope.CallbackInfo.Index
                            ),
                            DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(false)),
                            DLR.Expression.TryCatch(
                                DLR.Expression.Block(
                                    typeof(void),
                                    DLR.Expression.Assign(value, presetCallback),
                                    DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true))
                                ),
                                DLR.Expression.Catch(
                                    errorParam,
                                    DLR.Expression.Empty()
                                )
                            ),
                            DLR.Expression.IfThen(
                                scope.AssignDone,
                                DLR.Expression.Block(
                                    DLR.Expression.Call(
                                        typeof(Assign).GetMethod("AppendItem", flags),
                                        value,
                                        param,
                                        environment
                                    ),
                                    callback
                                )
                            ),
                            scope.CallbackInfo.NonPresetValue
                        );
                }
                else
                {
                    result = GenerateIndexAssign(scope, target, value);
                }
            }
            else if (Node.TestMonadicToken(this.target, Grammar.Tokens.VALUE))
            {
                var method = typeof(Value).GetMethod("Assign");
                var targetDLR = ((MonadicFunction)this.target).Expression.Generate(scope);

                DLR.Expression callback = BuildCallbackCall(scope, value);
                DLR.Expression presetCallback = BuildPresetCallbackCall(scope, value);

                DLR.ParameterExpression errorParam = DLR.Expression.Parameter(typeof(Error.Signal));
                DLR.ParameterExpression target = DLR.Expression.Parameter(typeof(AType), "__VALUETARGET__");

                DLR.Expression nameMaker =
                    DLR.Expression.Call(
                        VariableHelper.BuildValueQualifiedNameMethod,
                        DLR.Expression.Constant("."),
                        target.Property("asString")
                    );

                result =
                    DLR.Expression.Block(
                        new DLR.ParameterExpression[] { target },
                        DLR.Expression.Assign(target, targetDLR),
                        DLR.Expression.Assign(scope.CallbackInfo.QualifiedName, nameMaker),
                        DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(false)),
                        DLR.Expression.TryCatch(
                            DLR.Expression.Block(
                                typeof(void),
                                DLR.Expression.Assign(value, presetCallback),
                                DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true))
                            ),
                            DLR.Expression.Catch(
                                errorParam,
                                DLR.Expression.Empty()
                            )
                        ),
                        DLR.Expression.IfThen(
                            scope.AssignDone,
                            DLR.Expression.Block(
                                DLR.Expression.Call(
                                    method,
                                    target,
                                    value,
                                    environment
                                ),
                                callback
                            )
                        ),
                        scope.CallbackInfo.NonPresetValue
                    );
            }
            else if (Node.TestDyadicToken(this.target, Grammar.Tokens.VALUEINCONTEXT))
            {
                var targetNode = (DyadicFunction)this.target;
                var method = typeof(ValueInContext).GetMethod("Assign");
                var targetDLR = targetNode.Right.Generate(scope);
                var contextNameDLR = targetNode.Left.Generate(scope);

                DLR.Expression callback = BuildCallbackCall(scope, value);
                DLR.Expression presetCallback = BuildPresetCallbackCall(scope, value);

                DLR.ParameterExpression target = DLR.Expression.Parameter(typeof(AType), "__VALUETARGET__");
                DLR.ParameterExpression contextName = DLR.Expression.Parameter(typeof(AType), "__CONTEXTNAME__");
                DLR.ParameterExpression errorParam = DLR.Expression.Parameter(typeof(Error.Signal));

                DLR.Expression nameMaker =
                    DLR.Expression.Call(
                        VariableHelper.BuildValueQualifiedNameMethod,
                        contextName.Property("asString"),
                        target.Property("asString")
                    );

                result =
                    DLR.Expression.Block(
                        new DLR.ParameterExpression[] { target, contextName },
                        DLR.Expression.Assign(target, targetDLR),
                        DLR.Expression.Assign(contextName, contextNameDLR),
                        DLR.Expression.Assign(
                            scope.CallbackInfo.QualifiedName,
                            nameMaker
                       ),
                        DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(false)),
                        DLR.Expression.TryCatch(
                            DLR.Expression.Block(
                                typeof(void),
                                DLR.Expression.Assign(value, presetCallback),
                                DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true))
                            ),
                            DLR.Expression.Catch(
                                errorParam,
                                DLR.Expression.Empty()
                            )
                        ),
                        DLR.Expression.IfThen(
                            scope.AssignDone,
                            DLR.Expression.Block(
                                DLR.Expression.Call(
                                    method,
                                    target,
                                    contextName,
                                    value,
                                    environment
                                ),
                                callback
                            )
                        ),
                        scope.CallbackInfo.NonPresetValue
                    );
            }
            else if (Node.TestDyadicToken(this.target, Grammar.Tokens.PICK))
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

                DLR.Expression callback = AST.Assign.BuildCallbackCall(scope, value);
                DLR.Expression presetCallback = BuildPresetCallbackCall(scope, value);

                DLR.ParameterExpression temp = DLR.Expression.Parameter(typeof(AType));
                DLR.ParameterExpression errorParam = DLR.Expression.Parameter(typeof(Error.Signal));

                result =
                    DLR.Expression.Block(
                        new DLR.ParameterExpression[] { temp },
                        DLR.Expression.Assign(temp, this.target.Generate(scope)),
                        DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(false)),
                        DLR.Expression.TryCatch(
                            DLR.Expression.Block(
                                typeof(void),
                                DLR.Expression.Assign(value, presetCallback),
                                DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true))
                            ),
                            DLR.Expression.Catch(
                                errorParam,
                                DLR.Expression.Empty()
                            )
                        ),
                        DLR.Expression.IfThen(
                            scope.AssignDone,
                            DLR.Expression.Block(
                                DLR.Expression.Call(
                                    method,
                                    value,
                                    temp
                                ),
                                callback
                            )
                        )
                    );

                // switch off Assignment flag
                scope.IsAssignment = false;
            }

            // error parameter
            DLR.ParameterExpression errorVariable = DLR.Expression.Parameter(typeof(Error.Signal), "error");

            /**
             * The dark magic behind callbacks:
             * $value := the value of the right side of assignment
             * $target := the left side of the assignment
             * $qualifiedName := the name of the assigned variable
             * $index := the index of the assigned variable (if any)
             * $path := the path of the assigned variable (if any)
             * 
             * example for preset callback assignment
             * a := x := y
             * if we have a preset callback on x, then the x will get the value of the preset callback on x,
             * a will be y (or the value of the preset callback on a)
             * in any case we will result y
             */

            DLR.Expression blockedResult =
                DLR.Expression.Block(
                    new DLR.ParameterExpression[] { value, scope.AssignDone,
                        scope.CallbackInfo.QualifiedName, scope.CallbackInfo.Index,
                        scope.CallbackInfo.Path,scope.CallbackInfo.NonPresetValue },
                    DLR.Expression.Assign(scope.CallbackInfo.QualifiedName, DLR.Expression.Constant("")),
                    DLR.Expression.Assign(scope.CallbackInfo.Index, DLR.Expression.Constant(Utils.ANull())),
                    DLR.Expression.Assign(scope.CallbackInfo.Path, DLR.Expression.Constant(Utils.ANull())),
                    DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true, typeof(bool))),
                    DLR.Expression.Assign(scope.CallbackInfo.NonPresetValue, DLR.Expression.Constant(null, typeof(AType))),
                    DLR.Expression.Assign(value, this.expression.Generate(scope)),
                    DLR.Expression.IfThen(
                        DLR.Expression.Equal(scope.CallbackInfo.NonPresetValue, DLR.Expression.Constant(null, typeof(AType))),
                        DLR.Expression.Assign(scope.CallbackInfo.NonPresetValue, value)
                    ),
                    DLR.Expression.Assign(
                            value,
                            DLR.Expression.Condition(
                                DLR.Expression.Property(value, "IsMemoryMappedFile"),
                                value,
                                DLR.Expression.Call(
                                    value,
                                    typeof(AType).GetMethod("Clone")
                                )
                            )
                        ),
                    result,
                    scope.CallbackInfo.NonPresetValue
                );

            scope.CallbackInfo = savedCallbackInfos;
            scope.AssignDone = prevAssignDone;

            return blockedResult;
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

            scope.IsAssignment = true;
            DLR.Expression right = function.Right.Generate(scope);
            scope.IsAssignment = false;

            var method = DyadicFunctionInstance.Pick;
            DLR.Expression pickDLR = DLR.Expression.Call(
                DLR.Expression.Constant(method),
                method.GetType().GetMethod("AssignExecute"),
                right,
                left,
                scope.GetRuntimeExpression()
            );

            DLR.ParameterExpression errorParam = DLR.Expression.Parameter(typeof(Error.Signal));

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;

            DLR.ParameterExpression temp = DLR.Expression.Parameter(typeof(AType));
            DLR.Expression callback = BuildCallbackCall(scope, temp);
            DLR.Expression presetCallback = BuildPresetCallbackCall(scope, temp);

            DLR.Expression result =
                DLR.Expression.Block(
                    new DLR.ParameterExpression[] { temp },
                    DLR.Expression.Assign(scope.CallbackInfo.Path, left),
                    DLR.Expression.Assign(temp, value),
                    DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(false)),
                    DLR.Expression.TryCatch(
                        DLR.Expression.Block(
                            typeof(void),
                            DLR.Expression.Assign(temp, presetCallback),
                            DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true))
                        ),
                        DLR.Expression.Catch(
                            errorParam,
                            DLR.Expression.Empty()
                        )
                    ),
                    DLR.Expression.IfThen(
                        scope.AssignDone,
                        DLR.Expression.Block(
                            DLR.Expression.Call(
                                typeof(Assign).GetMethod("PickAssignHelper", flags),
                                temp,
                                pickDLR
                            ),
                            callback
                        )
                    ),
                    scope.CallbackInfo.NonPresetValue
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

        private static AType AppendItem(AType value, AType target, Aplus environment)
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
                for (int i = 0; i < value.Length; i++)
                {
                    target.Add(value[i].Clone());
                }
            }
            else
            {
                throw new Error.Length("assign");
            }

            return value;
        }

        private static AType CalculateIndexes(AType value, AType target, Aplus environment, out AType indexes)
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

            int currentLength = target.Length;

            if (target.Shape.SequenceEqual<int>(value.Shape))
            {
                indexes = AArray.Create(ATypes.AInteger);

                for (int i = 0; i < value.Length; i++)
                {
                    indexes.Add(AInteger.Create(currentLength));
                    currentLength++;
                }
            }
            else if (target.Shape.GetRange(1, target.Shape.Count - 1).SequenceEqual<int>(value.Shape))
            {
                indexes = AInteger.Create(currentLength);
                currentLength++;
            }
            else if (value.Rank == 0)
            {
                indexes = AInteger.Create(currentLength);
                currentLength++;
            }
            else if (value.Rank == target.Rank)
            {
                indexes = AArray.Create(ATypes.AInteger);

                for (int i = 0; i < value.Length; i++)
                {
                    indexes.Add(AInteger.Create(currentLength));
                    currentLength++;
                }
            }
            else
            {
                throw new Error.Length("assign");
            }

            indexes = ABox.Create(indexes);

            return value;
        }

        #endregion

        #region DLR: Assing to indexed target

        private static DLR.Expression GenerateIndexAssign(AplusScope scope, Indexing target, DLR.Expression value)
        {
            DLR.Expression result;

            if (target.IndexExpression == null)
            {
                result = DLR.Expression.Dynamic(
                    scope.GetRuntime().SetIndexBinder(new DYN.CallInfo(0)),
                    typeof(object),
                    target.Item.Generate(scope),
                    DLR.Expression.Constant(null),
                    value
                );
            }
            else
            {
                IEnumerable<DLR.Expression> indexerValues = target.IndexExpression.Items.Reverse().Select(
                    item => { return item.Generate(scope); }
                );

                DLR.ParameterExpression indexerParam = DLR.Expression.Parameter(typeof(List<AType>), "__INDEX__");

                DLR.Expression call =
                    DLR.Expression.Call(
                        typeof(Helpers).GetMethod("BuildIndexerArray"),
                        DLR.Expression.NewArrayInit(typeof(AType), indexerValues)
                    );

                Aplus runtime = scope.GetRuntime();
                string qualifiedName = ((Identifier)target.Item).BuildQualifiedName(runtime.CurrentContext);
                DLR.ParameterExpression temp = DLR.Expression.Parameter(typeof(AType));
                DLR.ParameterExpression errorParam = DLR.Expression.Parameter(typeof(Error.Signal));
                DLR.Expression callback = BuildCallbackCall(scope, temp);
                DLR.Expression presetCallback = BuildPresetCallbackCall(scope, temp);

                result =
                    DLR.Expression.Block(
                        new DLR.ParameterExpression[] { indexerParam, temp },
                        DLR.Expression.Assign(
                            indexerParam,
                            call
                        ),
                        DLR.Expression.Assign(
                            scope.CallbackInfo.Index,
                            DLR.Expression.Call(
                                typeof(Tools).GetMethod("ConvertATypeListToAType", BindingFlags.NonPublic | BindingFlags.Static),
                                indexerParam
                            )
                        ),
                        DLR.Expression.Assign(
                                scope.CallbackInfo.QualifiedName,
                                DLR.Expression.Constant(qualifiedName)
                        ),
                        DLR.Expression.Assign(temp, value),
                        DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(false)),
                        DLR.Expression.TryCatch(
                            DLR.Expression.Block(
                                typeof(void),
                                DLR.Expression.Assign(value, presetCallback),
                                DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true))
                            ),
                            DLR.Expression.Catch(
                                errorParam,
                                DLR.Expression.Empty()
                            )
                        ),
                        DLR.Expression.IfThen(
                            scope.AssignDone,
                            DLR.Expression.Block(
                                DLR.Expression.Dynamic(
                                    scope.GetRuntime().SetIndexBinder(new System.Dynamic.CallInfo(target.IndexExpression.Length)),
                                    typeof(object),
                                    target.Item.Generate(scope),
                                    indexerParam,
                                    value
                                ),
                                DLR.Expression.Assign(temp, value),
                                callback
                            )
                        ),
                        scope.CallbackInfo.NonPresetValue
                    );

            }
            return DLR.Expression.Convert(result, typeof(AType));
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
        ///     // need to disclose the box
        ///     if($__VALUES__.Type == ATypes.Box)
        ///     {
        ///         $__VALUES__ = disclose($__VALUES__);
        ///     }
        ///     a = $VALUES.Clone();
        ///     b = $VALUES.Clone();
        ///     ...
        /// }
        /// $__DependencyManager__.InvalidateDependencies(string[] { a,b, .. })
        /// $__DependencyManager__.ValidateDependencies(string[] { a,b, .. })
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
            DLR.ParameterExpression environment = scope.GetRuntimeExpression();

            DLR.ParameterExpression valuesParam = DLR.Expression.Parameter(typeof(AType), "__VALUES__");
            // for dependency evaluation
            List<string> targetVariables = new List<string>();

            // for case A) assigns
            List<DLR.Expression> strand2StrandAssigns = new List<DLR.Expression>();
            // for case B) assigns
            List<DLR.Expression> strand2ValueAssigns = new List<DLR.Expression>()
            {
                DLR.Expression.Assign(
                    valuesParam,
                    DLR.Expression.Call(
                        DLR.Expression.Constant(disclose),
                        disclose.GetType().GetMethod("Execute"),
                        valuesParam,
                        environment
                    )
                )
            };

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
                strand2StrandAssigns.Add(GenerateIdentifierAssign(scope, id, strandValue, isStrand: true));

                // case B) $TARGETS[i] = $VALUE.Clone()
                strand2ValueAssigns.Add(
                    GenerateIdentifierAssign(
                        scope,
                        id,
                        DLR.Expression.Condition(
                            DLR.Expression.Property(valuesParam, "IsMemoryMappedFile"),
                            valuesParam,
                            DLR.Expression.Call(
                                valuesParam,
                                typeof(AType).GetMethod("Clone")
                            )
                        ),
                        isStrand: true
                    )
                );

                // gather the global variables for dependency validation/invalidation. 
                if ((scope.IsMethod && id.IsEnclosed) || !scope.IsMethod)
                {
                    targetVariables.Add(id.BuildQualifiedName(runtime.CurrentContext));
                }

                indexCounter++;
            }

            // case A)
            DLR.Expression caseStrand2Strand =
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
                    DLR.Expression.Block(strand2StrandAssigns)
            );

            DLR.Expression dependencyManager = DLR.Expression.Property(scope.GetRuntimeExpression(), "DependencyManager");

            DLR.Expression result = DLR.Expression.Block(
                new DLR.ParameterExpression[] { valuesParam },
                DLR.Expression.Assign(valuesParam, DLR.Expression.Convert(value, typeof(AType))),
                DLR.Expression.IfThenElse(
                    DLR.Expression.PropertyOrField(valuesParam, "IsArray"),
                // case A)
                    caseStrand2Strand,
                // case B)
                    DLR.Expression.Block(strand2ValueAssigns)
                ),
                DLR.Expression.Call(
                    dependencyManager,
                    typeof(DependencyManager).GetMethod("InvalidateDependencies", new Type[] { typeof(string[]) }),
                    DLR.Expression.Constant(targetVariables.ToArray())
                ),
                DLR.Expression.Call(
                    dependencyManager,
                    typeof(DependencyManager).GetMethod("ValidateDependencies"),
                    DLR.Expression.Constant(targetVariables.ToArray())
                ),
                valuesParam
            );

            return DLR.Expression.Block(result, DLR.Expression.Assign(scope.CallbackInfo.NonPresetValue, value));
        }

        #endregion

        #region DLR: Assign to Identifier

        internal static DLR.Expression GenerateIdentifierAssign(
            AplusScope scope, Identifier target, DLR.Expression value, bool isStrand = false, bool needCallback = true)
        {
            Aplus runtime = scope.GetRuntime();
            DLR.Expression result = null;

            if (scope.IsMethod)
            {
                string qualifiedName = target.BuildQualifiedName(runtime.CurrentContext);

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
                                DLR.Expression.Block(
                                    needCallback
                                    ? DLR.Expression.Assign(scope.CallbackInfo.QualifiedName, DLR.Expression.Constant(qualifiedName))
                                    : (DLR.Expression)DLR.Expression.Empty(),
                                    DLR.Expression.Dynamic(
                                        runtime.SetMemberBinder(target.Name),
                                        typeof(object),
                                        functionScopeParam,
                                        value
                                    ),
                                    value
                                ),
                                // did NOT found the variable in the function scope
                                // perform the assignment in the global scope
                                BuildGlobalAssignment(scope, runtime, globalScopeParam, target, value, isStrand, needCallback)
                            );

                        }
                        else if (target.IsEnclosed)
                        {
                            result = BuildGlobalAssignment(scope, runtime, globalScopeParam, target, value, isStrand, needCallback);
                        }
                        else
                        {
                            // Simple case, we are inside a user defined function, but not inside an eval block
                            result =
                                DLR.Expression.Block(
                                    needCallback
                                    ? DLR.Expression.Assign(scope.CallbackInfo.QualifiedName, DLR.Expression.Constant(qualifiedName))
                                    : (DLR.Expression)DLR.Expression.Empty(),
                                    DLR.Expression.Dynamic(
                                    runtime.SetMemberBinder(target.Name),
                                    typeof(object),
                                    functionScopeParam,
                                    value
                                    )
                                );
                        }
                        break;

                    case IdentifierType.QualifiedName:
                    case IdentifierType.SystemName:
                    default:
                        // Do an assignment on the global scope
                        result = BuildGlobalAssignment(scope, runtime, globalScopeParam, target, value, isStrand, needCallback);
                        break;
                }
            }
            else
            {
                result = BuildGlobalAssignment(scope, runtime, scope.GetModuleExpression(), target, value, isStrand, needCallback);
            }


            return DLR.Expression.Convert(result, typeof(AType));
        }


        private static DLR.Expression BuildGlobalAssignment(
            AplusScope scope,
            Aplus runtime,
            DLR.Expression variableContainer,
            Identifier target,
            DLR.Expression value,
            bool isStrand = false,
            bool needCallback = true
            )
        {
            // Build the ET for updating the dependency
            DLR.Expression dependencyCall;
            string qualifiedName = target.BuildQualifiedName(runtime.CurrentContext);

            if (isStrand)
            {
                // We are inside a starnd assignment, do nothing.
                // Dependency update will be handled later.
                dependencyCall = DLR.Expression.Empty();
            }
            else
            {
                // Otherwise build the dependency invalidation call.
                DLR.Expression dependencyManager = DLR.Expression.Property(scope.GetRuntimeExpression(), "DependencyManager");
                dependencyCall = DLR.Expression.Call(
                    dependencyManager,
                    typeof(DependencyManager).GetMethod("InvalidateDependencies", new Type[] { typeof(string) }),
                    DLR.Expression.Constant(qualifiedName)
                );
            }

            DLR.ParameterExpression errorParam = DLR.Expression.Parameter(typeof(Error.Signal), "__ERRORPARAM__");
            DLR.ParameterExpression valueParam = DLR.Expression.Parameter(typeof(AType), "__VALUEPARAMETER__");
            scope.AssignDone = (scope.AssignDone == null) ? DLR.Expression.Parameter(typeof(bool), "__ASSIGNDONE__") : scope.AssignDone;

            DLR.Expression presetCallback = BuildPresetCallbackCall(scope, valueParam);
            DLR.Expression callback = BuildCallbackCall(scope, valueParam);

            DLR.Expression variableSet =
                DLR.Expression.Block(
                        VariableHelper.SetVariable(
                            runtime,
                            variableContainer,
                            target.CreateContextNames(runtime.CurrentContext),
                            valueParam
                        ),
                        dependencyCall
                    );

            DLR.Expression codebody;

            if (needCallback)
            {
                codebody =
                    DLR.Expression.Block(
                        typeof(void),
                        DLR.Expression.Assign(scope.CallbackInfo.QualifiedName, DLR.Expression.Constant(qualifiedName)),
                        DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(false)),
                        DLR.Expression.TryCatch(
                            DLR.Expression.Block(
                                typeof(void),
                                DLR.Expression.Assign(valueParam, presetCallback),
                                DLR.Expression.Assign(scope.AssignDone, DLR.Expression.Constant(true))
                            ),
                            DLR.Expression.Catch(
                                errorParam,
                                DLR.Expression.Empty()
                            )
                        ),
                        DLR.Expression.IfThen(
                            scope.AssignDone,
                            DLR.Expression.Block(
                                variableSet,
                                callback
                            )
                        )
                    );
            }
            else
            {
                codebody = variableSet;
            }

            // value assignment
            DLR.Expression result = DLR.Expression.Block(
                new DLR.ParameterExpression[] { valueParam },
                DLR.Expression.Assign(valueParam, value),
                codebody,
                valueParam
            );

            return result;
        }

        internal static DLR.Expression BuildPresetCallbackCall(AplusScope scope, DLR.ParameterExpression valueParam)
        {
            DLR.ParameterExpression callbackParameter = DLR.Expression.Parameter(typeof(CallbackItem), "__CALLBACK__");
            DLR.Expression callback = DLR.Expression.Block(
                new DLR.ParameterExpression[] { callbackParameter },
                DLR.Expression.Condition(
                    DLR.Expression.Call(
                        scope.GetRuntimeExpression().Property("CallbackManager"),
                        typeof(CallbackManager).GetMethod("TryGetPresetCallback"),
                        scope.CallbackInfo.QualifiedName,
                        callbackParameter
                    ),
                    DLR.Expression.Dynamic(
                // TODO: do not instantiate the binder here
                        new Runtime.Binder.CallbackBinder(),
                        typeof(object),
                        callbackParameter,
                        scope.GetRuntimeExpression(),
                        valueParam,
                        scope.CallbackInfo.Index,
                        scope.CallbackInfo.Path
                    ).To<AType>(),
                    (DLR.Expression)valueParam.To<AType>()
                )
            );
            return callback;
        }

        internal static DLR.Expression BuildCallbackCall(AplusScope scope, DLR.ParameterExpression valueParam)
        {
            // callback
            /* 
             * CallbackItem callback;
             * if(CallbackManager.TryGetCallback(globalName, out callback)
             * {
             *     CallbackBinder.Invoke(...);
             * }
             */
            DLR.ParameterExpression callbackParameter = DLR.Expression.Parameter(typeof(CallbackItem), "__CALLBACK__");
            DLR.Expression callback = DLR.Expression.Block(
                new DLR.ParameterExpression[] { callbackParameter },
                DLR.Expression.IfThen(
                    DLR.Expression.Call(
                        scope.GetRuntimeExpression().Property("CallbackManager"),
                        typeof(CallbackManager).GetMethod("TryGetCallback"),
                        scope.CallbackInfo.QualifiedName,
                        callbackParameter
                    ),
                    DLR.Expression.Dynamic(
                // TODO: do not instantiate the binder here
                        new Runtime.Binder.CallbackBinder(),
                        typeof(object),
                        callbackParameter,
                        scope.GetRuntimeExpression(),
                        valueParam,
                        scope.CallbackInfo.Index,
                        scope.CallbackInfo.Path
                    )
                )
            );
            return callback;
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
            DLR.ParameterExpression environment = scope.GetRuntimeExpression();
            var execute = typeof(AbstractMonadicFunction).GetMethod("Execute");

            // (iota rho x)
            DLR.Expression indexes = DLR.Expression.Call(
                DLR.Expression.Constant(MonadicFunctionInstance.Interval),
                execute,
                DLR.Expression.Call(
                    DLR.Expression.Constant(MonadicFunctionInstance.Shape),
                    execute,
                    argument,
                    environment
                ),
                environment
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
            DLR.ParameterExpression indexes = DLR.Expression.Parameter(typeof(List<AType>), "_INDEX_");

            // (,x)
            DLR.Expression raveledRight = DLR.Expression.Call(
                DLR.Expression.Constant(MonadicFunctionInstance.Ravel),
                execute,
                target,
                scope.GetRuntimeExpression()
            );

            // (,x)[indexExpression] := ..
            DLR.Expression result =
                DLR.Expression.Convert(
                    DLR.Expression.Block(
                        new DLR.ParameterExpression[] { indexes },
                        DLR.Expression.Assign(
                            indexes,
                            DLR.Expression.Call(
                                typeof(Helpers).GetMethod("BuildIndexerArray"),
                                DLR.Expression.NewArrayInit(typeof(AType), indexExpression)
                            )
                        ),
                        DLR.Expression.Assign(
                            scope.CallbackInfo.Index,
                            DLR.Expression.Call(
                                typeof(Tools).GetMethod("ConvertATypeListToAType", BindingFlags.NonPublic | BindingFlags.Static),
                                indexes
                            )
                        ),
                        DLR.Expression.Dynamic(
                            scope.GetRuntime().GetIndexBinder(new DYN.CallInfo(1)),
                            typeof(object),
                            raveledRight,
                            indexes
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
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Build a AST node representing an assignment.
        /// </summary>
        /// <param name="target">The target of the assignment.</param>
        /// <param name="expression">The value of the assignment.</param>
        /// <returns><see cref="Assign">Assignment</see> AST node.</returns>
        public static Assign Assign(Node target, Node expression)
        {
            return new Assign(target, expression);
        }
    }

    #endregion
}
