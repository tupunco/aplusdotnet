using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    //Order: right to left.
    public class Strand : Node
    {
        #region Variables

        private LinkedList<Node> arguments;

        #endregion

        #region Constructor

        public Strand()
        {
            this.arguments = new LinkedList<Node>();
        }

        #endregion

        #region Properties

        internal LinkedList<Node> Items
        {
            get { return this.arguments; }
        }

        #endregion

        #region Methods

        public void AddLast(Node item)
        {
            this.arguments.AddLast(item);
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            LinkedList<DLR.Expression> strandArguments = new LinkedList<DLR.Expression>();

            // Add the paramters for the strand in a reverse order
            // thus enforcing the right-to-left execution order
            foreach (Node item in arguments)
            {
                strandArguments.AddFirst(item.Generate(scope));
            }
            
            // Call the Runtime.Helpers.BuildStrand method
            //  with the reversed paramaters wrappend in an AType[] array
            DLR.Expression result = DLR.Expression.Call(
                typeof(Runtime.Helpers).GetMethod("BuildStrand"),
                DLR.Expression.NewArrayInit(
                    typeof(AType),
                    strandArguments
                )
            );

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            string tmp = String.Join(" ", this.arguments.ToList().ConvertAll<string>(item => item.ToString()).ToArray());
            return String.Format("Strand({0})", tmp);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Strand))
            {
                return false;
            }
            Strand other = (Strand)obj;

            if (this.arguments.Count != other.arguments.Count)
            {
                return false;
            }
            List<Node> myList = this.arguments.ToList<Node>();
            List<Node> otherList = other.arguments.ToList<Node>();

            for (int i = 0; i < myList.Count; i++)
            {
                if (!myList[i].Equals(otherList[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = this.arguments.GetHashCode();
            foreach (Node item in this.arguments)
            {
                hash ^= item.GetHashCode();
            }
            return hash;
        }

        #endregion

        #region GraphViz output (Only in DEBUG)

#if DEBUG
        private static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder textBuilder)
        {
            string name = String.Format("Strand{0}", counter++);
            foreach (Node argument in this.arguments)
            {
                string itemName = argument.ToDot(name, textBuilder);
                textBuilder.AppendFormat("  {0} -> {1};\n", name, itemName);
            }
            return name;
        }
#endif

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static Strand Strand()
        {
            Strand strand = new Strand();
            return strand;
        }
        public static Strand Strand(params Node[] nodes)
        {
            Strand strand = new Strand();
            foreach (Node node in nodes)
            {
                strand.AddLast(node);
            }
            return strand;
        }
        public static Strand Strand(IEnumerable<Node> nodes)
        {
            Strand strand = new Strand();
            foreach (Node node in nodes)
            {
                strand.AddLast(node);
            }
            return strand;
        }
    }

    #endregion
}
