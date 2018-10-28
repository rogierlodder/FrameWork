using System.Threading.Tasks;
using System.Threading;
using System;

namespace RGF
{
    public class RGOClientServerComm : RGOClient<CClientInfo, ServerInfo>
    {
        private static log4net.ILog log;
        public const int ServerConnectionTimeoutCycles = 8;

        public bool ServerConnected { get; private set; } = false;
        public bool ConnectionRejected { get; protected set; } = false;

        public long ServerCommCounter = 0;
        private int ServerDisconnectCounter = -1;

        public RGOClientServerComm(string IPAddress, int portNr, string serviceName) : base(IPAddress, portNr, serviceName)
        {
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType + "_"  + serviceName);

            if (ClientID != 0)
            {
                throw new Exception("A RGOClientServerComm has already been created. Only one RGO client is allowed per process");
            }
            Request = new CClientInfo();
            Reply = new ServerInfo();

            //generate "random" ClientID
            ClientID = (uint)DateTime.Now.Ticks;
            Request.ClientID = ClientID;
        }

        protected override void CreateRequest()
        {
            Request.Counter++;
            ServerCommCounter = Request.Counter;
        }

        protected override void ProcessReply ()
        {
            //check every incoming packet that the server has not rejected the connection
            WaitingForReply = false;
            if (Reply.ConnectionAccepted == false)
            {
                ConnectionRejected = true;
                ServerConnected = false;
            }
            else
            {
                ServerConnected = true;
                ConnectionRejected = false;
            }

            //process the session counter
            if (ServerCommCounter  == Reply.SessionCounter)
            {
                //ServerCommCounter++;
                ServerDisconnectCounter = 0;
                if (ServerConnected == false)
                {
                    ServerConnected = true;
                    ConnectionRejected = false;
                    log.Debug("Server connected");
                }
            }
            else
            {
                log.Debug($"Unexpected session number received ({ Reply.SessionCounter})");
                ServerDisconnectCounter++;
                if (ServerDisconnectCounter > ServerConnectionTimeoutCycles && ServerConnected == true)
                {
                    ServerConnected = false;
                    ServerDisconnectCounter = 0;
                }
            }
        }
    }
}
