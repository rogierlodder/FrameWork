using EthernetCommunication;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Diagnostics;

namespace RGF
{
    public abstract class RGOService<TRequest, TReply> : RGOServiceBase where TRequest : class
                                                                        where TReply : class
    {
        log4net.ILog log = log4net.LogManager.GetLogger("RGOService");

        public int NrConnections { get { return server.NrConnections; } }
        public TRequest Request;
        public TReply Reply;
        public abstract bool ProcessData();

        protected ConnectionBase Connection { get; private set; }

        private MemoryStream memStream = new MemoryStream();
        private BinaryFormatter binFormatter = new BinaryFormatter();
        private CEthernetServer<ConnectionBase> server;
        private int clientBuffeSize = 65536;
        private Stopwatch sw = new Stopwatch();

        public RGOService(string name, int portNr)
        {
            Name = name;
            Setup(portNr);
        }

        public RGOService(string name, int portNr, int bufferSize)
        {
            Name = name;
            clientBuffeSize = bufferSize;
            Setup(portNr);
        }

        private void Setup(int portNr)
        {
            try
            {
                server = new CEthernetServer<ConnectionBase>(Name,"0.0.0.0", portNr, "TCP", clientBuffeSize, 0);
                log.Info($"{Name} started on port {portNr}");
            }
            catch
            {
                log.Error($"The name:{Name} does not occur in the RGOSerice list in the configuration ");
            }
            AllServers.Add(this);
        }

        protected virtual void MonitorConnection() { }

        protected override bool RemoveClient(uint clientID)
        {
            var ConnectedClients = server.AllConnectionsList.Where(p => p.ClientID == clientID);
            if (ConnectedClients != null)
            {
                foreach (var client in ConnectedClients)
                {
                    server.RemoveConnection(client.Address);
                }
                return true;
            }
            else return false;
        }
   

        public override void Run()
        {
            MonitorConnection();

            for (int i = 0; i < server.AllConnectionsList.Count; i++)
            {
                Connection = server.AllConnectionsList[i];

                if (Connection != null && Connection.DataReceived)
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.Write(Connection.IncomingData, 0, Connection.NrReceivedBytes);
                    Connection.DataReceived = false;
                    memStream.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        Request = (TRequest)binFormatter.Deserialize(memStream);

                        //identify the client with the client ID from the incoming request
                        Connection.ClientID = (Request as RGORequestBase).ClientID;
                    }
                    catch
                    {
                        log.Error($"Non=serializable packet received on the {Name} service");
                        continue;
                    }

                    //prepare the server reply
                    if (ProcessData() == true)
                    {
                        //send the reply
                        memStream.Seek(0, SeekOrigin.Begin);
                        binFormatter.Serialize(memStream, Reply);
                        Connection.SendDataAsync(memStream.GetBuffer(), (int)memStream.Length);
                    }
                }
            }
        }
    }
}
