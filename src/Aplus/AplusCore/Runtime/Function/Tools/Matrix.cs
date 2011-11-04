using AplusCore.Types;

namespace AplusCore.Runtime.Function.Tools
{
    internal abstract class Matrix
    {
        #region Functionality

        internal abstract int Rows { get; }
        internal abstract int Columns { get; }

        internal abstract double this[int i, int j] { get; set; }

        internal abstract Matrix Clone();

        internal Matrix Transpose()
        {
            return new TransposedMatrix(this);
        }

        #endregion

        #region Operator

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            int rows = matrix1.Rows;
            int columns = matrix2.Columns;
            SimpleMatrix result = new SimpleMatrix(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    for (int k = 0; k < matrix1.Columns; k++)
                    {
                        result[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix matrix, double scalar)
        {
            return new ScaledMatrix(matrix, scalar);
        }

        #endregion

        #region Utility

        internal AType ToAType()
        {
            AType result = AArray.Create(ATypes.AFloat);

            for (int i = 0; i < this.Rows; i++)
            {
                AType row = AArray.Create(ATypes.AFloat);

                for (int j = 0; j < this.Columns; j++)
                {
                    row.Add(AFloat.Create(this[i, j]));
                }

                result.Add(row);
            }

            return result;
        }

        internal static Matrix MatrixFromAType(AType argument)
        {
            Matrix result = new SimpleMatrix(new double[argument.Shape[0], argument.Shape[1]]);

            for (int i = 0; i < argument.Shape[0]; i++)
            {
                for (int j = 0; j < argument.Shape[1]; j++)
                {
                    result[i, j] = argument[i][j].asFloat;
                }
            }

            return result;
        }

        #endregion
    }
}
