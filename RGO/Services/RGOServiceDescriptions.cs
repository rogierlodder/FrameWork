
using System;
using System.Collections.Generic;
using System.Linq;

namespace RGF
{
    public class RGOServiceDescriptions : RGOService<DescriptionRequest, DescriptionReply>
    {
        //static log4net.ILog log = log4net.LogManager.GetLogger("RGOServiceDescriptions");

        Dictionary<string, string> LocalDict = new Dictionary<string, string>();
        List<string> IDList = new List<string>();

        public RGOServiceDescriptions(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            log = log4net.LogManager.GetLogger("RGOServiceDescriptions");
            LocalDict = RGOBase.AllRGO.Select(p => p.Value).ToDictionary(p => p.ID, p => p.Description);
            IDList = RGOBase.AllRGO.Select(p => p.Value.ID).ToList();
        }

        public override DescriptionReply ProcessData(DescriptionRequest Request)
        {
            var Reply = new DescriptionReply();
            uint ID = Request.ClientID;
            if (!ClientSessions.ContainsKey(ID)) return null;

            log.Debug($"Requested description {Request.RequestIndex}");

            int start = Math.Min(LocalDict.Count - 1, Request.RequestIndex);
            int nr = Math.Min(LocalDict.Count - start, ClientSessions[ID].BatchSize);
            if (nr == 0) return null;

            Reply.Index = start;
            Reply.TotalNumber = IDList.Count;

            Reply.Descriptions.Clear();
            foreach (var I in IDList.GetRange(start, nr)) Reply.Descriptions.Add(I, LocalDict[I]);

            return Reply;
        }
    }
}
