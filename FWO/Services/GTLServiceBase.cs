using System.Collections.Generic;

namespace FWO
{
    public abstract class GTLServiceBase 
    {
        public static List<GTLServiceBase> AllServers { get; protected set; } = new List<GTLServiceBase>();

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
                    Server.RemoveClient(connName);
                }
                //remove the client from the FWO elements
                ClientSessions[ClientID].DeleteFWORef(ClientID);

                //erase the client from the sesions list
                ClientSessions.Remove(ClientID);
            }
            removeList.Clear();
        }

        protected static void AddToRemoveList(uint id)
        {
            removeList.Add(id);
        }

    }
}
