using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RLStateMachine;

namespace RGF
{
    public enum RGOStates { ConnectingToServer, StartFWOCLient, DownloadingAllFWO, StartingDSCClient, DownloadingDSC, Running, Disconnected }

    public class RGOClientManager : RGOStarterBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //clients
        public RGOClientServerComm ServerComm { get; private set; }
        public RGOClientNotifications NotifClient;
        public RGOClientFWO RGOClientDownloader;
        public RGOClientDescriptions DSCClient;
        public bool ClientHasStarted  { get; private set; } = false;
        public RGOStates state { get; private set; } = new RGOStates();

        private RLSM SM = new RLSM("RGOClientManager");
        private Timer CycleTimer;

        public void StartClients(int basePortNr, string ServerAddress, int cycleTime, bool runFromTimer)
        {
            state = RGOStates.ConnectingToServer;
            RGOBase.RunsOnServer = false;

            if (basePortNr == 0) basePortNr = DefBasePort;
            SetPorts(basePortNr);

            //start the Clients
            ServerComm = new RGOClientServerComm(ServerAddress, ServerCommServicePort, "ServerCommService");
            ServerComm.Running = true;

            RGOClientDownloader = new RGOClientFWO(ServerAddress, FrameWorkObjectServiceport, "FrameWorkObjectService");
            DSCClient = new RGOClientDescriptions(ServerAddress, DescriptionServicePort, "DescriptionService");

            CycleTimer = new Timer(Run);
            if (runFromTimer) CycleTimer.Change(0, cycleTime);
        }

        public RGOClientManager()
        {
            SM.FirstAction = () =>
            {
                //run the state machines of all clients
                for (int i = 0; i < RGOClientBase.AllClients.Count; i++)
                {
                    RGOClientBase.AllClients[i].Run();
                }
            };

            SM.AddState(RGOStates.ConnectingToServer, new List<Transition>
            {
                new Transition("ServerConnected", () => ServerComm.ServerConnected, () => RGOClientDownloader.Running = true, RGOStates.DownloadingAllFWO),
                new Transition("ConnectionRejected", () => ServerComm.ConnectionRejected, () => log.Error("The connection was rejected by the server."), RGOStates.Disconnected)
            }, null, StateType.entry);

            SM.AddState(RGOStates.DownloadingAllFWO, new List<Transition>
            {
                new Transition("DownloadDone", () => RGOClientDownloader.RGODownloadDone == true, () => DSCClient.Running = true,  RGOStates.DownloadingDSC),
            }, null, StateType.transition);

            SM.AddState(RGOStates.DownloadingDSC, new List<Transition>
            {
                new Transition("DescriptionsReceived", () => DSCClient.DSCsReceived == true, () => log.Info("All RGO downloads done"), RGOStates.Running),
            }, null, StateType.transition);

            SM.AddState(RGOStates.Running, new List<Transition>
            {
                new Transition("ServerCommDisconnected", () => ServerComm.ServerConnected == false, null, RGOStates.Disconnected),
            }, null, StateType.idle);

            SM.AddState(RGOStates.Disconnected, new List<Transition>
            {

            }, () =>
            {
                foreach (var C in RGOClientBase.AllClients) C.Disconnect();
            }, StateType.end);

            SM.Finalize();
        }


        private void Run(object o)
        {
            SM.Run();
        }

        public Task WaitForClientConnectTask()
        {
            return Task.Run(() => 
            {
                while ((RGOStates)SM.CurrentState != RGOStates.Running)
                {
                    Thread.Sleep(100);
                }
            });
        }


        public Task WaitForClientDisconnectTask()
        {
            return Task.Run(() =>
            {
                while ((RGOStates)SM.CurrentState != RGOStates.Disconnected)
                {
                    Thread.Sleep(100);
                }
            });
        }
    }
}
