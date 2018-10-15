using System;
using System.Collections.Generic;
using System.Linq;

namespace RGF
{
    public class GTLServiceFWO : GTLService<RequestAllFWO, ReplyAllFWO>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("GTLServiceFWO");

        List<RGOBase> LocalFWOList = new List<RGOBase>();

        public GTLServiceFWO(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            Request = new RequestAllFWO();
            Reply = new ReplyAllFWO();

            LocalFWOList = RGOBase.AllFWO.Select(p => p.Value).ToList();
        }

        public override bool ProcessData()
        {
            uint ID = Request.ClientID;
            if (!ClientSessions.ContainsKey(ID)) return false;

            log.Debug($"Requested {Request.RequestIndex}");

            int start = Math.Min(LocalFWOList.Count - 1, Request.RequestIndex);
            int nr = Math.Min(LocalFWOList.Count - start, ClientSessions[ID].BatchSize);
            if (nr == 0) return false;

            Reply.Index = start;
            Reply.TotalNumber = LocalFWOList.Count;
            Reply.FWOBjects = LocalFWOList.GetRange(start, nr);
            return true;
        }
    }
}
