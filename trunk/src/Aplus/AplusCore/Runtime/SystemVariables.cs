using System.Collections.Generic;

using AplusCore.Types;

namespace AplusCore.Runtime
{
    public class SystemVariables
    {
        #region Variables

        /// <summary>
        /// This stores the System Variables and the values for it
        /// </summary>
        private Dictionary<string, AType> store;

        #endregion

        #region Properties

        public AType this[string key]
        {
            get { return this.store[key]; }
            set { this.store[key] = value; }
        }

        #endregion

        #region Constructor

        public SystemVariables()
        {
            this.store = new Dictionary<string, AType>();
            SetVariables();
        }

        #endregion

        #region Default System Variable values

        private void SetVariables()
        {
            this.store["CCID"] = ASymbol.Create(".NET");
            this.store["cx"] = ASymbol.Create(".");
            this.store["mode"] = ASymbol.Create("apl");
            this.store["rl"] = AInteger.Create(1);
            this.store["language"] = ASymbol.Create("aplus");
            this.store["majorRelease"] = AInteger.Create(0);
            this.store["minorRelease"] = AInteger.Create(2);
            this.store["pp"] = AInteger.Create(10);
            this.store["stop"] = AInteger.Create(0);
        }

        #endregion
    }
}
