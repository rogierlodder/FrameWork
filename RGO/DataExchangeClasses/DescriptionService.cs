using System;
using System.Collections.Generic;


namespace RGF
{
    [Serializable]
    public class DescriptionRequest : RGORequestBase
    {
        public int RequestIndex;
    }

    [Serializable]
    public class DescriptionReply
    {
        public int TotalNumber { get; set; }
        public int Index { get; set; }

        public Dictionary<string, string> Descriptions { get; set; } = new Dictionary<string, string>();
    }
}
