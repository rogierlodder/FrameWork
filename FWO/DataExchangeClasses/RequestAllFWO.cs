using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWO
{
    [Serializable]
    public class RequestAllFWO : FWORequestBase
    {
        public int RequestIndex;
    }

    [Serializable]
    public class ReplyAllFWO
    {
        public int TotalNumber { get; set; }
        public int Index { get; set; }

        public List<FWOBase> FWOBjects;
    }
}
