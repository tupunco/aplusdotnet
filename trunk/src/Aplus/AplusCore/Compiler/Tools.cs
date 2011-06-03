using AplusCore.Runtime;
using AplusCore.Types;
using AplusCore.Types.MemoryMapped;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler
{
    class Tools
    {
        /// <summary>
        /// If the argument is Memory-mapped file and the mode is read only, we clone it.
        /// The clone creates an ordinary array.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static DLR.Expression CloneMemoryMappedFile(DLR.Expression argument)
        {
            DLR.ParameterExpression argumentParam = DLR.Expression.Parameter(typeof(AType), "__VALUE__");

            return DLR.Expression.Block(
                new DLR.ParameterExpression[] { argumentParam },
                DLR.Expression.Assign(argumentParam, argument),
                DLR.Expression.IfThen(
                    DLR.Expression.AndAlso(
                            DLR.Expression.Property(argumentParam, "IsMemoryMappedFile"),
                            DLR.Expression.Equal(
                                DLR.Expression.Property(
                                    DLR.Expression.Convert(
                                        DLR.Expression.Property(argumentParam, "Data"),
                                        typeof(IMapped)
                                    ),
                                    "Mode"
                                ),
                                DLR.Expression.Constant(MemoryMappedFileMode.Read)
                            )
                        ),
                        DLR.Expression.Assign(
                            DLR.Expression.Property(argumentParam, "Data"),
                            DLR.Expression.Property(
                                DLR.Expression.Call(
                                    argumentParam,
                                    typeof(AType).GetMethod("Clone")
                                ),
                                "Data"
                            )
                        )
                    ),
                    argumentParam
            );
        }
    }
}
