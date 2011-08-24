using System;
using System.Collections.Generic;
using System.IO;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class SystemCommand : Node
    {
        #region Variables

        private string command;
        private string argument;

        #endregion

        #region Properties

        public string Command { get { return this.command; } }

        public string Argument
        {
            get { return this.argument; }
            set { this.argument = value; }
        }

        #endregion

        #region Constructor

        public SystemCommand(string command, string argument = null)
        {
            this.command = command;
            this.argument = argument;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            Aplus runtime = scope.GetRuntime();
            // Default result
            LinkedList<DLR.Expression> codeBlock = new LinkedList<DLR.Expression>();
            codeBlock.AddLast(DLR.Expression.Constant(Utils.ANull()));

            switch (this.command)
            {
                case "$off":
                    Environment.Exit(0);
                    break;

                case "$laod": // Compatibility with original A+ interpreter
                case "$load":
                    IDictionary<string, AType> items = 
                        runtime.ContextLoader.FindContextElements(this.argument);

                    if (items.Count > 0)
                    {
                        foreach (KeyValuePair<string, AType> item in items)
                        {
                            codeBlock.AddFirst(
                                VariableHelper.SetVariable(
                                    runtime,
                                    scope.GetModuleExpression(),
                                    new string[] { this.argument, item.Key },
                                    DLR.Expression.Constant(item.Value)
                                )
                            );
                        }
                    }
                    else if (File.Exists(this.argument))
                    {
                        // TODO: Save working directory and restore.
                        // Save the previous context.
                        string previousContext = runtime.CurrentContext;

                        // Create the AST from file 
                        Node fileAST = Parse.LoadFile(this.argument, runtime.LexerMode);
                        // And generate the DLR tree
                        codeBlock.AddFirst(fileAST.Generate(scope));

                        // Restore the previous context
                        runtime.CurrentContext = previousContext;
                    }
                    else
                    {
                        codeBlock.Clear();
                        codeBlock.AddFirst(
                            DLR.Expression.Constant(Helpers.BuildString(String.Format("Invalid file: {0}", this.argument)))
                        );
                    }
                    break;

                case "$mode":
                    if (this.argument != null)
                    {
                        codeBlock.AddFirst(
                            DLR.Expression.Call(
                                scope.RuntimeExpression,
                                typeof(Aplus).GetMethod("SwitchLexerMode"),
                                DLR.Expression.Constant(this.argument)
                             )
                         );
                    }
                    else
                    {
                        codeBlock.Clear();
                        codeBlock.AddFirst(DLR.Expression.Constant(Runtime.Helpers.BuildString(runtime.LexerMode.ToString())));                           
                    }
                    break;

                case "$cx":
                    if (this.argument != null)
                    {
                        runtime.CurrentContext = this.argument;
                    }
                    else
                    {
                        codeBlock.Clear();
                        codeBlock.AddFirst(
                            DLR.Expression.Call(
                                typeof(Runtime.Helpers).GetMethod("BuildString"),
                                DLR.Expression.Property(scope.RuntimeExpression, "CurrentContext")                                
                            )
                        );
                    }
                    break;

                case "$cxs":
                    codeBlock.Clear();
                    codeBlock.AddFirst(
                        DLR.Expression.Call(
                            typeof(Runtime.SystemCommands).GetMethod("PrintContexts"),
                            scope.GetModuleExpression()
                        )
                    );
                    break;
                case "$pp":
                    if (this.argument != null)
                    {
                        runtime.SystemVariables["pp"] = AInteger.Create(int.Parse(this.argument));
                    }
                    else
                    {
                        codeBlock.Clear();
                        codeBlock.AddFirst(DLR.Expression.Constant(runtime.SystemVariables["pp"]));
                    }
                    break;

                case "$stop":
                    if (this.argument != null)
                    {
                        int stopNumber;
                        if (!int.TryParse(this.argument, out stopNumber) || stopNumber > 2)
                        {
                            stopNumber = 0;
                        }

                        runtime.SystemVariables["stop"] = AInteger.Create(stopNumber);
                    }
                    else
                    {
                        string resultText;
                        switch (runtime.SystemVariables["stop"].asInteger)
                        {
                            default:
                            case 0:
                                resultText = "0 off";
                                break;
                            case 1:
                                resultText = "1 on [warning]";
                                break;
                            case 2:
                                resultText = "2 trace";
                                break;
                        }

                        codeBlock.Clear();
                        codeBlock.AddFirst(DLR.Expression.Constant(Helpers.BuildString(resultText)));
                    }
                    break;

                default:
                    Console.WriteLine("Unknown system command: {0} {1}", this.command, this.argument);
                    break;

            }
            
            DLR.Expression result = DLR.Expression.Block(codeBlock);

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("SystemCommand({0} {1})", this.command, this.argument);
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static SystemCommand SystemCommand(string command, string argument = null)
        {
            return new SystemCommand(command, argument);
        }
    }

    #endregion
}
