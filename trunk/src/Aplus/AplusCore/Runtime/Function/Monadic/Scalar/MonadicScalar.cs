using System;
using System.Collections.Generic;
using System.Reflection;

using AplusCore.Types;

namespace AplusCore.Runtime.Function.Monadic.Scalar
{
    abstract class MonadicScalar : AbstractMonadicFunction
    {
        #region Variables

        private byte currentType;
        private HashSet<byte> allowedTypes;

        #endregion

        #region Properties

        public virtual ATypes ReturnType
        {
            get { return ATypes.AInteger; }
        }

        #endregion

        #region Constructor

        public MonadicScalar()
        {
            this.allowedTypes = new HashSet<byte>();
            
            MethodInfo[] methods = this.GetType().GetMethods(
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public
            );

            foreach (MethodInfo method in methods)
            {
                if (!method.IsVirtual)
                {
                    continue;
                }

                ParameterInfo[] parameterInfo = method.GetParameters();

                this.allowedTypes.Add((byte)Utils.GetATypesFromType(parameterInfo[0].ParameterType));
                
            }
        }

        #endregion

        #region Entry Point from DLR

        public override AType Execute(AType argument, AplusEnvironment environment = null)
        {
            this.currentType = (byte)argument.Type;
            // Check if we have a rule for the specific input types
            if (!this.allowedTypes.Contains(this.currentType))
            {
                // Rule not found

                // reset to the general case
                this.currentType = (byte)ATypes.AType;

                // Check if we have default case
                if (!this.allowedTypes.Contains(this.currentType))
                {
                    // throw a type error
                    throw new Error.Type(this.TypeErrorText);
                }
            }
            return ExecuteRecursion(argument, environment);
        }

        #endregion

        #region Recursion

        private AType ExecuteRecursion(AType argument, AplusEnvironment environment)
        {
            if (argument.IsArray)
            {
                uint typeCounter = 0;
                AType currentItem;

                AType arg = argument;
                AType result = AArray.Create(this.ReturnType);

                for (int i = 0; i < arg.Length; i++)
                {
                    currentItem = ExecuteRecursion(arg[i], environment);
                    typeCounter += (uint)((currentItem.Type == ATypes.AFloat) ? 1 : 0);

                    result.AddWithNoUpdate(currentItem);
                }

                result.UpdateInfo();
                if ((typeCounter != result.Length) && (typeCounter != 0))
                {
                    result.ConvertToFloat();
                }
                return result;
            }
            else
            {
                AType result = null;
                switch (currentType)
                {
                    //case (byte)ATypes.ANull:
                    //    result = new ANull();
                    //    break;
                    case (byte)ATypes.AInteger:
                        result = ExecutePrimitive((AInteger)argument.Data, environment);
                        break;
                    case (byte)ATypes.AFloat:
                        result = ExecutePrimitive((AFloat)argument.Data, environment);
                        break;
                    case (byte)ATypes.AChar:
                        result = ExecutePrimitive((AChar)argument.Data, environment);
                        break;
                    case (byte)ATypes.ASymbol:
                        result = ExecutePrimitive((ASymbol)argument.Data, environment);
                        break;
                    case (byte)ATypes.ABox:
                        result = ExecutePrimitive((ABox)argument.Data, environment);
                        break;
                    case (byte)ATypes.AType:
                        result = ExecuteDefault((AType)argument.Data, environment);
                        break;
                }

                return result;
            }
        }

        #endregion

        #region Type specific executes
       
        public virtual AType ExecuteDefault(AType argument, AplusEnvironment environment = null)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        public virtual AType ExecutePrimitive(AInteger argument, AplusEnvironment environment = null)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        public virtual AType ExecutePrimitive(AFloat argument, AplusEnvironment environment = null)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        public virtual AType ExecutePrimitive(AChar argument, AplusEnvironment environment = null)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        public virtual AType ExecutePrimitive(ASymbol argument, AplusEnvironment environment = null)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        public virtual AType ExecutePrimitive(ABox argument, AplusEnvironment environment = null)
        {
            throw new NotImplementedException("Invalid use-case");
        }

        #endregion

    }
}
