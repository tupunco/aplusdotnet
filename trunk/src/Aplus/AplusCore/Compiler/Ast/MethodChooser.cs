using System.Collections.Generic;

using AplusCore.Compiler.Grammar;
using AplusCore.Runtime.Function.Dyadic;
using AplusCore.Runtime.Function.Monadic;

namespace AplusCore.Compiler.AST
{
    class MethodChooser
    {
        #region Monadic Token Rewrite rules

        /// <summary>
        /// Monadic Token Type rewrite rules
        /// </summary>
        private static Dictionary<Tokens, Tokens> ReWriteRules = new Dictionary<Tokens, Tokens>()
        {
            { Tokens.ABSOLUTEVALUE, Tokens.RESIDUE },
            { Tokens.CEILING, Tokens.MAX },
            { Tokens.COUNT, Tokens.CHOOSE },
            { Tokens.DEFAULTFORMAT, Tokens.FORMAT },
            { Tokens.DEPTH, Tokens.MATCH },
            { Tokens.DISCLOSE, Tokens.GT },
            { Tokens.ENCLOSE, Tokens.LT },
            { Tokens.EXPONENTIAL, Tokens.POWER },
            { Tokens.EXECUTE, Tokens.EXECUTEINCONTEXT },
            { Tokens.FLOOR, Tokens.MIN },
            { Tokens.GRADEUP, Tokens.BINS },
            { Tokens.IDENTITY, Tokens.ADD },
            { Tokens.INTERVAL, Tokens.FIND },
            { Tokens.ITEMRAVEL, Tokens.RESTRUCTURE },
            { Tokens.MAPIN, Tokens.MAP },
            { Tokens.MATRIXINVERSE, Tokens.SOLVE },
            { Tokens.NATURALLOG, Tokens.LOG },
            { Tokens.NEGATE, Tokens.SUBTRACT },
            { Tokens.NOT, Tokens.LAMINATE },
            { Tokens.NULL, Tokens.LEFT },
            { Tokens.PACK, Tokens.DECODE },
            { Tokens.PARTITIONCOUNT, Tokens.PARTITION },
            { Tokens.PITIMES, Tokens.CIRCLE },
            { Tokens.PRINT, Tokens.DROP },
            { Tokens.RAKE, Tokens.MEMBER },
            { Tokens.RAVEL, Tokens.CATENATE },
            { Tokens.RAZE, Tokens.PICK },
            { Tokens.RECIPROCAL, Tokens.DIVIDE },
            { Tokens.REVERSE, Tokens.ROTATE },
            { Tokens.ROLL, Tokens.DEAL },
            { Tokens.SEPARATESYMBOLS, Tokens.COMBINESYMBOLS },
            { Tokens.SHAPE, Tokens.RESHAPE },
            { Tokens.SIGN, Tokens.MULTIPLY },
            { Tokens.SIGNAL, Tokens.TAKE },
            { Tokens.STOP, Tokens.AND },
            { Tokens.TRANSPOSE, Tokens.TRANSPOSEAXES },
            { Tokens.TYPE, Tokens.OR },
            { Tokens.UNPACK, Tokens.ENCODE },
            { Tokens.VALUE, Tokens.VALUEINCONTEXT }
        };

        #endregion


