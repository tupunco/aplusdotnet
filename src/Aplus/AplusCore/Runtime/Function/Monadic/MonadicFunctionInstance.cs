using AplusCore.Runtime.Function.Monadic.NonScalar.Comprasion;
using AplusCore.Runtime.Function.Monadic.NonScalar.Computational;
using AplusCore.Runtime.Function.Monadic.NonScalar.Informational;
using AplusCore.Runtime.Function.Monadic.NonScalar.Other;
using AplusCore.Runtime.Function.Monadic.NonScalar.Selection;
using AplusCore.Runtime.Function.Monadic.NonScalar.Structural;
using AplusCore.Runtime.Function.Monadic.Operator.Reduction;
using AplusCore.Runtime.Function.Monadic.Operator.Scan;
using AplusCore.Runtime.Function.Monadic.Scalar.Arithmetic;
using AplusCore.Runtime.Function.Monadic.Scalar.Elementary;
using AplusCore.Runtime.Function.Monadic.Scalar.Logical;
using AplusCore.Runtime.Function.Monadic.Scalar.Miscellaneous;

namespace AplusCore.Runtime.Function.Monadic
{
    class MonadicFunctionInstance
    {
        #region SCALAR

        #region Arithmetic

        internal static readonly AbstractMonadicFunction Identity = new Identity();
        internal static readonly AbstractMonadicFunction Negate = new Negate();

        #endregion

        #region Logical

        internal static readonly AbstractMonadicFunction Not = new Not();

        #endregion

        #region Miscellaneous

        internal static readonly AbstractMonadicFunction AbosulteValue = new AbsoluteValue();
        internal static readonly AbstractMonadicFunction Ceiling = new Ceiling();
        internal static readonly AbstractMonadicFunction Floor = new Floor();
        internal static readonly AbstractMonadicFunction Reciprocal = new Reciprocal();
        internal static readonly AbstractMonadicFunction Roll = new Roll();
        internal static readonly AbstractMonadicFunction Sign = new Sign();

        #endregion

        #region Elementary

        internal static readonly AbstractMonadicFunction Exponential = new Exponential();
        internal static readonly AbstractMonadicFunction NaturalLog = new NaturalLog();
        internal static readonly AbstractMonadicFunction PiTimes = new PiTimes();

        #endregion

        #endregion

        #region NONSCALAR

        #region Comprasion

        internal static readonly AbstractMonadicFunction GradeDown = new GradeDown();
        internal static readonly AbstractMonadicFunction GradeUp = new GradeUp();
        internal static readonly AbstractMonadicFunction PartitionCount = new PartitionCount();

        #endregion

        #region Computational

        internal static readonly AbstractMonadicFunction MatrixInverse = new MatrixInverse();
        internal static readonly AbstractMonadicFunction Pack = new Pack();
        internal static readonly AbstractMonadicFunction Unpack = new Unpack();

        #endregion

        #region Informational

        internal static readonly AbstractMonadicFunction Count = new Count();
        internal static readonly AbstractMonadicFunction Depth = new Depth();
        internal static readonly AbstractMonadicFunction Shape = new Shape();
        internal static readonly AbstractMonadicFunction Type = new TypeFunction();

        #endregion

        #region Selection

        internal static readonly AbstractMonadicFunction NullFunction = new NullFunction();
        internal static readonly AbstractMonadicFunction Right = new Right();
        internal static readonly AbstractMonadicFunction SeparateSymbols = new SeparateSymbols();

        #endregion

        #region Structural

        internal static readonly AbstractMonadicFunction Disclose = new Disclose();
        internal static readonly AbstractMonadicFunction Enclose = new Enclose();
        internal static readonly AbstractMonadicFunction Interval = new Interval();
        internal static readonly AbstractMonadicFunction ItemRavel = new ItemRavel();
        internal static readonly AbstractMonadicFunction Rake = new Rake();
        internal static readonly AbstractMonadicFunction Ravel = new Ravel();
        internal static readonly AbstractMonadicFunction Raze = new Raze();
        internal static readonly AbstractMonadicFunction Reverse = new Reverse();
        internal static readonly AbstractMonadicFunction Transpose = new Transpose();

        #endregion

        #region Other

        internal static readonly AbstractMonadicFunction ExecuteFunction = new ExecuteFunction();
        internal static readonly AbstractMonadicFunction DefaultFormat = new DefaultFormat();
        internal static readonly AbstractMonadicFunction Print = new Print();
        internal static readonly AbstractMonadicFunction Signal = new Signal();
        internal static readonly AbstractMonadicFunction Stop = new Stop();
        internal static readonly AbstractMonadicFunction MapIn = new MapIn();
        internal static readonly AbstractMonadicFunction Value = new Value();

        #endregion

        #endregion

        #region OPERATOR

        #region Bitwise

        internal static readonly AbstractMonadicFunction BitwiseNot = new BitwiseNot();

        #endregion

        #region Reduction

        internal static readonly AbstractMonadicFunction ReduceAdd = new ReduceAdd();
        internal static readonly AbstractMonadicFunction ReduceMultiply = new ReduceMultiply();
        internal static readonly AbstractMonadicFunction ReduceOr = new ReduceOr();
        internal static readonly AbstractMonadicFunction ReduceAnd = new ReduceAnd();
        internal static readonly AbstractMonadicFunction ReduceMax = new ReduceMax();
        internal static readonly AbstractMonadicFunction ReduceMin = new ReduceMin();

        #endregion

        #region Scan

        internal static readonly AbstractMonadicFunction ScanAdd = new ScanAdd();
        internal static readonly AbstractMonadicFunction ScanMultiply = new ScanMultiply();
        internal static readonly AbstractMonadicFunction ScanMin = new ScanMin();
        internal static readonly AbstractMonadicFunction ScanMax = new ScanMax();
        internal static readonly AbstractMonadicFunction ScanAnd = new ScanAnd();
        internal static readonly AbstractMonadicFunction ScanOr = new ScanOr();

        #endregion

        #endregion

    }
}
