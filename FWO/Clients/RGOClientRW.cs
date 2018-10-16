using System;

namespace RGF
{
    public class RGOClientRW : RGOClient<RequestRW, ReplyRW>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("RGOClientRW");

        public delegate void CreateWriteListDelegate();
        public CreateWriteListDelegate CreateWriteList;

        public RGOClientRW(string IPAddress, string serviceName) : base(IPAddress, RGOStarterBase.RWServicePort, serviceName)
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
                RGOBase.AllRGO[id].CopyValues(F);
                RGOBase.AllRGO[id].Update?.Invoke();
            }
        }
    }
}
