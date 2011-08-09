using System.Net;

namespace AplusCore.Runtime.Function.ADAP
{
    class StateObject
    {
        public IPEndPoint endpoint;

        public StateObject(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }
    }
}
