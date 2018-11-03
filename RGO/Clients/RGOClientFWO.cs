using System;
using System.Threading.Tasks;

namespace RGF
{
    public class RGOClientRGO : RGOClient<RequestAllRGO, ReplyAllRGO>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("RGOClientRGO");

        public bool RGODownloadDone { get; private set; } = false;

        private int NrReceivedRGO = 0;

        public RGOClientRGO(string IPAddress, int portNr, string serviceName) : base(IPAddress, portNr, serviceName)
        {
            if (ClientID == 0)
            {
                throw new Exception("The client ID is undefined");
            }

            Reply = new ReplyAllRGO();

            Request = new RequestAllRGO();

            Request.ClientID = ClientID;
            Request.RequestIndex = 0;
            RGODownloadDone = false;
        }

        public override void Disconnect()
        {
            base.Disconnect();
            Request.RequestIndex = 0;
            RGODownloadDone = false;
            NrReceivedRGO = 0;
        }

        protected override void CreateRequest() { }

        protected override void ProcessReply()
        {
            WaitingForReply = false;
            //if (Reply.FWOBjects.Count != 0) WaitingForReply = false;
            //else return;

            foreach (var F in Reply.RGObjects)
            {
                 F.AddToRGOList();
            }

            NrReceivedRGO += Reply.RGObjects.Count;
            Request.RequestIndex += Reply.RGObjects.Count;

            log.Debug($"number of received RGO {NrReceivedRGO}");

            if (NrReceivedRGO >= Reply.TotalNumber)
            {
                RGODownloadDone = true;

                //the RGO service only has to run once. It will not poll the entire RGO list
                Running = false;
            }

        }
    }
}