        /// <summary>
        /// Return the A+ native monadic function for the specified monadic token
        /// </summary>
        /// <returns><b>null</b> if no monadic function found</returns>
        internal static AbstractMonadicFunction GetMonadicMethod(Token token)
        {
            switch (token.Type)
            {
                #region Monadic Scalar

                case Tokens.ABSOLUTEVALUE:
                    return MonadicFunctionInstance.AbosulteValue;
                case Tokens.CEILING:
                    return MonadicFunctionInstance.Ceiling;
                case Tokens.EXPONENTIAL:
                    return MonadicFunctionInstance.Exponential;
                case Tokens.FLOOR:
                    return MonadicFunctionInstance.Floor;
                case Tokens.IDENTITY:
                    return MonadicFunctionInstance.Identity;
                case Tokens.NATURALLOG:
                    return MonadicFunctionInstance.NaturalLog;
                case Tokens.NEGATE:
                    return MonadicFunctionInstance.Negate;
                case Tokens.NOT:
                    return MonadicFunctionInstance.Not;
                case Tokens.PITIMES:
                    return MonadicFunctionInstance.PiTimes;
                case Tokens.RECIPROCAL:
                    return MonadicFunctionInstance.Reciprocal;
                case Tokens.ROLL:
                    return MonadicFunctionInstance.Roll;
                case Tokens.SIGN:
                    return MonadicFunctionInstance.Sign;

                #endregion

                #region Monadic Non Scalar

                case Tokens.COUNT:
                    return MonadicFunctionInstance.Count;
                case Tokens.DEPTH:
                    return MonadicFunctionInstance.Depth;
                case Tokens.DISCLOSE:
                    return MonadicFunctionInstance.Disclose;
                case Tokens.ENCLOSE:
                    return MonadicFunctionInstance.Enclose;
                case Tokens.EXECUTE:
                    return MonadicFunctionInstance.ExecuteFunction;
                case Tokens.DEFAULTFORMAT:
                    return MonadicFunctionInstance.DefaultFormat;
                case Tokens.GRADEDOWN:
                    return MonadicFunctionInstance.GradeDown;
                case Tokens.GRADEUP:
                    return MonadicFunctionInstance.GradeUp;
                case Tokens.INTERVAL:
                    return MonadicFunctionInstance.Interval;
                case Tokens.ITEMRAVEL:
                    return MonadicFunctionInstance.ItemRavel;
                case Tokens.MAPIN:
                    return MonadicFunctionInstance.MapIn;
                case Tokens.MATRIXINVERSE:
                    return MonadicFunctionInstance.MatrixInverse;
                case Tokens.NULL:
                    return MonadicFunctionInstance.NullFunction;
                case Tokens.PACK:
                    return MonadicFunctionInstance.Pack;
                case Tokens.PARTITIONCOUNT:
                    return MonadicFunctionInstance.PartitionCount;
                case Tokens.PRINT:
                    return MonadicFunctionInstance.Print;
                case Tokens.RAKE:
                    return MonadicFunctionInstance.Rake;
                case Tokens.RAVEL:
                    return MonadicFunctionInstance.Ravel;
                case Tokens.RAZE:
                    return MonadicFunctionInstance.Raze;
                case Tokens.REVERSE:
                    return MonadicFunctionInstance.Reverse;
                case Tokens.RIGHT:
                    return MonadicFunctionInstance.Right;
                case Tokens.SEPARATESYMBOLS:
                    return MonadicFunctionInstance.SeparateSymbols;
                case Tokens.SHAPE:
                    return MonadicFunctionInstance.Shape;
                case Tokens.SIGNAL:
                    return MonadicFunctionInstance.Signal;
                case Tokens.STOP:
                    return MonadicFunctionInstance.Stop;
                case Tokens.TRANSPOSE:
                    return MonadicFunctionInstance.Transpose;
                case Tokens.TYPE:
                    return MonadicFunctionInstance.Type;
                case Tokens.UNPACK:
                    return MonadicFunctionInstance.Unpack;
                case Tokens.VALUE:
                    return MonadicFunctionInstance.Value;

                #endregion

                #region Monadic Operator

                case Tokens.RADD:
                    return MonadicFunctionInstance.ReduceAdd;
                case Tokens.RMULTIPLY:
                    return MonadicFunctionInstance.ReduceMultiply;
                case Tokens.ROR:
                    return MonadicFunctionInstance.ReduceOr;
                case Tokens.RAND:
                    return MonadicFunctionInstance.ReduceAnd;
                case Tokens.RMAX:
                    return MonadicFunctionInstance.ReduceMax;
                case Tokens.RMIN:
                    return MonadicFunctionInstance.ReduceMin;
                case Tokens.SADD:
                    return MonadicFunctionInstance.ScanAdd;
                case Tokens.SMULTIPLY:
                    return MonadicFunctionInstance.ScanMultiply;
                case Tokens.SMIN:
                    return MonadicFunctionInstance.ScanMin;
                case Tokens.SMAX:
                    return MonadicFunctionInstance.ScanMax;
                case Tokens.SAND:
                    return MonadicFunctionInstance.ScanAnd;
                case Tokens.SOR:
                    return MonadicFunctionInstance.ScanOr;

                #endregion

                #region Bitwise Operator

                case Tokens.BWNOT:
                    return MonadicFunctionInstance.BitwiseNot;

                #endregion

                default:
                    return null;
            }
        }

