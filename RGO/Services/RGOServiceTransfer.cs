﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RGF
{
    public class RGOServiceTransfer : RGOService<RequestAllRGO, ReplyAllRGO>
    {
        log4net.ILog log = log4net.LogManager.GetLogger("RGOServiceFWO");

        List<RGOBase> LocalFWOList = new List<RGOBase>();

        public RGOServiceTransfer(string name, int portNr, int bufferSize) : base(name, portNr, bufferSize)
        {
            LocalFWOList = RGOBase.AllRGO.Select(p => p.Value).ToList();
        }

        public override ReplyAllRGO ProcessData(RequestAllRGO Request)
        {
            var Reply = new ReplyAllRGO();
            uint ID = Request.ClientID;
            if (!ClientSessions.ContainsKey(ID)) return null;

            log.Debug($"Requested {Request.RequestIndex}");

            int start = Math.Min(LocalFWOList.Count - 1, Request.RequestIndex);
            int nr = Math.Min(LocalFWOList.Count - start, ClientSessions[ID].BatchSize);
            if (nr == 0) return null;

            Reply.Index = start;
            Reply.TotalNumber = LocalFWOList.Count;
            Reply.FWOBjects = LocalFWOList.GetRange(start, nr);
            return Reply;
        }
    }
}
