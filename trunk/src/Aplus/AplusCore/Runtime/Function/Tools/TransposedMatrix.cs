namespace AplusCore.Runtime.Function.Tools
{
    internal class TransposedMatrix : Matrix
    {
        #region Variables

        private Matrix matrix;

        #endregion

        #region Constructors

        internal TransposedMatrix(Matrix matrix)
        {
            this.matrix = matrix;
        }

        #endregion

        #region Matrix overrides

        internal override int Rows
        {
            get { return matrix.Columns; }
        }

        internal override int Columns
        {
            get { return matrix.Rows; }
        }

        internal override double this[int i, int j]
        {
            get { return this.matrix[j, i]; }
            set { this.matrix[j, i] = value; }
        }

        internal override Matrix Clone()
        {
            return new TransposedMatrix(this);
        }

        #endregion
    }
}
