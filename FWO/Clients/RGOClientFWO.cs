using System;
using System.Threading.Tasks;

namespace RGF
{
    public class RGOClientFWO : RGOClient<RequestAllRGO, ReplyAllRGO>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("RGOClientFWO");

        public bool RGODownloadDone { get; private set; } = false;

        private int NrReceivedFWO = 0;

        public RGOClientFWO(string IPAddress, int portNr, string serviceName) : base(IPAddress, portNr, serviceName)
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

        protected override void CreateRequest() { }

        protected override void ProcessReply()
        {
            WaitingForReply = false;
            //if (Reply.FWOBjects.Count != 0) WaitingForReply = false;
            //else return;

            foreach (var F in Reply.FWOBjects)
            {
                
                F.AddToFWOList();
            }

            NrReceivedFWO += Reply.FWOBjects.Count;
            Request.RequestIndex += Reply.FWOBjects.Count;

            log.Debug($"number of received RGO {NrReceivedFWO}");

            if (NrReceivedFWO >= Reply.TotalNumber)
            {
                RGODownloadDone = true;

                //the RGO service only has to run once. It will not poll the entire RGO list
                Running = false;
            }

        }
    }
}
