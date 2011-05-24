using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;
using DYN = System.Dynamic;

namespace AplusCore.Compiler.AST
{
    public class Indexing : Node
    {
        #region Variables

        private Node item;
        private ExpressionList indexExpression;

        #endregion

        #region Properties

        internal Node Item { get { return this.item; } }
        internal ExpressionList IndexExpression { get { return this.indexExpression; } }

        #endregion

        #region Constructor

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

                List<DLR.Expression> arguments = new List<DLR.Expression>();
                arguments.Add(this.item.Generate(scope));
                arguments.Add(indexerParam);

                result = DLR.Expression.Block(
                    new DLR.ParameterExpression[] { indexerParam },
                    DLR.Expression.Assign(
                        indexerParam,
                        DLR.Expression.Call(
                            typeof(Helpers).GetMethod("BuildIndexerArray"),
                            DLR.Expression.NewArrayInit(typeof(AType), indexerValues)
                        )
                    ),
                    DLR.Expression.Convert(
                        DLR.Expression.Dynamic(
                            scope.GetRuntime().GetIndexBinder(new DYN.CallInfo(indexerValues.Count())),
                            typeof(object),
                            arguments
                        ),
                    typeof(AType)
                    )
                );
            }
            else
            {
                // in case of: a[];
                result = DLR.Expression.Convert(
                    DLR.Expression.Dynamic(
                        scope.GetRuntime().GetIndexBinder(new DYN.CallInfo(0)),
                        typeof(object),
                        this.item.Generate(scope),
                        DLR.Expression.Constant(null)
                    ),
                    typeof(AType)
                );
                    
            }

            return result;
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

        #region GraphViz output (Only in DEBUG)

#if DEBUG
        private static int counter = 0;
        internal override string ToDot(string parent, System.Text.StringBuilder textBuilder)
        {
            string name = String.Format("Idexing{0}", counter++);
            string itemName = this.item.ToDot(name, textBuilder);
            string indexExprName = this.indexExpression.ToDot(name, textBuilder);

            textBuilder.AppendFormat("  {0} [label=\"Indexing\"];", name);
            textBuilder.AppendFormat("  {0} -> {1};\n", name, itemName);
            textBuilder.AppendFormat("  {0} -> {1};\n", name, indexExprName);

            return name;
        }
#endif

        #endregion
    }

    #region Construction helper

    public partial class Node
    {
        public static Indexing Indexing(Node item, ExpressionList indexExpression)
        {
            return new Indexing(item, indexExpression);
        }
    }

    #endregion
}
