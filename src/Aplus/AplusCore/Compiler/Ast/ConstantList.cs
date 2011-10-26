using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Runtime;
using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler.AST
{
    /// <summary>
    /// Represents a series of <see cref="Constant"/>s as a Constant List in an A+ AST.
    /// </summary>
    public class ConstantList : Node, IEnumerable<Constant>
    {
        #region Variables

        private LinkedList<Constant> list;

        /// <summary>
        /// The type of the ConstantList
        /// </summary>
        /// <remarks>
        /// If 'Integer' constants stored, then a 'Double' constant
        /// added then the ConstantList's type will be 'Double'.
        /// </remarks>
        private ConstantType type;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="ConstantList"/> AST node.
        /// </summary>
        public ConstantList()
        {
            this.list = new LinkedList<Constant>();
            this.type = ConstantType.Undefined;
        }

        #endregion

        #region Methods

        private void UpdateType(ConstantType type)
        {
            if (type > this.type)
            {
                this.type = type;
            }
        }

        /// <summary>
        /// Adds the given <see cref="Constant"/> to the <see cref="ConstantList"/>.
        /// </summary>
        /// <param name="item">The <see cref="Constant"/> to add.</param>
        public void AddLast(Constant item)
        {
            UpdateType(item.Type);
            this.list.AddLast(item);
        }

        #endregion

        #region DLR Generator

        public override DLR.Expression Generate(AplusScope scope)
        {
            if (this.list.Count == 1)
            {
                return this.list.First.Value.Generate(scope);
            }

            AType items = null;

            if (this.type == ConstantType.Integer)
            {
                bool convertToFloat = false;
                items = AArray.Create(ATypes.AInteger);
                foreach (Constant item in this.list)
                {
                    AType value = item.AsNumericAType;
                    items.Add(value);

                    if (value.Type == ATypes.AFloat)
                    {
                        convertToFloat = true;
                    }
                }

                // Convert integer items in previous array to float
                if (convertToFloat)
                {
                    items.ConvertToFloat();
                }
            }
            // Treat the Infinite constants same as floats
            else if (this.type == ConstantType.Double ||
                    this.type == ConstantType.PositiveInfinity ||
                    this.type == ConstantType.NegativeInfinity)
            {
                items = AArray.Create(ATypes.AFloat);
                foreach (Constant item in this.list)
                {
                    items.Add(AFloat.Create(item.AsFloat));
                }
            }
            else if (this.type == ConstantType.Symbol)
            {
                items = AArray.Create(ATypes.ASymbol);
                foreach (Constant item in this.list)
                {
                    items.Add(ASymbol.Create(item.AsString));
                }
            }
            else
            {
                throw new Error.Parse(String.Format("Unknow ConstantType({0}) in current context", this.type));
            }

            DLR.Expression result = DLR.Expression.Constant(items, typeof(AType));
            // Example: .Constant<AplusCore.Types.AArray`1[AplusCore.Types.AInteger]>(1 2)
            return result;

        }

        #endregion

        #region IEnumerable<Constant> Members

        public IEnumerator<Constant> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this.list).GetEnumerator();
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            string tmp = String.Join(" ", this.list.ToStringArray<Constant>());
            return String.Format("ConstantList({0})", tmp);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ConstantList))
            {
                return false;
            }
            ConstantList other = (ConstantList)obj;

            if (this.list.Count != other.list.Count)
            {
                return false;
            }
            List<Constant> myList = this.list.ToList<Constant>();
            List<Constant> otherList = other.list.ToList<Constant>();

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
            int hash = this.list.GetHashCode();
            foreach (Constant item in this.list)
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
        /// Build a <see cref="AST.ConstantList"/> from the given <see cref="AST.Constant"/>s.
        /// </summary>
        /// <param name="constants">Series of <see cref="AST.Constant"/>s.</param>
        /// <returns>Returns a <see cref="AST.ConstantList"/> containing the given <see cref="AST.Constant"/>s.</returns>
        public static ConstantList ConstantList(params Constant[] constants)
        {
            ConstantList list = new ConstantList();
            foreach (Constant item in constants)
            {
                list.AddLast(item);
            }

            return list;
        }
    }

    #endregion
}
