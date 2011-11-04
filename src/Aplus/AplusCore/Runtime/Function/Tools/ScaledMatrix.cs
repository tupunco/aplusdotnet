namespace AplusCore.Runtime.Function.Tools
{
    internal class ScaledMatrix : Matrix
    {
        #region Variables

        private double scale;
        private Matrix matrix;

        #endregion

        #region Constructors

        internal ScaledMatrix(Matrix matrix, double scale)
        {
            this.scale = scale;
            this.matrix = matrix;
        }

        #endregion

        #region Matrix Members

        internal override int Rows
        {
            get { return this.matrix.Rows; }
        }

        internal override int Columns
        {
            get { return this.matrix.Columns; }
        }

        internal override double this[int i, int j]
        {
            get { return this.matrix[i, j] * scale; }
            set { this.matrix[i, j] = value; }
        }

        internal override Matrix Clone()
        {
            return new ScaledMatrix(this, this.scale);
        }

        #endregion
    }
}
