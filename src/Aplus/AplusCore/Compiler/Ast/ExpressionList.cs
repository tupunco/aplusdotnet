using System;
using System.Linq;
using System.Collections.Generic;

using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    public class ExpressionList : Node
    {
        #region Variables

        private LinkedList<Node> nodeList;

        #endregion

        #region Properties

        public int Length { get { return this.nodeList.Count; } }
        public LinkedList<Node> Items { get { return this.nodeList; } }
        public Node this[int index] { get { return this.nodeList.ElementAt<Node>(index); } }

        #endregion

        #region Constructor

        public ExpressionList()
        {
            this.nodeList = new LinkedList<Node>();
        }

        #endregion

        #region Methods

        public void AddFirst(Node item)
        {
            this.nodeList.AddFirst(item);
        }

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
            //string tmp = String.Join(" ", this.nodeList.ToList().ConvertAll<string>(item => item.ToString()).ToArray());
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
        /// Construct an Expression List AST Node
        /// </summary>
        /// <returns>Expression List AST Node</returns>
        public static ExpressionList ExpressionList()
        {
            ExpressionList nodeList = new ExpressionList();
            return nodeList;
        }

        /// <summary>
        /// Construct an Expression List AST Node wiht an initial set of node list
        /// </summary>
        /// <remarks>
        /// The order of input nodes are preserved, thus each node is added to the end of
        /// the Expression List.
        /// </remarks>
        /// <param name="nodes">List of nodes to add to Expression List</param>
        /// <returns>Expression List AST Node</returns>
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
