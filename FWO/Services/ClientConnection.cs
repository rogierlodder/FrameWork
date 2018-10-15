using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGO
{
    public enum FWODownloadStates { Idle, DownLoading, Finished }

    public class ClientConnection : EthernetCommunication.ConnectionBase
    {
        public string ConnectionName { get; set; }
        public bool IsLocal { get; set; }

        public long ClientCommCounter { get; set; } = 0;
        public long PrevClientCommCounter { get; set; } = 0;
        public bool ClientConnected { get; set; } =  false;
        public int ClientDisconnectCounter { get; set; } = -1;

        public FWODownloadStates st { get; set; } = FWODownloadStates.Idle;
        public int Index { get; set; }
        public int BatchSize { get; set; }

        public ClientConnection(uint ID)
        {
            RGOBase.AddClientID(ID);
        }

        public void DeleteFWORef(uint ID)
        {
            RGOBase.RemoveClientID(ID);
        }

        
    }
}