        /// <summary>
        /// Return the A+ native dyadic function for the specified dyadic token.
        /// </summary>
        /// <returns><b>null</b> if no dyadic function found</returns>
        internal static AbstractDyadicFunction GetDyadicMethod(Token token)
        {
            switch (token.Type)
            {
                #region Dyadic Scalar

                case Tokens.ADD:
                    return DyadicFunctionInstance.Add;
                case Tokens.AND:
                    return DyadicFunctionInstance.And;
                case Tokens.CIRCLE:
                    return DyadicFunctionInstance.Circle;
                case Tokens.COMBINESYMBOLS:
                    return DyadicFunctionInstance.CombineSymbols;
                case Tokens.DIVIDE:
                    return DyadicFunctionInstance.Divide;
                case Tokens.EQUAL:
                    return DyadicFunctionInstance.EqualTo;
                case Tokens.GT:
                    return DyadicFunctionInstance.GreaterThan;
                case Tokens.GTE:
                    return DyadicFunctionInstance.GreaterThanOrEqualTo;
                case Tokens.LOG:
                    return DyadicFunctionInstance.Log;
                case Tokens.LT:
                    return DyadicFunctionInstance.LessThan;
                case Tokens.LTE:
                    return DyadicFunctionInstance.LessThanOrEqualTo;
                case Tokens.MAX:
                    return DyadicFunctionInstance.Max;
                case Tokens.MIN:
                    return DyadicFunctionInstance.Min;
                case Tokens.MULTIPLY:
                    return DyadicFunctionInstance.Multiply;
                case Tokens.NOTEQUAL:
                    return DyadicFunctionInstance.NotEqualTo;
                /*case Tokens.OR:
                    //Cast nonscalar primitive functon has not implementeted yet!
                    return DyadicFunctionInstance.Or;
                    return DyadicFunctionInstance.Cast;*/
                case Tokens.POWER:
                    return DyadicFunctionInstance.Power;
                case Tokens.RESIDUE:
                    return DyadicFunctionInstance.Residue;
                case Tokens.SUBTRACT:
                    return DyadicFunctionInstance.Subtract;

                #endregion

                #region Dyadic Non Scalar

                case Tokens.BINS:
                    return DyadicFunctionInstance.Bins;
                case Tokens.CATENATE:
                    return DyadicFunctionInstance.Catenate;
                case Tokens.CHOOSE:
                    return DyadicFunctionInstance.Choose;
                case Tokens.DEAL:
                    return DyadicFunctionInstance.Deal;
                case Tokens.DECODE:
                    return DyadicFunctionInstance.Decode;
                case Tokens.DROP:
                    return DyadicFunctionInstance.Drop;
                case Tokens.ENCODE:
                    return DyadicFunctionInstance.Encode;
                case Tokens.EXECUTEINCONTEXT:
                    return DyadicFunctionInstance.ExecuteInContext;
                case Tokens.EXPAND:
                    return DyadicFunctionInstance.Expand;
                case Tokens.FORMAT:
                    return DyadicFunctionInstance.Format;
                case Tokens.FIND:
                    return DyadicFunctionInstance.Find;
                case Tokens.LAMINATE:
                    return DyadicFunctionInstance.Laminate;
                case Tokens.LEFT:
                    return DyadicFunctionInstance.Left;
                case Tokens.MAP:
                    return DyadicFunctionInstance.Map;
                case Tokens.MATCH:
                    return DyadicFunctionInstance.Match;
                case Tokens.MEMBER:
                    return DyadicFunctionInstance.Member;
                case Tokens.PARTITION:
                    return DyadicFunctionInstance.Partition;
                case Tokens.PICK:
                    return DyadicFunctionInstance.Pick;
                case Tokens.REPLICATE:
                    return DyadicFunctionInstance.Replicate;
                case Tokens.RESHAPE:
                    return DyadicFunctionInstance.Reshape;
                case Tokens.RESTRUCTURE:
                    return DyadicFunctionInstance.Restructure;
                case Tokens.ROTATE:
                    return DyadicFunctionInstance.Rotate;
                case Tokens.SOLVE:
                    return DyadicFunctionInstance.Solve;
                case Tokens.TAKE:
                    return DyadicFunctionInstance.Take;
                case Tokens.TRANSPOSEAXES:
                    return DyadicFunctionInstance.TransposeAxis;
                case Tokens.VALUEINCONTEXT:
                    return DyadicFunctionInstance.ValueInContext;

                #endregion

                #region Inner Products

                case Tokens.IPADDMULTIPLY:
                    return DyadicFunctionInstance.IPAddMultiply;
                case Tokens.IPMAXADD:
                    return DyadicFunctionInstance.IPMaxAdd;
                case Tokens.IPMINADD:
                    return DyadicFunctionInstance.IPMinAdd;

                #endregion

                #region Outer Products

                case Tokens.OPADD:
                    return DyadicFunctionInstance.OPAdd;
                case Tokens.OPDIVIDE:
                    return DyadicFunctionInstance.OPDivide;
                case Tokens.OPEQUAL:
                    return DyadicFunctionInstance.OPEqual;
                case Tokens.OPGT:
                    return DyadicFunctionInstance.OPGreater;
                case Tokens.OPGTE:
                    return DyadicFunctionInstance.OPGreaterEqual;
                case Tokens.OPLT:
                    return DyadicFunctionInstance.OPLess;
                case Tokens.OPLTE:
                    return DyadicFunctionInstance.OPLessEqual;
                case Tokens.OPMAX:
                    return DyadicFunctionInstance.OPMax;
                case Tokens.OPMIN:
                    return DyadicFunctionInstance.OPMin;
                case Tokens.OPMULTIPLY:
                    return DyadicFunctionInstance.OPMultiply;
                case Tokens.OPNOTEQUAL:
                    return DyadicFunctionInstance.OPNotEqual;
                case Tokens.OPPOWER:
                    return DyadicFunctionInstance.OPPower;
                case Tokens.OPRESIDUE:
                    return DyadicFunctionInstance.OPResidue;
                case Tokens.OPSUBSTRACT:
                    return DyadicFunctionInstance.Subtract;

                #endregion

                #region Bitwise Operators

                case Tokens.BWAND:
                    return DyadicFunctionInstance.BitwiseAnd;
                case Tokens.BWOR:
                    return DyadicFunctionInstance.BitwiseOr;
                case Tokens.BWLESS:
                    return DyadicFunctionInstance.BitwiseLess;
                case Tokens.BWLESSEQUAL:
                    return DyadicFunctionInstance.BitwiseLessEqual;
                case Tokens.BWEQUAL:
                    return DyadicFunctionInstance.BitwiseEqual;
                case Tokens.BWGREATEREQUAL:
                    return DyadicFunctionInstance.BitwiseGreaterEqual;
                case Tokens.BWGREATER:
                    return DyadicFunctionInstance.BitwiseGreater;
                case Tokens.BWNOTEQUAL:
                    return DyadicFunctionInstance.BitwiseNotEqual;

                #endregion

                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a Monadic token to a Dyadic Token. If no conversion is available then the Token is not modified
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if there was a conversion otherwise false</returns>
        internal static bool ConvertToDyadicToken(Token token)
        {
            if (ReWriteRules.ContainsKey(token.Type))
            {
                token.Type = ReWriteRules[token.Type];
                return true;
            }

            return false;
        }
    }
}
