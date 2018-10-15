
using System;
using System.Collections.Generic;
using System.Linq;

namespace RGF
{
    public class GTLServiceDescriptions : RGOService<DescriptionRequest, DescriptionReply>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("GTLServiceDescriptions");

        Dictionary<int, string> LocalDict = new Dictionary<int, string>();
        List<int> IDList = new List<int>();

        public GTLServiceDescriptions(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            Request = new DescriptionRequest();
            Reply = new DescriptionReply();

            LocalDict = RGOBase.AllFWO.Select(p => p.Value).ToDictionary(p => p.ID, p => p.Description);
            IDList = RGOBase.AllFWO.Select(p => p.Value.ID).ToList();
        }

        public override bool ProcessData()
        {
            uint ID = Request.ClientID;
            if (!ClientSessions.ContainsKey(ID)) return false;

            log.Debug($"Requested description {Request.RequestIndex}");

            int start = Math.Min(LocalDict.Count - 1, Request.RequestIndex);
            int nr = Math.Min(LocalDict.Count - start, ClientSessions[ID].BatchSize);
            if (nr == 0) return false;

            Reply.Index = start;
            Reply.TotalNumber = IDList.Count;

            Reply.Descriptions.Clear();
            foreach (var I in IDList.GetRange(start, nr)) Reply.Descriptions.Add(I, LocalDict[I]);

            return true;
        }
    }
}
