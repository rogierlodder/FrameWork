using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FWO
{
    public enum FWOStates { ConnectingToServer, StartFWOCLient, DownloadingAllFWO, StartingDSCClient, DownloadingDSC, Running, Disconnected }
    public class FWOClientStarter : FWOStarterBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //clients
        public static GTLClientServerComm ServerComm { get; private set; }
        private static GTLClientFWO FWOClient;
        private static GTLClientDescriptions DSCClient;
        private static Timer CycleTimer;
        private const int Cycletime = 20;

        public static GTLClientNotifications NotifClient;
        public static bool ClientHasStarted  { get; private set; } = false;
        public static FWOStates state { get; private set; }

        public static void StartClients(int basePortNr, string ServerAddress)
        {
            FWOBase.RunsOnServer = false;

            if (basePortNr == 0) basePortNr = DefBasePort;
            SetPorts(basePortNr);

            //start the Clients
            ServerComm = new GTLClientServerComm(ServerAddress, ServerCommServicePort, "ServerCommService");
            ServerComm.Start();

            FWOClient = new GTLClientFWO(ServerAddress, FrameWorkObjectServiceport, "FrameWorkObjectService");
            DSCClient = new GTLClientDescriptions(ServerAddress, DescriptionServicePort, "DescriptionService");

            CycleTimer = new Timer(Run);
            CycleTimer.Change(0, Cycletime);
        }

        private static void Run(object o)
        {
            //run the state machines of all clients
            for (int i=0; i< GTLClientBase.AllClients.Count; i++)
            {
                GTLClientBase.AllClients[i].Run();
            }
            //monitor the server connection
            ServerComm.MonitorConnection();

            //state machine
            if (state == FWOStates.ConnectingToServer)
            {
                if (ServerComm.ServerConnected)
                {
                    state = FWOStates.DownloadingAllFWO;
                    FWOClient.Start();
                }
            }

            if (state == FWOStates.DownloadingAllFWO)
            {
                if(FWOClient.FWODownloadDone == true)
                {
                    state = FWOStates.DownloadingDSC;
                    DSCClient.Start();
                }
            }

            if (state == FWOStates.DownloadingDSC)
            {
                if (DSCClient.DSCsReceived == true)
                {
                    state = FWOStates.Running;
                    log.Info("All FWO downloads done");
                }
            }

            if (state == FWOStates.Running)
            {
                if (ServerComm.ServerConnected == false) state = FWOStates.Disconnected;
            }

            if (state == FWOStates.Disconnected)
            {

            }
        }

        public static Task WaitForClientConnectTask()
        {
            return Task.Run(() => 
            {
                while (state != FWOStates.Running)
                {
                    Thread.Sleep(100);
                }
            });
        }


        public static Task WaitForClientDisconnectTask()
        {
            return Task.Run(() =>
            {
                while (state != FWOStates.Disconnected)
                {
                    Thread.Sleep(100);
                }
            });
        }
    }
}
