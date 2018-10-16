using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGF
{
    [Serializable]
    public class RequestAllRGO : RGORequestBase
    {
        public int RequestIndex;
    }

    [Serializable]
    public class ReplyAllRGO
    {
        public int TotalNumber { get; set; }
        public int Index { get; set; }

        public List<RGOBase> FWOBjects;
    }
}
