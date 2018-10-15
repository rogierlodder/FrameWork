using System.Collections.Generic;

namespace RGF
{
    public abstract class RGOServiceBase 
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("GTLServiceBase");

        public static List<RGOServiceBase> AllServers { get; protected set; } = new List<RGOServiceBase>();

        public static Dictionary<uint, ClientConnection> ClientSessions { get; set; } = new Dictionary<uint, ClientConnection>();

        public string Name { get; protected set; }

        public abstract void Run();

        protected abstract void RemoveClient(string name);

        private static List<uint> removeList = new List<uint>();

        public static void RunAllServices()
        {
            foreach (var Server in AllServers) Server.Run();

            if (removeList.Count == 0) return;

            //check for disconnected clients
            foreach (var ClientID in removeList)
            {
                foreach (var Server in AllServers)
                {
                    //remove all connections from the server
                    string connName = ClientSessions[ClientID].ConnectionName;
                    log.Debug($"Client {connName} was removed from the client list");
                    Server.RemoveClient(connName);
                }
                //remove the client from the FWO elements
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
        public static int maxNrClients { get; set; } = 4;

        protected static void AddToRemoveList(uint id)
        {
            removeList.Add(id);
        }

    }
}
