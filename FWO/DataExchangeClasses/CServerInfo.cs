using System;

namespace RGF
{
    [Serializable]
    public class CClientInfo : FWORequestBase
    {
        public long Counter;
    }

    [Serializable]
    public class CServerInfo
    {
        public bool ConnectionAccepted { get; set; }

        public long DT { get; private set; }

        public int CycleTime { get; set; }

        public int SessionCounter { get; set; }

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

