using System;
using System.Threading.Tasks;

namespace RGF
{
    public class GTLClientFWO : GTLClient<RequestAllFWO, ReplyAllFWO>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("GTLClientFWO");

        public bool FWODownloadDone { get; private set; } = false;

        private int NrReceivedFWO = 0;

        public GTLClientFWO(string IPAddress, int portNr, string serviceName) : base(IPAddress, portNr, serviceName)
        {
            if (ClientID == 0)
            {
                throw new Exception("The client ID is undefined");
            }

            Reply = new ReplyAllFWO();

            Request = new RequestAllFWO();

            Request.ClientID = ClientID;
            Request.RequestIndex = 0;
            FWODownloadDone = false;
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

            log.Debug($"number of received FWO {NrReceivedFWO}");

            if (NrReceivedFWO >= Reply.TotalNumber)
            {
                FWODownloadDone = true;

                //the FWO service only has to run once. It will not poll the entire FWO list
                Running = false;
            }

        }
    }
}
