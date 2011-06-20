using System;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.NonScalar.Computational
{
    class MatrixInverse : AbstractMonadicFunction
    {
        #region Entry Point

        public override AType Execute(AType argument, AplusEnvironment environment = null)
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
                result = ScalarInverse(argument);
            }
            else if (argument.Rank == 1)
            {
                result = VectorInverse(argument);
            }
            else
            {
                if (argument.Shape[0] == argument.Shape[1])
                {
                    // square matrix
                    result = SquareMatrixInverse(argument);
                    if (IsIllconditionedSquareMatrix(argument, result))
                    {
                        throw new Error.Domain(DomainErrorText);
                    }
                }
                else
                {
                    // not a square matrix
                    // TODO: implement pseudo-inverse
                    throw new NotImplementedException("pseudo-inverse not implemented yet");
                }
            }

            return result;
        }

        public AType ScalarInverse(AType argument)
        {
            return AFloat.Create(1 / argument.asFloat);
        }

        public AType VectorInverse(AType argument)
        {
            double sqrmagnitude = 0;
            argument.Type = ATypes.AFloat;

            for (int i = 0; i < argument.Length; i++)
            {
                sqrmagnitude += (argument[i].asFloat * argument[i].asFloat);
            }

            for (int i = 0; i < argument.Length; i++)
            {
                argument[i] = AFloat.Create(argument[i].asFloat / sqrmagnitude);
            }

            return argument;
        }

        public AType SquareMatrixInverse(AType argument)
        {
            AType result;
            double[,] original = new double[argument.Shape[0], argument.Shape[1]];
            double[,] inverse = new double[argument.Shape[0], argument.Shape[1]];

            for (int i = 0; i < argument.Shape[0]; i++)
            {
                for (int j = 0; j < argument.Shape[1]; j++)
                {
                    original[i, j] = argument[i][j].asFloat;
                    inverse[i, j] = (i == j) ? 1 : 0;
                }
            }

            for (int i = 0; i < argument.Shape[0]; i++)
            {
                double reciprocal = 1 / original[i, i];

                for (int k = 0; k < argument.Shape[0]; k++)
                {
                    original[i, k] *= reciprocal;
                    inverse[i, k] *= reciprocal;
                }

                for (int j = 0; j < argument.Shape[1]; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    double quotient = original[j, i] / original[i, i];

                    for (int k = 0; k < argument.Shape[0]; k++)
                    {
                        original[j, k] = original[j, k] - quotient * original[i, k];
                        inverse[j, k] = inverse[j, k] - quotient * inverse[i, k];
                    }
                }
            }

            bool inverseExist = true;

            for (int i = 0; i < argument.Shape[0]; i++)
            {
                for (int j = 0; j < argument.Shape[1]; j++)
                {
                    if ((i == j && original[i, j] != 1) ||
                        (i != j && original[i, j] != 0))
                    {
                        inverseExist = false;
                    }
                }
            }

            if (!inverseExist)
            {
                throw new Error.Domain(DomainErrorText);
            }

            result = AArray.Create(ATypes.AFloat);

            for (int i = 0; i < argument.Shape[0]; i++)
            {
                AType row = AArray.Create(ATypes.AFloat);

                for (int j = 0; j < argument.Shape[1]; j++)
                {
                    row.Add(AFloat.Create(inverse[i, j]));
                }

                result.Add(row);
            }

            return result;
        }

        #endregion

        #region Error Checking

        public bool IsIllconditionedSquareMatrix(AType original, AType inverse)
        {
            double norm = 0;
            double inverseNorm = 0;
            double condition = 0;

            for (int i = 0; i < original.Shape[0]; i++)
            {
                double localNorm = 0;
                double localInverseNorm = 0;

                for (int j = 0; j < original.Shape[1]; j++)
                {
                    localNorm += Math.Abs(original[i][j].asFloat);
                    localInverseNorm += Math.Abs(inverse[i][j].asFloat);
                }

                norm = (norm < localNorm) ? localNorm : norm;
                inverseNorm = (inverseNorm < localInverseNorm) ? localInverseNorm : inverseNorm;
            }

            condition = norm * inverseNorm;
            return (Math.Log(condition) <= Math.Pow(10, -13));
        }

        private void ErrorCheck(AType argument)
        {
            if (argument.Type != ATypes.AInteger && argument.Type != ATypes.AFloat)
            {
                throw new Error.Type(TypeErrorText);
            }

            if (argument.Rank == 0)
            {
                if (argument.IsTolerablyWholeNumber && argument.asInteger == 0)
                {
                    throw new Error.Domain(DomainErrorText);
                }
            }
            else if (argument.Rank == 1)
            {
                double sum = 0;

                for (int i = 0; i < argument.Length; i++)
                {
                    sum += argument[i].asFloat;
                }

                AType sumAType = AFloat.Create(sum);

                if (sumAType.IsTolerablyWholeNumber && sumAType.asInteger == 0)
                {
                    throw new Error.Domain(DomainErrorText);
                }
            }
            else if (argument.Rank == 2)
            {
                if (argument.Shape[1] > argument.Shape[0])
                {
                    throw new Error.Domain(DomainErrorText);
                }
            }
            else if (argument.Rank > 2)
            {
                throw new Error.Rank(RankErrorText);
            }
        }

        #endregion
    }
}
