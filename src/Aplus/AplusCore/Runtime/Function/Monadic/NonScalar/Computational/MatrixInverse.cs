using System;
using System.Linq;

using AplusCore.Runtime.Function.Tools;
using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Computational
{
    class MatrixInverse : AbstractMonadicFunction
    {
        #region Entry Point

        public override AType Execute(AType argument, Aplus environment = null)
        {
            this.ErrorCheck(argument);
            return Compute(argument);
        }

        #endregion

        #region Computation

        /// <summary>
        /// Returns with the inverse of the input matrix.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private AType Compute(AType argument)
        {
            AType result;

            if (argument.Rank == 0)
            {
                result = AFloat.Create(1 / argument.asFloat);
            }
            else if (argument.Rank == 1)
            {
                result = VectorInverse(argument);
            }
            else if (argument.Shape[0] == argument.Shape[1])
            {
                Matrix matrix = Matrix.MatrixFromAType(argument);
                Matrix inverseMatrix = SquareMatrixInverse(matrix);
                // square matrix

                if (IsIllconditionedSquareMatrix(matrix, inverseMatrix))
                {
                    throw new Error.Domain(DomainErrorText);
                }

                result = inverseMatrix.ToAType();
            }
            else
            {
                // not a square matrix
                Matrix A = Matrix.MatrixFromAType(argument);
                Matrix AT = A.Transpose();
                Matrix ATA = AT * A;
                Matrix pseudoInverse = SquareMatrixInverse(ATA) * AT; // inv(A^T*A)*A^T

                result = pseudoInverse.ToAType();
            }

            return result;
        }

        private AType VectorInverse(AType argument)
        {
            AType result = AArray.Create(ATypes.AFloat);
            double sqrmagnitude = argument.Sum(item => item.asFloat * item.asFloat);

            for (int i = 0; i < argument.Length; i++)
            {
                result.Add(AFloat.Create(argument[i].asFloat / sqrmagnitude));
            }

            return result;
        }

        private Matrix SquareMatrixInverse(Matrix argument)
        {
            int size = argument.Rows;
            
            Matrix original = argument.Clone();
            Matrix inverse = new SimpleMatrix(size, size);

            for (int i = 0; i < size; i++)
            {
                inverse[i, i] = 1;
            }

            for (int i = 0; i < size; i++)
            {
                double reciprocal = 1 / original[i, i];

                for (int k = 0; k < size; k++)
                {
                    original[i, k] *= reciprocal;
                    inverse[i, k] *= reciprocal;
                }

                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    double quotient = original[j, i] / original[i, i];

                    for (int k = 0; k < size; k++)
                    {
                        original[j, k] = original[j, k] - quotient * original[i, k];
                        inverse[j, k] = inverse[j, k] - quotient * inverse[i, k];
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if ((i == j && original[i, j] != 1) ||
                        (i != j && original[i, j] != 0))
                    {
                        // no inverse found report error
                        throw new Error.Domain(DomainErrorText);
                    }
                }
            }

            return inverse;
        }

        #endregion

        #region Error Checking

        private static bool IsIllconditionedSquareMatrix(Matrix original, Matrix inverse)
        {
            int size = original.Columns;
            double norm = 0;
            double inverseNorm = 0;

            for (int i = 0; i < size; i++)
            {
                double localNorm = 0;
                double localInverseNorm = 0;

                for (int j = 0; j < size; j++)
                {
                    localNorm += Math.Abs(original[i, j]);
                    localInverseNorm += Math.Abs(inverse[i, j]);
                }

                norm = Math.Max(localNorm, norm);
                inverseNorm = Math.Max(inverseNorm, localInverseNorm);
            }

            double condition = norm * inverseNorm;
            return Math.Log(condition) <= Math.Pow(10, -13);
        }

        private void ErrorCheck(AType argument)
        {
            if (argument.Type != ATypes.AInteger && argument.Type != ATypes.AFloat)
            {
                throw new Error.Type(TypeErrorText);
            }

            switch (argument.Rank)
            {
                case 0:
                case 1:
                    AType sumValue = (argument.Rank == 0)
                        ? argument  // a simple scalar
                        : AFloat.Create(argument.Sum(item => item.asFloat)); // we have a vector
                    
                    if (sumValue.IsTolerablyWholeNumber && sumValue.asInteger == 0)
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                    break;

                case 2:
                    if (argument.Shape[1] > argument.Shape[0])
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                    break;

                default:
                    throw new Error.Rank(RankErrorText);
            }
        }

        #endregion
    }
}
