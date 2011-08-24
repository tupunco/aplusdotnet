using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    public class AipcAttribute
    {
        #region Variables

        private bool noDelay;
        private bool writePause;
        private bool readPause;
        private int readPriority;
        private int writePriority;
        private int readBufsize;
        private int writeBufSize;
        private bool retry;
        private AType clientData;
        private bool debug;
        private bool burstMode;

        ///<summary>
        /// The handle number of the listener, which created the connection.
        ///</summary>
        private int listener;

        private AipcConnection connection;

        #endregion

        #region Properties

        public bool Retry { get { return this.retry; } }

        public bool WritePause
        {
            get { return writePause; }
            set { this.writePause = value; }
        }

        public int Listener
        {
            get { return this.listener; }
            set { this.listener = value; }
        }

        public bool ReadPause
        {
            get { return this.readPause; }
            set { this.readPause = value; }
        }

        public bool NoDelay
        {
            get { return this.noDelay; }
            set { this.noDelay = value; }
        }

        public bool BurstMode { get { return this.burstMode; } }
        public int ReadBufsize { get { return this.readBufsize; } }
        public int WriteBufsize { get { return this.writeBufSize; } }

        #endregion

        #region Constructors

        private AipcAttribute(AipcConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Creates a new <see cref="AipcAttrbiute"/> with default values.
        /// </summary>
        /// <param name="connection">Connection to set on the new <see cref="AipcAttribute"/>.</param>
        /// <returns>Returns the new <see cref="AipcAttriute"/>  with default values.</returns>
        public static AipcAttribute Create(AipcConnection connection)
        {
            AipcAttribute attributes = new AipcAttribute(connection)
            {
                noDelay = false,
                writePause = false,
                readPause = false,
                readPriority = 0,
                writePriority = 0,
                retry = false,
                clientData = Utils.ANull(),
                debug = false,
                burstMode = false,
                listener = 0,
                readBufsize = 32768,
                writeBufSize = 32768
            };

            return attributes;
        }

        /// <summary>
        /// Create a new <see cref="AipcAttribute"/> from the existing one.
        /// </summary>
        /// <param name="connection">Connection to set on the new <see cref="AipcAttribute"/>.</param>
        /// <returns>Returns the new <see cref="AipcAttriute"/>.</returns>
        public AipcAttribute CreateNew(AipcConnection connection)
        {
            AipcAttribute attributes = new AipcAttribute(connection)
            {
                noDelay = this.noDelay,
                writePause = this.writePause,
                readPause = this.readPause,
                readPriority = this.readPriority,
                writePriority = this.writePriority,
                retry = this.retry,
                clientData = this.clientData,
                debug = this.debug,
                burstMode = this.burstMode,
                listener = this.listener,
                readBufsize = this.readBufsize,
                writeBufSize = this.writeBufSize
            };

            return attributes;
        }

        #endregion

        #region Methods

        public AType Attributes()
        {
            return AArray.Create(
                ATypes.ABox,
                ABox.Create(
                    AArray.Create(ATypes.ASymbol,
                        ASymbol.Create("noDelay"), ASymbol.Create("readPause"), ASymbol.Create("writePause"),
                        ASymbol.Create("readPriority"), ASymbol.Create("writePriority"), ASymbol.Create("readBufsize"),
                        ASymbol.Create("writeBufsize"), ASymbol.Create("retry"), ASymbol.Create("clientData"),
                        ASymbol.Create("debug"), ASymbol.Create("fd"), ASymbol.Create("port"),
                        ASymbol.Create("writeStatus"), ASymbol.Create("readStatus"), ASymbol.Create("listener"),
                        ASymbol.Create("burstMode")
                                )
                        ),
                ABox.Create(
                     AArray.Create(ATypes.ASymbol,
                         ASymbol.Create("noDelay"), ASymbol.Create("readPause"), ASymbol.Create("writePause"),
                         ASymbol.Create("readPriority"), ASymbol.Create("writePriority"), ASymbol.Create("readBufsize"),
                         ASymbol.Create("writeBufsize"), ASymbol.Create("retry"), ASymbol.Create("ClientData"),
                         ASymbol.Create("debug"), ASymbol.Create("burstMode")
                             )
                         )
            );
        }

        public AType GetAttribute(AType attribute)
        {
            AType result;

            switch (attribute.asString)
            {
                case "noDelay":
                    result = AInteger.Create(noDelay ? 1 : 0);
                    break;
                case "readPause":
                    result = AInteger.Create(readPause ? 1 : 0);
                    break;
                case "writePause":
                    result = AInteger.Create(writePause ? 1 : 0);
                    break;
                case "readPriority":
                    result = AInteger.Create(readPriority);
                    break;
                case "writePriority":
                    result = AInteger.Create(writePriority);
                    break;
                case "readBufsize":
                    result = AInteger.Create(readBufsize);
                    break;
                case "writeBufsize":
                    result = AInteger.Create(writeBufSize);
                    break;
                case "retry":
                    result = AInteger.Create(retry ? 1 : 0);
                    break;
                case "clientData":
                    result = clientData;
                    break;
                case "debug":
                    result = AInteger.Create(debug ? 1 : 0);
                    break;
                case "fd":
                    result = AInteger.Create(connection.isOpen ? connection.Socket.Handle.ToInt32() : -1);
                    break;
                case "port":
                    result = AInteger.Create(connection.ConnectionAttributes.Port);
                    break;
                case "writeStatus":
                    result = this.connection.AipcService.WriteQueueStatus(connection.ConnectionAttributes.HandleNumber);
                    break;
                case "readStatus":
                    result = this.connection.AipcService.ReadQueueStatus(connection.ConnectionAttributes.HandleNumber);
                    break;
                case "listener":
                    result = (listener == 0) ? Utils.ANull() : AInteger.Create(listener);
                    break;
                case "burstMode":
                    result = AInteger.Create(debug ? 1 : 0);
                    break;
                default:
                    result = Utils.ANull();
                    break;
            }

            return result;
        }

        public AType SetAttribute(AType attribute, AType value)
        {
            AType toSet;
            bool tryFirstScalar = value.TryFirstScalar(out toSet, true);


            if (attribute.asString != "clientData")
            {
                if (value.Type != ATypes.AInteger || !tryFirstScalar)
                {
                    return AInteger.Create(-1);
                }
            }

            //if (value.Type != ATypes.AInteger && attribute.asString != "clientData")
            //{
            //    return AInteger.Create(-1);
            //}

            //if (!value.TryFirstScalar(out toSet, true) && attribute.asString != "clientData")
            //{
            //    return AInteger.Create(-1);
            //}

            AType result = AInteger.Create(0);

            switch (attribute.asString)
            {
                case "noDelay":
                    noDelay = (toSet.asInteger == 1);
                    connection.SetSocket();
                    break;
                case "readPause":
                    readPause = (toSet.asInteger == 1);
                    break;
                case "writePause":
                    writePause = (toSet.asInteger == 1);
                    break;
                case "readPriority":
                    readPriority = toSet.asInteger;
                    break;
                case "writePriority":
                    writePriority = toSet.asInteger;
                    break;
                case "readBufsize":
                    readBufsize = toSet.asInteger;
                    connection.SetSocket();
                    break;
                case "writeBufsize":
                    writeBufSize = toSet.asInteger;
                    connection.SetSocket();
                    break;
                case "retry":
                    retry = (toSet.asInteger == 1);
                    break;
                case "clientData":
                    clientData = value;
                    break;
                case "debug":
                    debug = (toSet.asInteger == 1);
                    break;
                case "burstMode":
                    burstMode = (toSet.asInteger == 1);
                    break;
                default:
                    result = AInteger.Create(-1);
                    break;
            }

            return result;
        }

        public AType GetWriteQueue()
        {
            return this.connection.AipcService.WriteQueueStatus(connection.ConnectionAttributes.HandleNumber);
        }

        #endregion
    }
}
