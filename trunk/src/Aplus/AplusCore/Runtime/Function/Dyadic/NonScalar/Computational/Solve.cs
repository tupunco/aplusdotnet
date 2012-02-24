using System;
using System.Collections.Generic;
using System.Linq;

using AplusCore.Types;
using AplusCore.Runtime.Function.Tools;

namespace AplusCore.Runtime.Function.Dyadic.NonScalar.Computational
{
    class Solve : AbstractDyadicFunction
    {
        #region Constants

        private static readonly int ITERATIONS = 255;
        private static readonly int SHIFT_CUT = 64;

        #endregion

        #region Computation

        public override AType Execute(AType right, AType left, Aplus environment = null)
        {
            this.ErrorCheck(right, left);
            return SolveEquation(left, right);
        }

        private void ErrorCheck(AType left, AType right)
        {
            if (!left.IsNumber || !right.IsNumber)
            {
                throw new Error.Type(TypeErrorText);
            }

            if (left.Rank > 2 || right.Rank > 2)
            {
                throw new Error.Rank(RankErrorText);
            }

            if (left.Length != right.Length) // the leading axis must pass
            {
                throw new Error.Length(LengthErrorText);
            }

            if (MatrixWithMoreColumnsThanRows(left) || MatrixWithMoreColumnsThanRows(right))
            {
                throw new Error.Domain(DomainErrorText);
            }
        }

        private AType SolveEquation(AType constants, AType equations)
        {
            AType lhs;
            AType rhs;

            if (constants.TryFirstScalar(out lhs, true) && equations.TryFirstScalar(out rhs, true))
            {
                // both left and right values are one element arrays.
                return AFloat.Create(lhs.asFloat / rhs.asFloat);
            }

            Matrix constantsArray = new SimpleMatrix(ExtractConstants(constants));
            Matrix originalEquations = new SimpleMatrix(FloatFromAType(equations));
            int[] rowsSequence;
            Matrix eliminatedConstants;
            GaussianElimination(originalEquations, constantsArray, out rowsSequence, out eliminatedConstants);

            AType result = AArray.Create(ATypes.AFloat);

            if (equations.Shape[0] == equations.Shape[1])
            {
                // square equation
                if (constants.Rank > 1)
                {
                    foreach (int item in rowsSequence)
                    {
                        AType subArray = AArray.Create(ATypes.AFloat);

                        for (int i = 0; i < eliminatedConstants.Columns; i++)
                        {
                            subArray.Add(AFloat.Create(eliminatedConstants[item, i]));
                        }

                        result.Add(subArray);
                    }
                }
                else
                {
                    foreach (int item in rowsSequence)
                    {
                        result.Add(AFloat.Create(eliminatedConstants[item, 0]));
                    }
                }
            }
            else
            {
                double[][] independentConstants = BuildIndependentConstants(rowsSequence, eliminatedConstants);
                double[] beta;
                double[] actualconstants;

                if (constants.Rank == 1)
                {
                    beta = independentConstants.Select(item => item[0]).ToArray();
                    actualconstants = constants.Select(item => item.asFloat).ToArray();

                    double[] solution = OverDeterminedEquationSolve(beta, actualconstants, originalEquations);

                    foreach (double item in solution)
                    {
                        result.Add(AFloat.Create(item));
                    }
                }
                else
                {
                    for (int objective = 0; objective < constants.Shape[1]; objective++)
                    {
                        beta = independentConstants.Select(item => item[objective]).ToArray();
                        actualconstants = constants.Select(item => item[objective].asFloat).ToArray();

                        double[] solution = OverDeterminedEquationSolve(beta, actualconstants, originalEquations);
                        AType solutionArray = AArray.Create(ATypes.AFloat);

                        foreach (double item in solution)
                        {
                            solutionArray.Add(AFloat.Create(item));
                        }

                        result.Add(solutionArray);
                    }
                }
            }

            return result;
        }

        private static double[][] BuildIndependentConstants(int[] rowsSequence, Matrix eliminatedConstants)
        {
            double[][] independentConstants = new double[rowsSequence.Length][];

            for (int row = 0; row < rowsSequence.Length; row++)
            {
                independentConstants[row] = new double[eliminatedConstants.Columns];

                for (int i = 0; i < eliminatedConstants.Columns; i++)
                {
                    independentConstants[row][i] = eliminatedConstants[rowsSequence[row], i];
                }
            }

            return independentConstants;
        }

        private static double[,] ExtractConstants(AType constants)
        {
            double[,] currentConstants;

            if (constants.Rank == 1)
            {
                currentConstants = new double[constants.Length, 1];

                for (int i = 0; i < constants.Length; i++)
                {
                    currentConstants[i, 0] = constants[i].asFloat;
                }
            }
            else
            {
                currentConstants = FloatFromAType(constants);
            }

            return currentConstants;
        }

