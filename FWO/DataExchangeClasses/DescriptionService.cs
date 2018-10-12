using System;
using System.Collections.Generic;


namespace FWO
{
    [Serializable]
    public class DescriptionRequest : FWORequestBase
    {
        public int RequestIndex;
    }

    [Serializable]
    public class DescriptionReply
    {
        public int TotalNumber { get; set; }
        public int Index { get; set; }

        public Dictionary<int, string> Descriptions { get; set; } = new Dictionary<int, string>();
    }
}
