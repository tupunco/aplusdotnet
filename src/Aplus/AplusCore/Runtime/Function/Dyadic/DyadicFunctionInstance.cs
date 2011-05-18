using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AplusCore.Types;
using AplusCore.Runtime.Function.Dyadic.Scalar;
using AplusCore.Runtime.Function.Dyadic.Scalar.Arithmetic;
using AplusCore.Runtime.Function.Dyadic.Scalar.Logical;
using AplusCore.Runtime.Function.Dyadic.Scalar.Miscellaneous;
using AplusCore.Runtime.Function.Dyadic.Scalar.Relational;
using AplusCore.Runtime.Function.Dyadic.Scalar.Elementary;
using AplusCore.Runtime.Function.Dyadic.NonScalar.Structural;
using AplusCore.Runtime.Function.Dyadic.NonScalar.Comparison;
using AplusCore.Runtime.Function.Dyadic.NonScalar.Selection;
using AplusCore.Runtime.Function.Dyadic.NonScalar.Other;
using AplusCore.Runtime.Function.Dyadic.NonScalar.Computational;
using AplusCore.Runtime.Function.Dyadic.Product;
using AplusCore.Runtime.Function.Dyadic.Scalar.Bitwise;

namespace AplusCore.Runtime.Function.Dyadic
{
    internal static class DyadicFunctionInstance
    {
        #region SCALAR

        #region Arithmetic

        internal static readonly AbstractDyadicFunction Add = new Add();
        internal static readonly AbstractDyadicFunction Divide = new Divide();
        internal static readonly AbstractDyadicFunction Multiply = new Multiply();
        internal static readonly AbstractDyadicFunction Subtract = new Subtract();
        
        #endregion

        #region Logical

        internal static readonly AbstractDyadicFunction And = new And();
        internal static readonly AbstractDyadicFunction Or = new Or();

        #endregion

        #region Miscellaneous

        internal static readonly AbstractDyadicFunction CombineSymbols = new CombineSymbols();
        internal static readonly AbstractDyadicFunction Max = new Max();
        internal static readonly AbstractDyadicFunction Min = new Min();
        internal static readonly AbstractDyadicFunction Residue = new Residue();
        
        #endregion

        #region Relational

        internal static readonly AbstractDyadicFunction EqualTo = new EqualTo();
        internal static readonly AbstractDyadicFunction GreaterThan = new GreaterThan();
        internal static readonly AbstractDyadicFunction GreaterThanOrEqualTo = new GreaterThanOrEqualTo();
        internal static readonly AbstractDyadicFunction LessThan = new LessThan();
        internal static readonly AbstractDyadicFunction LessThanOrEqualTo = new LessThanOrEqualTo();
        internal static readonly AbstractDyadicFunction NotEqualTo = new NotEqualTo();

        #endregion

        #region Elementary

        internal static readonly AbstractDyadicFunction Circle = new Circle();
        internal static readonly AbstractDyadicFunction Log = new Log();
        internal static readonly AbstractDyadicFunction Power = new Power();

        #endregion

        #endregion

        #region NONSCALAR

        #region Comparison

        internal static readonly AbstractDyadicFunction Bins = new Bins();
        internal static readonly AbstractDyadicFunction Find = new Find();
        internal static readonly AbstractDyadicFunction Match = new Match();
        internal static readonly AbstractDyadicFunction Member = new Member();

        #endregion

        #region Computational

        internal static readonly AbstractDyadicFunction Deal = new Deal();
        internal static readonly AbstractDyadicFunction Decode = new Decode();
        internal static readonly AbstractDyadicFunction Encode = new Encode();

        #endregion

        #region Selection

        internal static readonly Choose Choose = new Choose();
        internal static readonly AbstractDyadicFunction Left = new Left();
        internal static readonly Pick Pick = new Pick();

        #endregion

        #region Structural

        internal static readonly AbstractDyadicFunction Catenate = new Catenate();
        internal static readonly AbstractDyadicFunction Drop = new Drop();
        internal static readonly AbstractDyadicFunction Expand = new Expand();
        internal static readonly AbstractDyadicFunction Laminate = new Laminate();
        internal static readonly AbstractDyadicFunction Partition = new Partition();
        internal static readonly AbstractDyadicFunction Replicate = new Replicate();
        internal static readonly AbstractDyadicFunction Reshape = new Reshape();
        internal static readonly AbstractDyadicFunction Restructure = new Restructure();
        internal static readonly AbstractDyadicFunction Rotate = new Rotate();
        internal static readonly AbstractDyadicFunction Take = new Take();
        internal static readonly AbstractDyadicFunction TransposeAxis = new TransposeAxes();

        #endregion

        #region Other

        internal static readonly AbstractDyadicFunction Cast = new Cast();
        internal static readonly AbstractDyadicFunction ExecuteInContext = new ExecuteInContext();
        internal static readonly AbstractDyadicFunction Format = new Format();
        internal static readonly AbstractDyadicFunction Map = new Map();
        internal static readonly AbstractDyadicFunction ValueInContext = new ValueInContext();

        #endregion

        #endregion

        #region OPERATOR

        #region Inner Product

        internal static readonly AbstractDyadicFunction IPAddMultiply = new IPAddMultiply();
        internal static readonly AbstractDyadicFunction IPMaxAdd = new IPMaxAdd();
        internal static readonly AbstractDyadicFunction IPMinAdd = new IPMinAdd();

        #endregion

        #region Outer Product

        internal static readonly AbstractDyadicFunction OPAdd = new OPAdd();
        internal static readonly AbstractDyadicFunction OPDivide = new OPDivide();
        internal static readonly AbstractDyadicFunction OPEqual = new OPEqual();
        internal static readonly AbstractDyadicFunction OPGreater = new OPGreater();
        internal static readonly AbstractDyadicFunction OPGreaterEqual = new OPGreaterEqual();
        internal static readonly AbstractDyadicFunction OPLess = new OPLess();
        internal static readonly AbstractDyadicFunction OPLessEqual = new OPLessEqual();
        internal static readonly AbstractDyadicFunction OPMax = new OPMax();
        internal static readonly AbstractDyadicFunction OPMin = new OPMin();
        internal static readonly AbstractDyadicFunction OPMultiply = new OPMultiply();
        internal static readonly AbstractDyadicFunction OPNotEqual = new OPNotEqual();
        internal static readonly AbstractDyadicFunction OPPower = new OPPower();
        internal static readonly AbstractDyadicFunction OPResidue = new OPResidue();
        internal static readonly AbstractDyadicFunction OPSubtract = new OPSubtract();

        #endregion

        #region Bitwise

        internal static readonly AbstractDyadicFunction BitwiseAnd = new BitwiseAnd();
        internal static readonly AbstractDyadicFunction BitwiseOr = new BitwiseOr();
        internal static readonly AbstractDyadicFunction BitwiseLess = new BitwiseLess();
        internal static readonly AbstractDyadicFunction BitwiseLessEqual = new BitwiseLessEqual();
        internal static readonly AbstractDyadicFunction BitwiseEqual = new BitwiseEqual();
        internal static readonly AbstractDyadicFunction BitwiseGreaterEqual = new BitwiseGreaterEqual();
        internal static readonly AbstractDyadicFunction BitwiseGreater = new BitwiseGreater();
        internal static readonly AbstractDyadicFunction BitwiseNotEqual = new BitwiseNotEqual();

        #endregion

        #endregion
    }
}
