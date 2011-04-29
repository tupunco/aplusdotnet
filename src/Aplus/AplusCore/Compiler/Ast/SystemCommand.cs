using System;
using DLR = System.Linq.Expressions;
using AplusCore.Runtime;
using AplusCore.Types;
using System.Collections.Generic;
using System.IO;

namespace AplusCore.Compiler.AST
{
    public class SystemCommand : Node
    {
        #region Variables

        private string command;
        private string argument;

        #endregion

        #region Properties

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
                    if (!File.Exists(this.argument))
                    {
                        break;
                    }
                    codeBlock.Clear();

                    // Create the AST from file 
                    Node fileAST = Parse.LoadFile(this.argument, runtime.LexerMode);
                    // And generate the DLR tree
                    codeBlock.AddFirst(
                        fileAST.Generate(scope)
                    );

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

        #region GraphViz output (Only in DEBUG)
#if DEBUG
        private static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder textBuilder)
        {
            string name = String.Format("SystemCommand{0}", counter++);
            textBuilder.AppendFormat("  {0}_cmd [label=\"{1}\"];\n", name, this.command);
            textBuilder.AppendFormat("  {0}_arg [label=\"{1}\"];\n", name, this.argument);
            textBuilder.AppendFormat("  {0} -> {0}_cmd;\n", name);
            textBuilder.AppendFormat("  {0} -> {0}_arg;\n", name);

            return name;
        }
#endif
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
