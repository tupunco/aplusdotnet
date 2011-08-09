using AplusCore.Types;

namespace AplusCore.Runtime.Function.ADAP
{
    public class ConnectionAttribute
    {
        #region Constants

        public static readonly AType DEFAULT_PROTOCOL = ASymbol.Create("A");
        public static readonly AType DEFAULT_HOST = ASymbol.Create("0.0.0.0");
        public static readonly int DEFAULT_PORT = 0;

        #endregion

        #region Variables

        private bool zeroPort;
        private AType func;
        private AType name;
        private AType protocol;
        private AType host;
        private int handleNumber;
        private int port;

        /// <summary>
        /// Specifies if the connection is a listener socket.
        /// </summary>
        private bool listener = false;

        #endregion

        #region Constructors

        public ConnectionAttribute()
        {
            this.zeroPort = false;
            this.protocol = DEFAULT_PROTOCOL;
            this.host = DEFAULT_HOST;
            this.port = DEFAULT_PORT;
            this.listener = false;
        }

        public ConnectionAttribute(ConnectionAttribute attribute)
        {
            this.zeroPort = attribute.zeroPort;
            this.func = attribute.func;
            this.name = attribute.name;
            this.protocol = attribute.protocol;
            this.host = attribute.host;
            this.port = attribute.port;
            this.listener = attribute.listener;
            this.handleNumber = attribute.handleNumber;
        }

        #endregion

        #region Properties

        public AType Function
        {
            get { return this.func; }
            set { this.func = value; }
        }

        public AType Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public AType Protocol
        {
            get { return this.protocol; }
            set { this.protocol = value; }
        }

        public AType Host
        {
            get { return this.host; }
            set { this.host = value; }
        }

        public int HandleNumber
        {
            get { return this.handleNumber; }
            set { this.handleNumber = value; }
        }

        public int Port
        {
            get { return this.port; }
            set { this.port = value; }
        }

        public bool ZeroPort
        {
            get { return this.zeroPort; }
            set { this.zeroPort = value; }
        }

        public bool IsListener
        {
            get { return this.listener; }
            set { this.listener = value; }
        }

        #endregion
    }
}
