using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RGF
{
    public enum RGOStates { ConnectingToServer, StartFWOCLient, DownloadingAllFWO, StartingDSCClient, DownloadingDSC, Running, Disconnected }

    public class RGOClientManager : RGOStarterBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //clients
        public static RGOClientServerComm ServerComm { get; private set; }
        private static RGOClientFWO RGOClientDownloader;
        private static RGOClientDescriptions DSCClient;
        private static Timer CycleTimer;
        private const int Cycletime = 200;

        public static RGOClientNotifications NotifClient;
        public static bool ClientHasStarted  { get; private set; } = false;
        public static RGOStates state { get; private set; }

        public static void StartClients(int basePortNr, string ServerAddress)
        {
            RGOBase.RunsOnServer = false;

            if (basePortNr == 0) basePortNr = DefBasePort;
            SetPorts(basePortNr);

            //start the Clients
            ServerComm = new RGOClientServerComm(ServerAddress, ServerCommServicePort, "ServerCommService");
            ServerComm.Start();

            RGOClientDownloader = new RGOClientFWO(ServerAddress, FrameWorkObjectServiceport, "FrameWorkObjectService");
            DSCClient = new RGOClientDescriptions(ServerAddress, DescriptionServicePort, "DescriptionService");

            CycleTimer = new Timer(Run);
            CycleTimer.Change(0, Cycletime);
        }

        private static void Run(object o)
        {
            //run the state machines of all clients
            for (int i=0; i< RGOClientBase.AllClients.Count; i++)
            {
                RGOClientBase.AllClients[i].Run();
            }
            //monitor the server connection
            ServerComm.MonitorConnection();

            //state machine
            if (state == RGOStates.ConnectingToServer)
            {
                if (ServerComm.ServerConnected)
                {
                    state = RGOStates.DownloadingAllFWO;
                    RGOClientDownloader.Start();
                }
            }

            if (state == RGOStates.DownloadingAllFWO)
            {
                if(RGOClientDownloader.RGODownloadDone == true)
                {
                    state = RGOStates.DownloadingDSC;
                    DSCClient.Start();
                }
            }

            if (state == RGOStates.DownloadingDSC)
            {
                if (DSCClient.DSCsReceived == true)
                {
                    state = RGOStates.Running;
                    log.Info("All RGO downloads done");
                }
            }

            if (state == RGOStates.Running)
            {
                if (ServerComm.ServerConnected == false) state = RGOStates.Disconnected;
            }

            if (state == RGOStates.Disconnected)
            {

            }
        }

        public static Task WaitForClientConnectTask()
        {
            return Task.Run(() => 
            {
                while (state != RGOStates.Running)
                {
                    Thread.Sleep(100);
                }
            });
        }


        public static Task WaitForClientDisconnectTask()
        {
            return Task.Run(() =>
            {
                while (state != RGOStates.Disconnected)
                {
                    Thread.Sleep(100);
                }
            });
        }
    }
}
