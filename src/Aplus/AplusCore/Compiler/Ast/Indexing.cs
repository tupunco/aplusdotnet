using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents an indexing expression in an A+ AST.
    /// </summary>
    public class Indexing : Node
    {
        #region Variables

        private Node item;
        private ExpressionList indexExpression;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the target <see cref="Node"/> of the indexing.
        /// </summary>
        public Node Item
        {
            get { return this.item; } 
        }

        /// <summary>
        /// Gets the indexing expressions.
        /// </summary>
        public ExpressionList IndexExpression
        {
            get { return this.indexExpression; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="Indexing"/> AST node.
        /// </summary>
        /// <param name="item">The target of the indexing.</param>
        /// <param name="indexExpression">The indexer expressions.</param>
        public Indexing(Node item, ExpressionList indexExpression)
        {
            this.item = item;
            this.indexExpression = indexExpression;
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            DLR.Expression result;

            if (this.indexExpression != null)
            {
                // Generate each indexer expression in reverse order
                // the reverse order is to mimic the A+ evaulation order
                IEnumerable<DLR.Expression> indexerValues = this.indexExpression.Items.Reverse().Select(
                    item => { return item.Generate(scope); }
                );

                DLR.ParameterExpression indexerParam = DLR.Expression.Parameter(typeof(List<AType>), "__INDEX__");

                result = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { indexerParam },
                    DLR.Expression.Assign(
                        indexerParam,
                        DLR.Expression.Call(
                            typeof(Helpers).GetMethod("BuildIndexerArray"),
                            DLR.Expression.NewArrayInit(typeof(AType), indexerValues)
                        )
                    ),
                    DLR.Expression.Dynamic(
                        scope.GetRuntime().GetIndexBinder(new DYN.CallInfo(indexerValues.Count())),
                        typeof(object),
                        this.item.Generate(scope),
                        indexerParam
                    )
                );
            }
            else
            {
                // in case of: a[];
                result =
                    DLR.Expression.Dynamic(
                        scope.GetRuntime().GetIndexBinder(new DYN.CallInfo(0)),
                        typeof(object),
                        this.item.Generate(scope),
                        DLR.Expression.Constant(null)
                    );
            }

            return result.To<AType>();
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Indexing({0} {1})", this.item, this.indexExpression);
        }

        public override bool Equals(object obj)
        {
            if (obj is Indexing)
            {
                Indexing other = (Indexing)obj;
                return this.item.Equals(other.item) && this.indexExpression.Equals(other.indexExpression);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.item.GetHashCode() ^ this.indexExpression.GetHashCode();
        }

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        /// <summary>
        /// Builds a <see cref="Node"/> representing an indexing expression.
        /// </summary>
        /// <param name="item">The target of the indexing.</param>
        /// <param name="indexExpression">The indexer expressions.</param>
        /// <returns>Returns a <see cref="Indexing"/> AST node.</returns>
        public static Indexing Indexing(Node item, ExpressionList indexExpression)
        {
            return new Indexing(item, indexExpression);
        }
    }

    #endregion
}