        /// <summary>
        /// Solves an overdetermined linear system of equations.
        /// </summary>
        /// <param name="beta">The starting guess of the optimal solution.</param>
        /// <param name="constants">The constants of the equation system.</param>
        /// <param name="equations">The equations.</param>
        /// <returns>The solution with the least square difference.</returns>
        private double[] OverDeterminedEquationSolve(double[] beta, double[] constants, Matrix equations)
        {
            Matrix Jacobian = equations.Clone();
            Matrix equation =Jacobian.Transpose() * Jacobian; // JtT * Jt
            Matrix negatedTransposedEquations = Jacobian.Transpose() * -1; // - JtT   

            double[] x = beta; // the starting x is from the beta

            for (int iter = 0; iter < ITERATIONS; iter++)
            {
                bool solutionBetter = false; // TODO: is it enough to check only the improvement of the "objective funtion"?
                double sumOfDifference = FunctionSquareEvaluate(equations, constants, x).Sum();
                Matrix right = negatedTransposedEquations * new SimpleMatrix(FunctionEvaluate(equations, constants, x));
                int[] rowsSequence;
                Matrix eliminatedConstants;

                GaussianElimination(equation, right, out rowsSequence, out eliminatedConstants);

                // get the first column as a delta from the constants
                double[] delta = rowsSequence.Select(item => eliminatedConstants[item, 0]).ToArray();

                // shift-cutting
                double f = 1;
                for (int steps = 0; steps < SHIFT_CUT; steps++)
                {
                    double[] actualx = new double[x.Length];

                    for (int i = 0; i < actualx.Length; i++)
                    {
                        actualx[i] = x[i] - f * delta[i];
                    }

                    double actualSumOfDifference = FunctionSquareEvaluate(equations, constants, actualx).Sum();

                    if (actualSumOfDifference < sumOfDifference)
                    {
                        solutionBetter = true;
                        x = actualx;
                        sumOfDifference = actualSumOfDifference;
                        break;
                    }

                    f /= 2.0;
                }

                if (!solutionBetter)
                {
                    break;
                }
            }

            return x;
        }

        /// <summary>
        /// Gaussian elimination with partial pivoting.
        /// </summary>
        /// <param name="equation">The equation.</param>
        /// <param name="constants">The constants.</param>
        /// <param name="sequenceOfRows">The sequence of the pivot rows.</param>
        /// <param name="eliminatedConstants">The resulting constants after the elimination.</param>
        /// <returns>The eliminated matrix.</returns>
        private Matrix GaussianElimination(
            Matrix equation, Matrix constants, out int[] sequenceOfRows, out Matrix eliminatedConstants)
        {
            int rows = equation.Rows;
            int columns = equation.Columns;
            Matrix eliminatedMatrix = equation.Clone();
            eliminatedConstants = constants.Clone();
            sequenceOfRows = Enumerable.Repeat<int>(-1, columns).ToArray();

            // gaussian elimination with partial pivoting
            for (int i = 0; i < columns; i++)
            {
                double max = 0;
                int actualpivot = -1;

                for (int k = 0; k < rows; k++)
                {
                    if (Math.Abs(eliminatedMatrix[k, i]) > max && !sequenceOfRows.Contains(k))
                    {
                        actualpivot = k;
                        max = eliminatedMatrix[k, i];
                    }
                }

                if (actualpivot == -1)
                {
                    // underdetermined equation
                    throw new Error.Domain(this.DomainErrorText);
                }

                sequenceOfRows[i] = actualpivot;
                double reciprocal = 1 / max;

                // divide the row of the pivot item with the pivot item
                for (int k = 0; k < columns; k++)
                {
                    eliminatedMatrix[actualpivot, k] *= reciprocal;
                }

                // divide the row of the pivot item with the pivot item in the constants too
                for (int k = 0; k < eliminatedConstants.Columns; k++)
                {
                    eliminatedConstants[actualpivot, k] *= reciprocal;
                }

                // eliminate with the pivot item's row (in the constants too)
                for (int j = 0; j < rows; j++)
                {
                    if (j == actualpivot)
                    {
                        continue;
                    }

                    double quotient = eliminatedMatrix[j, i] / eliminatedMatrix[actualpivot, i];

                    for (int k = 0; k < columns; k++)
                    {
                        eliminatedMatrix[j, k] -= quotient * eliminatedMatrix[actualpivot, k];
                    }

                    for (int k = 0; k < eliminatedConstants.Columns; k++)
                    {
                        eliminatedConstants[j, k] -= quotient * eliminatedConstants[actualpivot, k];
                    }
                }
            }

            return eliminatedMatrix;
        }

        private static double[,] FloatFromAType(AType argument)
        {
            int rows = argument.Shape[0];
            int columns = argument.Shape[1];
            double[,] result = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = argument[i][j].asFloat;
                }
            }

            return result;
        }

        private static bool MatrixWithMoreColumnsThanRows(AType argument)
        {
            return (argument.Rank == 2) && (argument.Shape[0] < argument.Shape[1]);
        }

        #endregion

        #region Methods for Matrices

        /// <summary>
        /// Calculates the values of rows, and the differences from the constants.
        /// </summary>
        /// <param name="matrix">The matrix to evaluate.</param>
        /// <param name="constants">The constants.</param>
        /// <param name="x">The x to substitue.</param>
        /// <returns>The differences of M(x) from the constants.</returns>
        private static double[] FunctionEvaluate(Matrix matrix, double[] constants, double[] x)
        {
            double[] result = new double[matrix.Rows];

            for (int i = 0; i < constants.Length; i++)
            {
                double actRow = 0;

                for (int j = 0; j < matrix.Columns; j++)
                {
                    actRow += matrix[i, j] * x[j];
                }

                result[i] = constants[i] - actRow;
            }

            return result;
        }

        /// <summary>
        /// Calculates the square differences from the constants.
        /// </summary>
        /// <param name="matrix">The matrix to evaluate.</param>
        /// <param name="constants">The constants.</param>
        /// <param name="x">The x to substitue.</param>
        /// <returns>The squares of <see cref="FunctionEvaulate"/>.</returns>
        private static double[] FunctionSquareEvaluate(Matrix matrix, double[] constants, double[] x)
        {
            double[] functionEvaluated = FunctionEvaluate(matrix, constants, x);
            double[] result = new double[functionEvaluated.GetLength(0)];

            for (int i = 0; i < functionEvaluated.GetLength(0); i++)
            {
                result[i] = Math.Pow(functionEvaluated[i], 2);
            }

            return result;
        }

        #endregion
    }
}
