using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using DLR = System.Linq.Expressions;
using Microsoft.Scripting.Runtime;
using AplusCore.Compiler;
using AplusCore.Types;

namespace AplusCore.Runtime
{
    public class Aplus
    {
        #region Variables

        private ExpandoObject globals;
        private Scope dlrglobals;

        private SystemVariables sysvars;
        private MemoryMappedFileManager mmfmanager;
        private DependencyManager dependencies;

        #endregion

        #region Properties

        public MemoryMappedFileManager MemoryMappedFileManager
        {
            get { return this.mmfmanager; }
        }

        public SystemVariables SystemVariables
        {
            get { return this.sysvars; }
        }

        public DependencyManager DependencyManager
        {
            get { return this.dependencies; }
        }

        public LexerMode LexerMode
        {
            get { return ConvertToLexerMode(this.sysvars["mode"].asString); }
            set { SwitchLexerMode(value.ToString()); }
        }

        public string CurrentContext
        {
            get { return this.sysvars["cx"].asString; }
            set { this.sysvars["cx"] = ASymbol.Create(value); }
        }

        #endregion

        #region Constructor

        public Aplus(Scope dlrglobals, LexerMode parsemode)
        {
            this.sysvars = new SystemVariables();
            this.dependencies = new DependencyManager();

            this.dlrglobals = dlrglobals;
            this.globals = new ExpandoObject();

            this.sysvars["mode"] = ASymbol.Create(parsemode.ToString().ToLower());

            this.mmfmanager = new MemoryMappedFileManager();
        }

        #endregion

        #region Methods

        public void SwitchLexerMode(string mode)
        {
            this.sysvars["mode"] = ASymbol.Create(ConvertToLexerMode(mode).ToString().ToLower());
        }

        #endregion

        #region Static Methods

        internal static LexerMode ConvertToLexerMode(string mode)
        {
            if (mode.ToLower() == "apl")
            {
                return Compiler.LexerMode.APL;
            }

            return Compiler.LexerMode.ASCII;
        }

        #endregion

        #region Binders
        // TODO: add binders


        private Dictionary<string, Binder.SetMemberBinder> _setMemberBinders =
            new Dictionary<string, Binder.SetMemberBinder>();
        internal Binder.SetMemberBinder SetMemberBinder(string name)
        {
            lock (this._setMemberBinders)
            {
                if (this._setMemberBinders.ContainsKey(name))
                {
                    return this._setMemberBinders[name];
                }

                Binder.SetMemberBinder binder = new Binder.SetMemberBinder(name);
                this._setMemberBinders[name] = binder;
                return binder;
            }
        }

        private Dictionary<string, Binder.GetMemberBinder> _getMemberBinders =
            new Dictionary<string, Binder.GetMemberBinder>();
        internal Binder.GetMemberBinder GetMemberBinder(string name)
        {
            lock (this._getMemberBinders)
            {
                if (this._getMemberBinders.ContainsKey(name))
                {
                    return this._getMemberBinders[name];
                }

                Binder.GetMemberBinder binder = new Binder.GetMemberBinder(name);
                this._getMemberBinders[name] = binder;
                return binder;
            }
        }

        private Dictionary<Type, Binder.ConvertBinder> _convertBinders =
            new Dictionary<Type, Binder.ConvertBinder>();
        internal Binder.ConvertBinder ConvertBinder(Type type)
        {
            lock (this._convertBinders)
            {
                if (this._convertBinders.ContainsKey(type))
                {
                    return this._convertBinders[type];
                }

                Binder.ConvertBinder binder = new Binder.ConvertBinder(type);
                this._convertBinders[type] = binder;
                return binder;
            }
        }

        private Dictionary<CallInfo, Binder.InvokeBinder> _invokeBinders =
            new Dictionary<CallInfo, Binder.InvokeBinder>();
        internal Binder.InvokeBinder InvokeBinder(CallInfo callInfo)
        {
            lock (this._invokeBinders)
            {
                if (this._invokeBinders.ContainsKey(callInfo))
                {
                    return this._invokeBinders[callInfo];
                }

                Binder.InvokeBinder binder = new Binder.InvokeBinder(callInfo);
                this._invokeBinders[callInfo] = binder;
                return binder;
            }
        }

        private Dictionary<CallInfo, Binder.GetIndexBinder> _getIndexBinders =
            new Dictionary<CallInfo, Binder.GetIndexBinder>();
        internal Binder.GetIndexBinder GetIndexBinder(CallInfo callInfo)
        {
            lock (this._getIndexBinders)
            {
                if (this._getIndexBinders.ContainsKey(callInfo))
                {
                    return this._getIndexBinders[callInfo];
                }

                Binder.GetIndexBinder binder = new Binder.GetIndexBinder(callInfo);
                this._getIndexBinders[callInfo] = binder;
                return binder;
            }
        }

        private Dictionary<CallInfo, Binder.SetIndexBinder> _setIndexBinders =
            new Dictionary<CallInfo, Binder.SetIndexBinder>();
        internal Binder.SetIndexBinder SetIndexBinder(CallInfo callInfo)
        {
            lock (this._setIndexBinders)
            {
                if (this._setIndexBinders.ContainsKey(callInfo))
                {
                    return this._setIndexBinders[callInfo];
                }

                Binder.SetIndexBinder binder = new Binder.SetIndexBinder(callInfo);
                this._setIndexBinders[callInfo] = binder;
                return binder;
            }
        }

        #endregion
    }
}
