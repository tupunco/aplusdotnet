using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a strand construction in an A+ AST.
    /// </summary>
    /// <remarks>
    /// The order of the <see cref="Node"/>s in the strand is right to left.
    /// </remarks>
    public class Strand : Node
    {
        #region Variables

        private LinkedList<Node> arguments;

        #endregion

        #region Properties

        public LinkedList<Node> Items
        {
            get { return this.arguments; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Strand"/> AST node.
        /// </summary>
        public Strand()
        {
            this.arguments = new LinkedList<Node>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adss the <see cref="Node"/> to the end of the strand.
        /// </summary>
        /// <param name="item">The <see cref="Node"/> to add.</param>
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
            return String.Format("Strand({0})", String.Join(" ", this.arguments.ToStringArray()));
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
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds a <see cref="Node"/> representing a <see cref="Strand"/> construction.
        /// </summary>
        /// <param name="nodes">The <see cref="Node"/>s to add to the <see cref="Strand"/>.</param>
        /// <returns>Returns a <see cref="Strand"/> containing the given <see cref="Node"/>s.</returns>
        public static Strand Strand(params Node[] nodes)
        {
            return Strand(nodes as IEnumerable<Node>);
        }

        /// <summary>
        /// Builds a <see cref="Node"/> representing a <see cref="Strand"/> construction.
        /// </summary>
        /// <param name="nodes">The <see cref="Node"/>s to add to the <see cref="Strand"/>.</param>
        /// <returns>Returns a <see cref="Strand"/> containing the given <see cref="Node"/>s.</returns>
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
