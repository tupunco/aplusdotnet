namespace AplusCore.Runtime.Function.Tools
{
    internal class SimpleMatrix : Matrix
    {
        #region Variables

        private int rows;
        private int columns;
        private double[,] coeffs;

        #endregion

        #region Constructors

        internal SimpleMatrix(double[,] coeffs)
        {
            this.rows = coeffs.GetLength(0);
            this.columns = coeffs.GetLength(1);
            this.coeffs = (double[,])coeffs.Clone();
        }

        internal SimpleMatrix(double[] vector)
        {
            this.rows = vector.Length;
            this.columns = 1;
            this.coeffs = new double[this.rows, this.columns];

            for (int i = 0; i < this.rows; i++)
            {
                this.coeffs[i, 0] = vector[i];
            }
        }

        internal SimpleMatrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            this.coeffs = new double[rows, columns];
        }

        #endregion

        #region Matrix members

        internal override int Rows
        {
            get { return this.rows; }
        }

        internal override int Columns
        {
            get { return this.columns; }
        }

        internal override double this[int i, int j]
        {
            get { return this.coeffs[i, j]; }
            set { this.coeffs[i, j] = value; }
        }

        internal override Matrix Clone()
        {
            return new SimpleMatrix(this.coeffs);
        }

        #endregion
    }
}
