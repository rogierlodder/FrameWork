using System.Threading.Tasks;
using System.Threading;
using System;

namespace FWO
{
    public class GTLClientServerComm : GTLClient<CClientInfo, CServerInfo>
    {
        private static log4net.ILog log;
        public const int ServerConnectionTimeoutCycles = 8;

        public bool ServerConnected { get; private set; } = false;
        public bool ConnectionRejected { get; protected set; } = false;

        public long ServerCommCounter = 0;
        private int ServerDisconnectCounter = -1;

        public GTLClientServerComm(string IPAddress, int portNr, string serviceName) : base(IPAddress, portNr, serviceName)
        {
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType + "_"  + serviceName);

            if (ClientID != 0)
            {
                throw new Exception("A GTLClientServerComm has already been created. Only one FWO client is allowed per process");
            }
            Request = new CClientInfo();
            Reply = new CServerInfo();

            //generate "random" ClientID
            ClientID = (uint)DateTime.Now.ToBinary();
            Request.ClientID = ClientID;
        }

        protected override void CreateRequest()
        {
            Request.Counter++;
        }

        protected override void ProcessReply ()
        {
            WaitingForReply = false;
            if (Reply.ConnectionAccepted == false)
            {
                ConnectionRejected = true;
            }
        }

        public void MonitorConnection()
        {
            if (ServerCommCounter != Reply.SessionCounter)
            {
                ServerCommCounter = Reply.SessionCounter;
                ServerDisconnectCounter = 0;
                if (ServerConnected == false)
                {
                    ServerConnected = true;
                    log.Debug("Server connected");
                }
            }
            else
            {
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
