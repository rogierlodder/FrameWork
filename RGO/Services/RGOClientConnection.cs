using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    public class RGOClientConnection
    {
        public string ConnectionName { get; set; }
        public bool IsLocal { get; set; }

        public long ClientCommCounter { get; set; } = 0;
        public long PrevClientCommCounter { get; set; } = 0;
        public bool ClientConnected { get; set; } =  false;
        public int ClientDisconnectCounter { get; set; } = -1;

        public int Index { get; set; }
        public int BatchSize { get; set; }

        public RGOClientConnection(uint ID)
        {
            RGOBase.AddClientID(ID);
        }

        public void DeleteRGORef(uint ID)
        {
            RGOBase.RemoveClientID(ID);
        }

        
    }
}
