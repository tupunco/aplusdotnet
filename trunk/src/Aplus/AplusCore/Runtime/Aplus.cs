using System;
using System.Collections.Generic;

using Microsoft.Scripting.Runtime;

using AplusCore.Compiler;
using AplusCore.Runtime.Callback;
using AplusCore.Runtime.Context;
using AplusCore.Runtime.Function.ADAP;
using AplusCore.Types;

using DYN = System.Dynamic;

namespace AplusCore.Runtime
{
    public class Aplus
    {
        #region Variables

        private DYN.ExpandoObject globals;
        private Scope dlrglobals;

        private DYN.IDynamicMetaObjectProvider context;
        private DYN.ExpandoObject functionscope;

        private SystemVariables sysvars;
        private MemoryMappedFileManager mmfmanager;
        private DependencyManager dependencies;
        private CallbackManager callbackManager;

        private Dictionary<string, AType> systemFunctions;

        private ContextLoader contextLoader;
        private bool isAutoLoaded;
        private string[] autoloadContexts;

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

        public CallbackManager CallbackManager
        {
            get { return this.callbackManager; }
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

        public Dictionary<string, AType> SystemFunctions
        {
            get { return this.systemFunctions; }
        }

        internal ContextLoader ContextLoader
        {
            get { return this.contextLoader; }
        }

        public DYN.IDynamicMetaObjectProvider Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        public DYN.ExpandoObject FunctionScope
        {
            get { return this.functionscope; }
            set { this.functionscope = value; }
        }


        #endregion

        #region Constructor

        public Aplus(Scope dlrglobals, LexerMode parsemode)
        {
            this.sysvars = new SystemVariables();
            this.dependencies = new DependencyManager();
            this.callbackManager = new CallbackManager();

            this.dlrglobals = dlrglobals;
            this.globals = new DYN.ExpandoObject();

            this.sysvars["mode"] = ASymbol.Create(parsemode.ToString().ToLower());

            this.mmfmanager = new MemoryMappedFileManager();

            this.systemFunctions = Function.SystemFunction.DiscoverSystemFunctions();

            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("APATH", EnvironmentVariableTarget.User)))
            {
                string paths = String.Join(";", ".", "./Runtime/Context/");
                Environment.SetEnvironmentVariable("APATH", paths, EnvironmentVariableTarget.User);
            }

            // TODO: Move this to app.config?
            this.autoloadContexts = new string[] { "sys" };
            this.contextLoader = new ContextLoader(this);
        }

        #endregion

        #region Methods

        public void SwitchLexerMode(string mode)
        {
            this.sysvars["mode"] = ASymbol.Create(ConvertToLexerMode(mode).ToString().ToLower());
        }

        /// <summary>
        /// Loads default contexts into the provided <see cref="Scope"/>Scope.
        /// </summary>
        /// <param name="scope">Scope where the contexts will be loaded.</param>
        /// <remarks>
        /// Warning! This method will overwrite the context if there is already one.
        /// </remarks>
        /// <returns>On the first call it will return true, otherwise false.</returns>
        public bool AutoloadContext(Scope scope)
        {
            if (this.isAutoLoaded)
            {
                return false;
            }

            foreach (string contextName in this.autoloadContexts)
            {
                IDictionary<string, object> context = new DYN.ExpandoObject();
                IDictionary<string, AType> contextElements = this.ContextLoader.FindContextElements(contextName);

                foreach (KeyValuePair<string, AType> item in contextElements)
                {
                    context[item.Key] = item.Value;
                }

                if (context.Count > 0)
                {
                    // TODO: ? is it ok to replace the whole context or replace only individual elements?
                    scope.Storage[contextName] = context;
                }
            }

            this.isAutoLoaded = true;

            return true;
        }
        
        #endregion

        #region Service handling

        private Dictionary<Type, object> serviceMapping = new Dictionary<Type, object>();

        /// <summary>
        /// Get a registered service from the Aplus runtime.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns>
        /// Returns the registered service for the given type if there is such,
        /// or null if the service was not found.
        /// </returns>
        public TService GetService<TService>() where TService : class
        {
            object service;

            if (!this.serviceMapping.TryGetValue(typeof(TService), out service))
            {
                return null;
            }

            return service as TService;
        }

        /// <summary>
        /// Register a service for the Aplus runtime.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="service">Service object to register.</param>
        public void SetService<TService>(TService service) where TService : class
        {
            this.serviceMapping[typeof(TService)] = service;
        }

        #endregion

        #region Static Methods

        internal static LexerMode ConvertToLexerMode(string mode)
        {
            if (mode.ToLower() == "apl")
            {
                return Compiler.LexerMode.APL;
            }
            else if (mode.ToLower() == "uni")
            {
                return Compiler.LexerMode.UNI;
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

        private Dictionary<DYN.CallInfo, Binder.InvokeBinder> _invokeBinders =
            new Dictionary<DYN.CallInfo, Binder.InvokeBinder>();
        internal Binder.InvokeBinder InvokeBinder(DYN.CallInfo callInfo)
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

        private Dictionary<DYN.CallInfo, Binder.GetIndexBinder> _getIndexBinders =
            new Dictionary<DYN.CallInfo, Binder.GetIndexBinder>();
        internal Binder.GetIndexBinder GetIndexBinder(DYN.CallInfo callInfo)
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

        private Dictionary<DYN.CallInfo, Binder.SetIndexBinder> _setIndexBinders =
            new Dictionary<DYN.CallInfo, Binder.SetIndexBinder>();
        internal Binder.SetIndexBinder SetIndexBinder(DYN.CallInfo callInfo)
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
