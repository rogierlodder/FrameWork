using System.Collections.Generic;

namespace FWO
{
    public class GTLServiceServerConn : GTLService<CClientInfo, CServerInfo>
    {
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

                        FWOServiceStarter.ConnStateChanged?.Invoke(true, C.Key);
                    }
                }
                else
                {
                    C.Value.ClientDisconnectCounter++;
                    if (C.Value.ClientDisconnectCounter > FWOSettings.DisconnectCount 
                        && C.Value.ClientConnected == true)
                    {
                        C.Value.ClientConnected = false;

                        FWOServiceStarter.ConnStateChanged?.Invoke(false, C.Key);
                        C.Value.ClientDisconnectCounter = 0;

                        //signal that this client is no longer connected and can be erased from existence
                        AddToRemoveList(C.Key);
                    }
                }
            }
        }

        public GTLServiceServerConn(string name, int portNr) : base(name, portNr)
        {
            Request = new CClientInfo();
            Reply = new CServerInfo();
        }


        public override bool ProcessData()
        {
            uint ID = Request.ClientID;

            if (ClientSessions.ContainsKey(ID) == false)
            {
                if (ClientSessions.Count < FWOSettings.maxNrClients)
                {
                    var clientConn = new ClientConnection(ID)
                    {
                        IsLocal = Connection.IsOnLoopback,
                        BatchSize = FWOSettings.LocalBatchsize,
                        ConnectionName = Connection.Address
                    };
                    if (!Connection.IsOnLoopback) clientConn.BatchSize = FWOSettings.RemoteBatchsize;

                    ClientSessions.Add(ID, clientConn);

                    Reply.ConnectionAccepted = true;
                }
                else Reply.ConnectionAccepted = false;
            }
            else
            {
                ClientSessions[ID].ClientCommCounter = Request.Counter;

                Reply.SetTime();
                Reply.SessionCounter++;
                Reply.CycleTime = FWOServiceStarter.MeasuredCycleTime;
            }

            return true;
        }
    }
}
