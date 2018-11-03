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

namespace RGF
{
    public abstract class RGOClient<TRequest, TReply> : RGOClientBase where TRequest : class
                                                                      where TReply : class
    {
        public TRequest Request { get;  set; }
        public TReply Reply { get; protected set; }
        public bool IsConnected { get { return client.ConnState == CEthernetDevice.State.Connected; } }
        public bool Received { get { return client.Received; } }
        public bool Running { get; set; } = false;
        public Action<string, string> ReportError { get; set; }

        protected CEthernetClient client;
        protected abstract void CreateRequest();
        protected abstract void ProcessReply();

        protected byte[] receiveBuf = new byte[65536];
        protected bool WaitingForReply = false;

        private log4net.ILog log;
        private MemoryStream memStream = new MemoryStream();
        private BinaryFormatter formatter = new BinaryFormatter();
        private Stopwatch sw = new Stopwatch();

        public RGOClient(string IPAddress, int portNr, string serviceName)
        {
            Name = serviceName;
            log = log4net.LogManager.GetLogger($"RGOClient<{Name}>");
            try
            {
                client = new CEthernetClient("");
                client.SetConnection(IPAddress, portNr, "TCP");
                client.ByteDataReceived = ProcessReceivedData;
            }
            catch
            {
                log.Error($"RGOClientBase: The name:{Name} does not occur in the RGOService list in the configuration ");
            }
            AllClients.Add(this);
        }

        public override void Disconnect()
        {
            client.Disconnect();
            Running = false;
        }

        public override void Connect()
        {
            client.Connect();
            Running = true;
            WaitingForReply = false;
        }

        public void Stop()
        {
            Running = false;
        }

        public void Start()
        {
            Running = true;
        }

        public void ProcessReceivedData(int nrBytes)
        {
            if (nrBytes > 0)
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

        public override bool Run()
        {
            if (!Running) return false;

            //run the state machine of the ethernet client
            client.Run();

            if (IsConnected)
            {
                if (WaitingForReply == false)
                {
                    CreateRequest();

                    memStream.Seek(0, SeekOrigin.Begin);
                    formatter.Serialize(memStream, Request);

                    if (client.SendData(memStream.GetBuffer(), (int)memStream.Length, receiveBuf))
                    {
                        WaitingForReply = true;
                        return true;
                    }
                    else return false;
                }
            }
            return false;
        }
    }

   
}
