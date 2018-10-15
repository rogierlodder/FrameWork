using System;
using System.Threading.Tasks;

namespace RGO
{
    public class GTLClientDescriptions : GTLClient<DescriptionRequest, DescriptionReply>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("GTLClientDescriptions");

        public bool DSCsReceived { get; private set; } = false;
        private int NrReceivedDSC = 0;

        public Task ReceiveDSCsTask()
        {
            return Task.Run(() =>
            {
                while (DSCsReceived == false) Task.Delay(100);
            });
        }

        public GTLClientDescriptions(string IPAddress, int portNr, string serviceName) : base(IPAddress, portNr, serviceName)
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
                    RGOBase.AllFWO[F.Key].Description = F.Value;
                }
            }
            catch { throw new Exception("Unknown FWO received on the Description service"); }

            NrReceivedDSC += Reply.Descriptions.Count;
            Request.RequestIndex += Reply.Descriptions.Count;

            log.Debug($"number of received Description {NrReceivedDSC}");

            if (NrReceivedDSC >= Reply.TotalNumber)
            {
                DSCsReceived = true;

                //the description service only has to run once. It will not poll the entire FWO list
                Running = false;
            }
        }
    }
}
