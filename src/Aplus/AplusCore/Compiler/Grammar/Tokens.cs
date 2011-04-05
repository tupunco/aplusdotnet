using System;
using System.Collections.Generic;

namespace AplusCore.Compiler.Grammar
{
    public enum Tokens
    {
        SYSTEMNAME,
        QUALIFIEDNAME,
        UNQUALIFIEDNAME,
        INT,
        FLOAT,
        DO,

        STACKREFERENCE,

        // Monadic Scalar:
        ABSOLUTEVALUE,
        CEILING,
        EXPONENTIAL,
        FLOOR,
        IDENTITY,
        NATURALLOG,
        NEGATE,
        NOT,
        PITIMES,
        RECIPROCAL,
        ROLL,
        SIGN,

        // Monadic Operator:
        RADD,
        RAND,
        RMAX,
        RMIN,
        RMULTIPLY,
        ROR,
        SADD,
        SAND,
        SMAX,
        SMIN,
        SMULTIPLY,
        SOR,

        // Monadic Non Scalar:
        COUNT,
        DEFAULTFORMAT,
        DEPTH,
        DISCLOSE,
        ENCLOSE,
        EXECUTE,
        GRADEDOWN,
        GRADEUP,
        INTERVAL,
        ITEMRAVEL,
        MAPIN,
        NULL,
        PACK,
        PARTITIONCOUNT,
        PRINT,
        RAKE,
        RAVEL,
        RAZE,
        RESULT,
        REVERSE,
        RIGHT,
        SEPARATESYMBOLS,
        SHAPE,
        SIGNAL,
        STOP,
        TRANSPOSE,
        TYPE,
        UNPACK,
        VALUE,

        // Dyadic Scalar:
        ADD,
        AND,
        CIRCLE,
        COMBINESYMBOLS,
        DIVIDE,
        EQUAL,
        GT,
        GTE,
        LAMINATE,
        LOG,
        LT,
        LTE,
        MAX,
        MIN,
        MULTIPLY,
        NOTEQUAL,
        OR,
        PARTITION,
        POWER,
        RESIDUE,
        SUBTRACT,

        //Dyadic NonScalar:
        ASSIGN,
        BINS,
        CATENATE,
        CHOOSE,
        DEAL,
        DECODE,
        DROP,
        ENCODE,
        EXPAND,
        EXECUTEINCONTEXT, // Protected execute
        FIND,
        FORMAT,
        LEFT,
        MAP,
        MATCH,
        MEMBER,
        PICK,
        REPLICATE,
        RESHAPE,
        RESTRUCTURE,
        ROTATE,
        TAKE,
        TRANSPOSEAXES,
        VALUEINCONTEXT,

        //Dyadic Operator:
        IPADDMULTIPLY,
        IPMAXADD,
        IPMINADD,
        OPADD,
        OPDIVIDE,
        OPEQUAL,
        OPGT,
        OPGTE,
        OPLT,
        OPLTE,
        OPMAX,
        OPMIN,
        OPMULTIPLY,
        OPNOTEQUAL,
        OPPOWER,
        OPRESIDUE,
        OPSUBSTRACT,

        // Bitwise operator
        BWNOT,
        BWAND,
        BWOR,
        BWLESS,
        BWLESSEQUAL,
        BWGREATER,
        BWGREATEREQUAL,
        BWEQUAL,
        BWNOTEQUAL,

        RANK,
        EACH,
    }

    class TokenUtils
    {
        #region List of Dyadic Scalar Tokens

        private static List<Tokens> DyadicScalarTokens = new List<Tokens>()
        {
            Tokens.ADD,
            Tokens.AND,
            Tokens.CIRCLE,
            Tokens.COMBINESYMBOLS,
            Tokens.DIVIDE,
            Tokens.EQUAL,
            Tokens.GT,
            Tokens.GTE,
            Tokens.LAMINATE,
            Tokens.LOG,
            Tokens.LT,
            Tokens.LTE,
            Tokens.MAX,
            Tokens.MIN,
            Tokens.MULTIPLY,
            Tokens.NOTEQUAL,
            Tokens.OR,
            Tokens.PARTITION,
            Tokens.POWER,
            Tokens.RESIDUE,
            Tokens.SUBTRACT,
        };

        #endregion

        public static bool IsDyadicScalarToken(Tokens token)
        {
            return DyadicScalarTokens.Contains(token);
        }

        #region List of Monadic Scalar Tokens

        private static List<Tokens> MonadicScalarTokens = new List<Tokens>()
        {
            Tokens.ABSOLUTEVALUE,
            Tokens.CEILING,
            Tokens.EXPONENTIAL,
            Tokens.FLOOR,
            Tokens.IDENTITY,
            Tokens.NATURALLOG,
            Tokens.NEGATE,
            Tokens.NOT,
            Tokens.PITIMES,
            Tokens.RECIPROCAL,
            Tokens.ROLL,
            Tokens.SIGN,
        };

        #endregion

        public static bool IsMonadicScalarToken(Tokens token)
        {
            return MonadicScalarTokens.Contains(token);
        }

        #region List of Primitive Functions in assignment
        
        private static List<Tokens> PrimitiveFunctionsInAssignment = new List<Tokens>()
        {
            // Listed in Language Reference:
            Tokens.DROP,
            Tokens.EXPAND,
            Tokens.ITEMRAVEL,
            Tokens.RAVEL,
            Tokens.REPLICATE,
            Tokens.RESHAPE,
            Tokens.TAKE,
            Tokens.TRANSPOSE,
            Tokens.TRANSPOSEAXES,

            // Not listed in Language reference but still work the same way
            Tokens.LEFT

        };

        #endregion

        public static bool AllowedPrimitiveFunction(Tokens token)
        {
            return PrimitiveFunctionsInAssignment.Contains(token);
        }

    }
}