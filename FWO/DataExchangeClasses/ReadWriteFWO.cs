using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWO
{
    [Serializable]
    public class RequestRW : FWORequestBase
    {
        public List<int> ReqList = new List<int>();
        public List<FWOBase> WriteList = new List<FWOBase>();
    }

    [Serializable]
    public class ReplyRW
    {
        public List<FWOBase> FWOList = new List<FWOBase>();
        public List<bool> WriteOKList = new List<bool>();
        public bool ConnectionAccepted = false;
    }
}
