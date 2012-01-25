using AplusCore.Types;

using DLR = System.Linq.Expressions;

namespace AplusCore.Compiler
{
    internal class CallbackInfoStorage
    {
        #region Variables

        private DLR.ParameterExpression qualifiedName;
        private DLR.ParameterExpression index;
        private DLR.ParameterExpression path;
        private DLR.ParameterExpression nonPresetValue;

        #endregion

        #region Properties

        public DLR.ParameterExpression QualifiedName
        {
            get { return this.qualifiedName; }
            set { this.qualifiedName = value; }
        }

        public DLR.ParameterExpression Index
        {
            get { return this.index; }
            set { this.index = value; }
        }

        public DLR.ParameterExpression Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        public DLR.ParameterExpression NonPresetValue
        {
            get { return this.nonPresetValue; }
            set { this.nonPresetValue = value; }
        }

        #endregion

        #region Constructor

        public CallbackInfoStorage()
        {
            this.qualifiedName = DLR.Expression.Parameter(typeof(string), "__QUALIFIEDNAME__");
            this.index = DLR.Expression.Parameter(typeof(AType), "__CALLBACKINDEX__");
            this.path = DLR.Expression.Parameter(typeof(AType), "__PATH__");
            this.nonPresetValue = DLR.Expression.Parameter(typeof(AType), "__NONPRESETVALUE__");
        }

        #endregion
    }
}
