using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    [Serializable]
    public class RequestRW : RGORequestBase
    {
        public List<int> ReqList = new List<int>();
        public List<RGOBase> WriteList = new List<RGOBase>();
    }

    [Serializable]
    public class ReplyRW
    {
        public List<RGOBase> FWOList = new List<RGOBase>();
        public List<bool> WriteOKList = new List<bool>();
        public bool ConnectionAccepted = false;
    }
}
