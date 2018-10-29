using System;
using System.Threading.Tasks;

namespace RGF
{
    public class RGOClientDescriptions : RGOClient<DescriptionRequest, DescriptionReply>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("RGOClientDescriptions");

        public bool DSCsReceived { get; private set; } = false;
        private int NrReceivedDSC = 0;

        public Task ReceiveDSCsTask()
        {
            return Task.Run(() =>
            {
                while (DSCsReceived == false) Task.Delay(100);
            });
        }

        public RGOClientDescriptions(string IPAddress, int portNr, string serviceName) : base(IPAddress, portNr, serviceName)
        {
            if (ClientID == 0)
            {
                throw new Exception("The client ID is undefined");
            }

            Reply = new DescriptionReply();
            Request = new DescriptionRequest();

            Request.ClientID = ClientID;
            Request.RequestIndex = 0;
            DSCsReceived = false;
        }

        public override void Disconnect()
        {
            base.Disconnect();
            NrReceivedDSC = 0;
            Request.ClientID = ClientID;
            Request.RequestIndex = 0;
            DSCsReceived = false;
        }


        protected override void CreateRequest()
        {
            

        }

        protected override void ProcessReply()
        {
            WaitingForReply = false;
            try
            {
                foreach (var F in Reply.Descriptions)
                {
                    RGOBase.AllRGO[F.Key].Description = F.Value;
                }
            }
            catch { throw new Exception("Unknown RGO received on the Description service"); }

            NrReceivedDSC += Reply.Descriptions.Count;
            Request.RequestIndex += Reply.Descriptions.Count;

            log.Debug($"number of received Description {NrReceivedDSC}");

            if (NrReceivedDSC >= Reply.TotalNumber)
            {
                DSCsReceived = true;

                //the description service only has to run once. It will not poll the entire RGO list
                Running = false;
            }
        }
    }
}
