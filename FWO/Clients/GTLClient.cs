using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using EthernetCommunication;
using System.Diagnostics;
using System.Threading;

namespace RGO
{
    public abstract class GTLClient<TRequest, TReply> : GTLClientBase where TRequest : class
                                                                      where TReply : class
    {
        public TRequest Request { get;  set; }
        public TReply Reply { get; protected set; }
        public bool IsConnected { get { return client.ConnState == CEthernetDevice.State.Connected; } }
        public bool Received { get { return client.Received; } }

        protected CEthernetClient client;
        protected abstract void CreateRequest();
        protected abstract void ProcessReply();

        protected byte[] receiveBuf = new byte[65536];
        protected bool Running = false;
        protected bool WaitingForReply = false;

        private log4net.ILog log = log4net.LogManager.GetLogger("GTLClient<>");
        private MemoryStream memStream = new MemoryStream();
        private BinaryFormatter formatter = new BinaryFormatter();
        private Stopwatch sw = new Stopwatch();

        public GTLClient(string IPAddress, int portNr, string serviceName)
        {
            Name = serviceName;
            try
            {
                client = new CEthernetClient("");
                client.SetConnection(IPAddress, portNr, "TCP");
                //client.ConnectAndStart();
                client.Connect();
                client.ByteDataReceived = DataReceived;
            }
            catch
            {
                log.Error($"GTLClientBase: The name:{Name} does not occur in the GTLService list in the configuration ");
            }
            AllClients.Add(this);
        }

        public void Stop()
        {
            Running = false;
        }

        public void Start()
        {
            Running = true;
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public bool SendSerializedData()
        {
            CreateRequest();

            memStream.Seek(0, SeekOrigin.Begin);
            formatter.Serialize(memStream, Request);

            if (client.SendData(memStream.GetBuffer(), (int)memStream.Length, receiveBuf)) return true;
            else return false;
        }

        public void ProcessReceivedData()
        {
            if (client.NrReceivedBytes > 0)
            {
                log.Debug($"Received bytes: {client.NrReceivedBytes}");
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.Write(receiveBuf, 0, client.NrReceivedBytes);
                memStream.Seek(0, SeekOrigin.Begin);
                try
                {
                    Reply = (TReply)formatter.Deserialize(memStream);

                    //process received data
                    ProcessReply();
                }
                catch { log.Error($"Non-deserializable data has been received on the {Name} service"); }
            }
        }

        public override void Run()
        {
            if (!Running) return;

            //run the state machine of the ethernet client
            client.Run();

            if (IsConnected)
            {
                if (WaitingForReply == false)
                {
                    SendSerializedData();
                    WaitingForReply = true;
                }
            }
        }

        private void DataReceived(int nrBytes)
        {
            ProcessReceivedData();
        }
    }

   
}
