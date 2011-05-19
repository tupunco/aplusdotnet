using DYN = System.Dynamic;

namespace AplusCore.Runtime
{
    /// <summary>
    /// Store Runtime and Context inforamtion,
    /// which are used during the execution of an APlusScriptCode.
    /// </summary>
    public class AplusEnvironment
    {
        #region Varaibles

        private Aplus runtime;
        private DYN.IDynamicMetaObjectProvider context;
        private DYN.ExpandoObject functionscope;

        #endregion

        #region Properties

        public Aplus Runtime { get { return this.runtime; } }
        public DYN.IDynamicMetaObjectProvider Context { get { return this.context; } }
        public DYN.ExpandoObject FunctionScope
        {
            get { return this.functionscope; }
            set { this.functionscope = value; }
        }


        #endregion

        #region Constructor

        public AplusEnvironment(Aplus runtime, DYN.IDynamicMetaObjectProvider context, DYN.ExpandoObject functionscope = null)
        {
            this.runtime = runtime;
            this.context = context;
            this.functionscope = functionscope;
        }

        #endregion
    }
}
