using System;

namespace FWO
{
    public class GTLClientRW : GTLClient<RequestRW, ReplyRW>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("GTLClientRW");

        public delegate void CreateWriteListDelegate();
        public CreateWriteListDelegate CreateWriteList;

        public GTLClientRW(string IPAddress, string serviceName) : base(IPAddress, FWOStarterBase.RWServicePort, serviceName)
        {
            if (ClientID == 0)
            {
                throw new Exception("The client ID is undefined");
            }

            Request = new RequestRW();
            Request.ClientID = ClientID;

            Reply = new ReplyRW();
        }

        protected override void CreateRequest()
        {
            CreateWriteList?.Invoke();
        }

        protected override void ProcessReply()
        {
            WaitingForReply = false;

            log.Debug($"ClientRW {Reply.FWOList.Count} objects received");

            foreach (var F in Reply.FWOList)
            {
                int id = F.ID;
                FWOBase.AllFWO[id].CopyValues(F);
                FWOBase.AllFWO[id].Update?.Invoke();
            }
        }
    }
}
