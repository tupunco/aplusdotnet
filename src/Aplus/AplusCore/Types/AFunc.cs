using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AplusCore.Types
{
    public class AFunc : AValue
    {
        #region Variables

        /// <summary>
        /// The user defined method
        /// </summary>
        private object method;

        /// <summary>
        /// valence of user defined method
        /// </summary>
        private int valence;

        /// <summary>
        /// name of the user defined method (debug)
        /// </summary>
        private string name;

        /// <summary>
        /// the generated DLR expression tree in readable format
        /// </summary>
        private string codestring;

        /// <summary>
        /// true if the containing method represens a built-in function
        /// </summary>
        private bool builtin;

        /// <summary>
        /// True if function is derived.
        /// </summary>
        private bool derived;

        #endregion

        #region Properties

        public object Method { get { return this.method; } }

        public int Valence { get { return this.valence; } }
        public string Name { get { return this.name; } }
        public bool IsBuiltin { get { return this.builtin; } }
        public bool IsDerived { get { return this.derived; } }

        #endregion

        #region Constructor

        private AFunc(string name, object method, int valence, string codestring)
        {
            this.name = name;
            this.method = method;
            this.valence = valence;
            this.codestring = codestring;

            this.type = ATypes.AFunc;
        }

        public static AType Create(string name, object method, int valence, string codestring)
        {
            return new AReference(new AFunc(name, method, valence, codestring));
        }

        #endregion

        #region Methods

        public static AType CreateBuiltIn(string name, object method, string codestring, bool derived)
        {
            AFunc function = new AFunc(name, method, 3, codestring);
            function.builtin = true;
            function.derived = derived;
            return new AReference(function);
        }

        #endregion

        #region Overrides

        public override AType Clone(bool isMemmoryMapped = false)
        {
            return this;
        }

        public override string ToString()
        {
            return this.codestring;
        }

        public override bool Equals(object obj)
        {
            if (obj is AFunc)
            {
                AFunc other = (AFunc)obj;

                if (IsBuiltin)
                {
                    return this.valence == other.valence && this.name == other.name;
                }

                return other.method == this.method && this.valence == other.valence && this.name == other.name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.method.GetHashCode() ^ this.name.GetHashCode() ^ this.valence.GetHashCode();
        }

        #endregion
    }
}
