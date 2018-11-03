using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RLStateMachine;

namespace RGF
{
    public enum RGOStates { ConnectingToServer, StartRGOCLient, DownloadingAllRGO, StartingDSCClient, DownloadingDSC, Running, Disconnecting, Disconnected, Stopped }

    public class RGOClientManager : RGOStarterBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int InstanceCounter = 0;
        //clients
        public static RGOClientServerComm ServerComm { get; private set; }
        public static RGOClientNotifications NotifClient;
        public static RGOClientRGO RGOClientDownloader;
        public static RGOClientDescriptions DSCClient;
        public static bool ClientHasStarted  { get; private set; } = false;
        public static RGOStates state { get; private set; } = new RGOStates();
        public static bool ServerCommError { get; private set; }
        public static bool Reconnect { get; set; } = true;

        private static RLSM SM = new RLSM("RGOClientManager");
        private static Timer CycleTimer;

        public static void StartClients(int basePortNr, string ServerAddress, int cycleTime, bool runFromTimer)
        {
            state = RGOStates.ConnectingToServer;
            RGOBase.RunsOnServer = false;

            if (basePortNr == 0) basePortNr = DefBasePort;
            SetPorts(basePortNr);

            //start the Clients
            ServerComm = new RGOClientServerComm(ServerAddress, ServerCommServicePort, "ServerCommService");
            ServerComm.Connect();
            ServerComm.ReportError = (p, q) =>
            {
                log.Error($": Client {p} reported an errror: {q}");
                ServerCommError = true;
            };

            RGOClientDownloader = new RGOClientRGO(ServerAddress, FrameWorkObjectServiceport, "FrameWorkObjectService");
            DSCClient = new RGOClientDescriptions(ServerAddress, DescriptionServicePort, "DescriptionService");

            CycleTimer = new Timer(Run);
            if (runFromTimer) CycleTimer.Change(0, cycleTime);
        }

        public RGOClientManager()
        {
            InstanceCounter++;
            if (InstanceCounter > 1)
            {
                throw new Exception("Only one instance of the RGOclientManager may be created in this scope");
            }

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
                new Transition("ServerConnected", () => ServerComm.ServerConnected, () => RGOClientDownloader.Connect(), RGOStates.DownloadingAllRGO),
                new Transition("ConnectionRejected", () => ServerComm.ConnectionRejected, () => log.Error("The connection was rejected by the server."), RGOStates.ConnectingToServer)
            }, () => ServerCommError = false, StateType.entry);

            SM.AddState(RGOStates.DownloadingAllRGO, new List<Transition>
            {
                new Transition("DownloadDone", () => RGOClientDownloader.RGODownloadDone == true, () => DSCClient.Connect(),  RGOStates.DownloadingDSC),
            }, null, StateType.transition);

            SM.AddState(RGOStates.DownloadingDSC, new List<Transition>
            {
                new Transition("DescriptionsReceived", () => DSCClient.DSCsReceived == true, () => 
                {
                    log.Info("All RGO downloads done");
                    foreach (var C in RGOClientBase.AllClients) C.Connect();
                }, RGOStates.Running),
            }, null, StateType.transition);

            SM.AddState(RGOStates.Running, new List<Transition>
            {
                new Transition("ServerCommDisconnected", () => ServerComm.ServerConnected == false, null, RGOStates.Disconnecting),
                new Transition("ServerCommErrorReported", () => ServerCommError == true, null, RGOStates.Disconnecting),
            }, null, StateType.idle);

            SM.AddState(RGOStates.Disconnecting, new List<Transition>
            {
                new Transition("AllClientsDisconnected", () => true, null, RGOStates.Disconnected),
            },
            () =>
            {
                foreach (var C in RGOClientBase.AllClients) C.Disconnect();
            }, StateType.transition);

            SM.AddState(RGOStates.Disconnected, new List<Transition>
            {
                new Transition("Reconnect", () => Reconnect == true, () =>
                {
                    ServerComm.Connect();
                }
                , RGOStates.ConnectingToServer)  
            }, null, StateType.end);

            SM.Finalize();
        }


        private static void Run(object o)
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
