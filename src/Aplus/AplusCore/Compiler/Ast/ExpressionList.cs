using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a series of <see cref="Node"/>s in an A+ AST.
    /// </summary>
    public class ExpressionList : Node
    {
        #region Variables

        private LinkedList<Node> nodeList;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of <see cref="Node"/>s in the <see cref="ExpressionList"/>.
        /// </summary>
        public int Length
        {
            get { return this.nodeList.Count; }
        }

        /// <summary>
        /// Gets the list of <see cref="Node"/>s in the <see cref="ExpressionList"/>.
        /// </summary>
        public LinkedList<Node> Items
        {
            get { return this.nodeList; }
        }

        public Node this[int index]
        {
            get { return this.nodeList.ElementAt<Node>(index); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="ExpressionList"/> AST node.
        /// </summary>
        public ExpressionList()
        {
            this.nodeList = new LinkedList<Node>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add the <see cref="Node"/> as the first item to the <see cref="ExpressionList"/>.
        /// </summary>
        /// <param name="item">The <see cref="Node"/> to add.</param>
        public void AddFirst(Node item)
        {
            this.nodeList.AddFirst(item);
        }

        /// <summary>
        /// Add the <see cref="Node"/> as the last item to the <see cref="ExpressionList"/>.
        /// </summary>
        /// <param name="item">The <see cref="Node"/> to add.</param>
        public void AddLast(Node item)
        {
            this.nodeList.AddLast(item);
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;

            if (this.nodeList.Count > 0)
            {
                result = DLR.Expression.Block(this.nodeList.Select(node => node.Generate(scope)));
            }
            else
            {
                result = DLR.Expression.Constant(Utils.ANull());
            }

            return result;
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            string tmp = String.Join(" ", this.nodeList.ToStringArray<Node>());
            return String.Format("ExpressionList({0})", tmp);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExpressionList))
            {
                return false;
            }
            ExpressionList other = (ExpressionList)obj;

            if (this.nodeList.Count != other.nodeList.Count)
            {
                return false;
            }
            List<Node> myList = this.nodeList.ToList<Node>();
            List<Node> otherList = other.nodeList.ToList<Node>();

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
            int hash = this.nodeList.GetHashCode();
            foreach (Node item in this.nodeList)
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
        /// Build an <see cref="ExpressionList"/> AST Node with an initial set of <see cref="Node"/>s
        /// </summary>
        /// <remarks>
        /// The order of input nodes are preserved, thus each node is added to the end of
        /// the Expression List.
        /// </remarks>
        /// <param name="nodes">Series of <see cref="Node"/>s.</param>
        /// <returns>Returns an <see cref="ExpersisonList"/> containing the give nodes</returns>
        public static ExpressionList ExpressionList(params Node[] nodes)
        {
            ExpressionList list = new ExpressionList();
            foreach (Node node in nodes)
            {
                list.AddLast(node);
            }
            return list;
        }
    }

    #endregion
}
