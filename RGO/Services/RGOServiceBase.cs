using System.Collections.Generic;

namespace RGF
{ 
    public abstract class RGOServiceBase 
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("RGOServiceBase");

        public static List<RGOServiceBase> AllServers { get; protected set; } = new List<RGOServiceBase>();

        public static Dictionary<uint, RGOClientConnection> ClientSessions { get; set; } = new Dictionary<uint, RGOClientConnection>();

        public string Name { get; protected set; }

        public abstract void Run();

        protected abstract bool RemoveClient(uint clientID);

        private static List<uint> removeList = new List<uint>();

        public static void RunAllServices()
        {
            foreach (var Server in AllServers) Server.Run();
            
            //check for disconnected clients
            if (removeList.Count == 0) return;
            foreach (var ClientID in removeList)
            {
                foreach (var Server in AllServers)
                {
                    if (Server.RemoveClient(ClientID)) log.Debug($"Client {ClientID} was removed from the client list of the {Server.Name}");
                    else log.Debug($"Client {ClientID} could not be removed from the client list of the {Server.Name}");
                }
                //remove the client from the RGO elements
                ClientSessions[ClientID].DeleteFWORef(ClientID);


                //erase the client from the sesions list
                ClientSessions.Remove(ClientID);
            }
            removeList.Clear();
        }

        //settings
        public static int LocalBatchsize { get; set; } = 500;
        public static int RemoteBatchsize { get; set; } = 10;
        public static int DisconnectCount { get; set; } = 20;
        public static int maxNrClients { get; set; } = 3;

        protected static void AddToRemoveList(uint id)
        {
            removeList.Add(id);
        }

    }
}
