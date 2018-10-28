using System;

namespace RGF
{
    [Serializable]
    public class CClientInfo : RGORequestBase
    {
        public long Counter;
    }

    [Serializable]
    public class ServerInfo
    {
        public bool ConnectionAccepted { get; set; } = false;

        public long DT { get; private set; }

        public int CycleTime { get; set; }

        public long SessionCounter { get; set; }

        public void SetTime()
        {
            DT = DateTime.Now.ToBinary();
        }

        public DateTime TimeStamp
        {
            get { return DateTime.FromBinary(DT); }
        }
    }
}

