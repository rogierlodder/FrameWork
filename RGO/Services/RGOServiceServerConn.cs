using System;
using System.Collections.Generic;

namespace RGF
{
    public class RGOServiceServerConn : RGOService<CClientInfo, ServerInfo>
    {
        public Action<bool, uint> ConnStateChanged { get; set; }

        protected override void MonitorConnection()
        {
            foreach (var C in ClientSessions)
            {
                if (C.Value.ClientCommCounter != C.Value.PrevClientCommCounter)
                {

                    C.Value.ClientCommCounter = C.Value.PrevClientCommCounter;
                    C.Value.ClientDisconnectCounter = 0;
                    if (C.Value.ClientConnected == false)
                    {
                        C.Value.ClientConnected = true;

                        ConnStateChanged?.Invoke(true, C.Key);
                    }
                }
                else
                {
                    C.Value.ClientDisconnectCounter++;
                    if (C.Value.ClientDisconnectCounter > RGOServiceBase.DisconnectCount 
                        && C.Value.ClientConnected == true)
                    {
                        C.Value.ClientConnected = false;

                        ConnStateChanged?.Invoke(false, C.Key);
                        C.Value.ClientDisconnectCounter = 0;

                        //signal that this client is no longer connected and can be erased from existence
                        AddToRemoveList(C.Key);
                    }
                }
            }
        }

        public RGOServiceServerConn(string name, int portNr) : base(name, portNr)
        {

        }


        public override ServerInfo ProcessData(CClientInfo Request)
        {
            ServerInfo Reply = new ServerInfo();
            uint ID = Request.ClientID;

            if (ClientSessions.ContainsKey(ID) == false)
            {
                if (ClientSessions.Count < RGOServiceBase.maxNrClients)
                {
                    var clientConn = new RGOClientConnection(ID)
                    {
                        IsLocal = Connection.IsOnLoopback,
                        BatchSize = RGOServiceBase.LocalBatchsize,
                        ConnectionName = Connection.Address
                    };
                    if (!Connection.IsOnLoopback) clientConn.BatchSize = RGOServiceBase.RemoteBatchsize;

                    ClientSessions.Add(ID, clientConn);

                    
                    Reply.ConnectionAccepted = true;
                }
                else Reply.ConnectionAccepted = false;
            }
            else
            {
                ClientSessions[ID].ClientCommCounter = Request.Counter;
                log.Debug($"Client: {Request.ClientID}, counter:{Request.Counter}");

                Reply.SetTime();
                Reply.SessionCounter = Request.Counter;
                Reply.CycleTime = RGOServiceStarter.MeasuredCycleTime;
            }

            return Reply;
        }
    }
}
